using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public TickStatus Status => m_status;

        private float m_gaptime;
        private float m_curtime;
        private float m_maxtime;
        private float m_totaltime;

        private int m_frame = 1;
        private Action<float> m_executor;
        private Func<bool> m_predicate;
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
                m_curtime = 0;
                if (m_predicate?.Invoke()??false)
                {
                    OnComplete();
                    return;
                }
                m_executor(deltaTime);
            }

            if (m_maxtime > 0f && m_totaltime >= m_maxtime)
            {
                OnComplete();
            }
        }

        public void OnComplete()
        {
            m_status = TickStatus.Complete;
            m_complete?.Invoke();
            m_complete = null;
        }
    }
}