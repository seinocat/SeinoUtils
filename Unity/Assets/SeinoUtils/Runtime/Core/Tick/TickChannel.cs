using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public TickStatus Status => m_status;
        
        private float m_gaptime;// update间隔
        private float m_curtime;// 本轮时间
        private float m_maxtime;// 最大执行时间
        private float m_totaltime; // 已执行时间

        private int m_frame = 1;
        private Action<float> m_executor; //执行逻辑
        private Func<bool> m_predicate; //条件判断
        private Action m_complete;
        private TickStatus m_status;
        
        public static TickChannel Create(Func<bool> pre, Action<float> exe, Action callback, float time, int frame)
        {
            TickChannel channel = new TickChannel();
            channel.m_executor = exe;
            channel.m_predicate = pre;
            channel.m_complete = callback;
            channel.m_maxtime = time;
            channel.m_frame = frame;
            channel.m_gaptime = 1f / frame;
            return channel;
        }

        public void Update(float deltaTime)
        {
            m_curtime += deltaTime;
            m_totaltime += deltaTime;
            m_status = TickStatus.Running;
            if (m_curtime >= m_gaptime)
            {
                m_curtime -= m_gaptime;
                //如果有中止条件就判断一下
                if (m_predicate?.Invoke()??false)
                {
                    OnComplete();
                    return;
                }
                m_executor(deltaTime);
            }

            //超过最大时间中止执行
            if (m_maxtime > 0f && m_totaltime >= m_maxtime)
            {
                OnComplete();
            }
        }

        public void Play()
        {
            m_status = TickStatus.Running;
        }

        public void OnComplete()
        {
            m_status = TickStatus.Complete;
            m_complete?.Invoke();
            m_complete = null;
        }
    }
}