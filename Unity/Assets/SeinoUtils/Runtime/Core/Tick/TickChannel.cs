using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public TickStatus Status => m_Status;

        private float m_IntervalTime;
        private float m_curtime;
        private float m_maxtime;
        private float m_ctime;

        private int m_frame = 1;
        private Action<float> m_executor;
        private Func<bool> m_predicate;
        private Action m_callback;
        private TickStatus m_Status;


        public static TickChannel Create(Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            TickChannel channel = new TickChannel();
            channel.m_executor = exe;
            channel.m_predicate = pre;
            channel.m_callback = call;
            channel.m_maxtime = time;
            channel.m_frame = frame;
            channel.m_IntervalTime = 1f / frame;
            return channel;
        }

        public void Update(float deltaTime)
        {
            m_curtime += deltaTime;
            m_ctime += deltaTime;
            m_Status = TickStatus.Running;
            if (m_curtime >= m_IntervalTime)
            {
                m_curtime = 0;
                if (m_predicate?.Invoke()??false)
                {
                    m_callback?.Invoke();
                    OnComplete();
                    return;
                }
                m_executor(deltaTime);
            }

            if (m_maxtime > 0f && m_ctime >= m_maxtime)
            {
                m_callback?.Invoke();
                OnComplete();
            }
        }

        public void OnComplete()
        {
            m_Status = TickStatus.Complete;
        }
    }
}