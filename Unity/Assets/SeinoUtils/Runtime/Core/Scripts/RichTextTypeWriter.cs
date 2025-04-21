using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

[RequireComponent(typeof(TMP_Text))]
public class RichTextTypeWriter : MonoBehaviour
{
    [TextArea(3, 10)]
    public string FullText;
    public float TypeSpeed = 0f;
    public bool IsTyping => m_IsTyping;

    private TMP_Text m_TMPText;
    private bool m_IsTyping;
    private bool m_IsSkipped;
    private string m_JoinedTags;
    private float? m_StaggerValue;

    /// <summary>
    /// 控制标签列表
    /// </summary>
    private readonly List<string> m_ControlTagL = new()
    {
        "delay",
        "speed",
        "stagger",
        "wait",
    };

    [Button("Play")]
    public void Play()
    {
        PlayAsync();
    }

    private void Awake()
    {
        m_TMPText = GetComponent<TMP_Text>();
        m_JoinedTags = string.Join("|", m_ControlTagL);
    }

    /// <summary>
    /// 开始打字效果（使用UniTask，支持await）
    /// </summary>
    public async UniTask PlayAsync(string text = null, Action onComplete = null)
    {
        if (m_IsTyping)
            return;

        m_IsTyping = true;
        m_IsSkipped = false;

        float currentDelay = TypeSpeed;
        string content = text ?? FullText;
        var tagsStack = new Stack<string>();
        var output = new StringBuilder();
        int i = 0;

        while (i < content.Length)
        {
            if (m_IsSkipped)
            {
                m_TMPText.text = StripControlTags(content);
                break;
            }

            if (content[i] == '<')
            {
                int closeIndex = content.IndexOf('>', i);
                if (closeIndex == -1) 
                    break;

                string richTag = content.Substring(i, closeIndex - i + 1);
                // 控制标签
                if (IsControlTag(richTag))
                {
                    if (richTag.StartsWith("<wait"))
                    {
                        currentDelay = ParseTimeValue(richTag);
                        await UniTask.Delay(TimeSpan.FromSeconds(currentDelay)); // 延迟指定的时间
                        i = closeIndex + 1;
                        currentDelay = TypeSpeed; // 重置延迟
                        continue;
                    }

                    currentDelay = ParseTimeValue(richTag);
                    i = closeIndex + 1;
                    continue;
                }

                // 富文本标签
                output.Append(richTag);
                if (richTag[1] == '/')
                {
                    if (tagsStack.Count > 0)
                        tagsStack.Pop();
                }
                else
                {
                    tagsStack.Push(richTag);
                }

                i = closeIndex + 1;
                continue;
            }

            // 构建显示内容
            var visible = new StringBuilder();
            foreach (var richTag in tagsStack)
                visible.Insert(0, richTag);
            visible.Append(content[i]);
            foreach (var richTag in tagsStack)
                visible.Append(GetClosingTag(richTag));

            m_TMPText.text = output + visible.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));

            output.Append(content[i]);
            i++;
        }

        m_IsTyping = false;
        m_TMPText.text = StripControlTags(content);

        onComplete?.Invoke();
    }

    /// <summary>
    /// 跳过
    /// </summary>
    public void Skip()
    {
        if (m_IsTyping) m_IsSkipped = true;
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        m_IsTyping = false;
        m_IsSkipped = true;
    }

    /// <summary>
    /// 是否为控制标签
    /// </summary>
    /// <param name="richTag"></param>
    /// <returns></returns>
    private bool IsControlTag(string richTag)
    {
        // 使用正则表达式来识别控制标签（可以是自闭合标签）
        return Regex.IsMatch(richTag, $@"^<\/?({m_JoinedTags})(=[^>]*)?>$", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// 根据标签类型解析时间
    /// </summary>
    /// <param name="richTag"></param>
    /// <returns></returns>
    private float ParseTimeValue(string richTag)
    {
        foreach (var ctrTag in m_ControlTagL)
        {
            if (richTag.StartsWith($"<{ctrTag}="))
            {
                var val = ParseTagValue(ctrTag, richTag);
                
                if (ctrTag == "wait")
                {
                    if (float.TryParse(val, out float waitTime))
                        return waitTime;
                    return TypeSpeed;
                }

                if (ctrTag == "stagger")
                {
                    if (float.TryParse(val, out float s))
                        m_StaggerValue = s;
                    else if (val.Equals("slow", StringComparison.OrdinalIgnoreCase))
                        m_StaggerValue = 0.3f;
                    else if (val.Equals("fast", StringComparison.OrdinalIgnoreCase))
                        m_StaggerValue = 0.05f;
                    else if (val.Equals("word", StringComparison.OrdinalIgnoreCase))
                        m_StaggerValue = -1f; // 保留字：按词暂停
                    return TypeSpeed;
                }

                if (float.TryParse(val, out float f))
                    return f;
                return TypeSpeed;
            }

            // 关闭 stagger，恢复默认
            if (richTag.StartsWith($"</{ctrTag}"))
            {
                if (ctrTag == "stagger")
                    m_StaggerValue = null;
                return TypeSpeed;
            }
        }

        return TypeSpeed;
    }

    /// <summary>
    /// 解析标签值
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="richTag"></param>
    /// <returns></returns>
    private string ParseTagValue(string tagName, string richTag)
    {
        var pattern = $"^<{tagName}=([^>]+)>$";
        var match = Regex.Match(richTag, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>
    /// 获取闭合标签
    /// </summary>
    /// <param name="openTag"></param>
    /// <returns></returns>
    private string GetClosingTag(string openTag)
    {
        int spaceIndex = openTag.IndexOf(' ');
        int tagNameEnd = (spaceIndex != -1) ? spaceIndex : openTag.IndexOf('>');
        string tagName = openTag.Substring(1, tagNameEnd - 1);
        return $"</{tagName}>";
    }

    /// <summary>
    /// 过滤控制标签
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string StripControlTags(string text)
    {
        return Regex.Replace(text, $@"<\/?({m_JoinedTags})(=[^>]*)?>", "", RegexOptions.IgnoreCase);
    }
}
