using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class RichTextTypeWriter : MonoBehaviour
{
    /// <summary>
    /// 面板文本
    /// </summary>
    [TextArea(5, 10)]
    public string       EditorText;
    /// <summary>
    /// 默认打字速度
    /// </summary>
    public float        TypeSpeed = 0.05f;
    /// <summary>
    /// 是否正在打字
    /// </summary>
    public bool         IsTyping => m_IsTyping;

    private string      m_Content;
    private TMP_Text    m_TMPText;
    private bool        m_IsTyping;
    private bool        m_IsSkipped;
    private string      m_JoinedTags;
    private float?      m_StaggerValue;
    
    /// <summary>
    /// 控制标签列表
    /// </summary>
    private static readonly List<string> m_ControlTagL = new()
    {
        "delay",
        "speed",
        "wait",
    };

#if ODIN_INSPECTOR && UNITY_EDITOR
    
    [Sirenix.OdinInspector.Button("Play")]
    public async void Play()
    {
        m_Content = EditorText;
        await PlayAsync();
    }
    
#endif
    
    private void Awake()
    {
        m_TMPText    = GetComponent<TMP_Text>();
        m_JoinedTags = string.Join("|", m_ControlTagL);
    }
    
    /// <summary>
    /// 播放打字
    /// </summary>
    /// <param name="onComplete"></param>
    public async UniTask PlayAsync(Action onComplete = null)
    {
        if (m_IsTyping || m_TMPText == null)
            return;

        m_IsTyping  = true;
        m_IsSkipped = false;

        int index            = 0;                   // 文本索引
        var currentDelay= TypeSpeed;           // 当前速度 
        var tagStack         = new Stack<string>(); // 富文本标签栈
        var realText         = new StringBuilder(); // 显示文本     
        var tempText         = new StringBuilder(); // 临时文本(包含临时的富文本标签)
        var pureText         = new StringBuilder(); // 纯文本(不包含富文本标签)
        
        while (index < m_Content.Length)
        {
            if (m_IsSkipped)
            {
                m_TMPText.text = StripControlTags(m_Content);
                break;
            }
            
            if (tagStack.Count == 0)
            {
                // 没有富文本标签时，将临时文本添加到显示文本
                realText.Append(tempText);
                tempText.Clear();
                pureText.Clear();
            }
            
            if (m_Content[index] == '<')
            {
                int closeIndex = m_Content.IndexOf('>', index);
                if (closeIndex == -1)
                    break;

                string richTag = m_Content.Substring(index, closeIndex - index + 1);
                if (IsControlTag(richTag))
                {
                    currentDelay = ParseTimeValue(richTag);
                    // wait标签，等待指定时间后继续打字
                    if (richTag.StartsWith("<wait"))
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));
                        currentDelay = TypeSpeed;
                    }
                    
                    index = closeIndex + 1;
                    continue;
                }
                
                if (richTag[1] == '/') // 富文本标签闭合，弹出栈
                {
                    if (tagStack.Count > 0)
                        tagStack.Pop(); 
                }
                else                   // 富文本标签开始, 压入栈
                {
                    tagStack.Push(richTag);
                }

                index = closeIndex + 1; //将index指向标签结束的位置
                continue;
            }
            
            // 清除临时文本
            tempText.Clear();
            pureText.Append(m_Content[index]);

            // 插入富文本标签
            foreach (var richTag in tagStack.Reverse())
                tempText.Append(richTag);

            // 插入纯文本
            tempText.Append(pureText);

            // 插入富文本闭合标签
            foreach (var richTag in tagStack)
                tempText.Append(GetClosingTag(richTag));

            // 显示文本
            m_TMPText.text = realText + tempText.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));
            
            index++;
        }

        m_IsTyping = false;
        m_TMPText.text = StripControlTags(m_Content); 

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
        return Regex.IsMatch(richTag, $@"^<\/?({m_JoinedTags})(=[^>]*)?>$", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// 根据标签类型解析时间
    /// </summary>
    /// <param name="richTag"></param>
    /// <returns></returns>
    private float ParseTimeValue(string richTag)
    {
        var match = Regex.Match(richTag, @"^<\/?(?<tag>\w+)(=(?<val>[^>]+))?>$", RegexOptions.IgnoreCase);
        if (!match.Success)
            return TypeSpeed;
        
        string val = match.Groups["val"].Value;
        if (float.TryParse(val, out float time))
            return time;
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
        return match.Success ? match.Groups[1].Value : TypeSpeed.ToString();
    }

    /// <summary>
    /// 获取闭合标签
    /// </summary>
    /// <param name="openTag"></param>
    /// <returns></returns>
    private string GetClosingTag(string openTag)
    {
        int tagNameEnd = openTag.IndexOfAny(new[] { ' ', '=', '>' });
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
