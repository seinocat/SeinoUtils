using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public long Id => m_id;
        public bool IsPause;

        private float m_IntervalTime;
        private float m_Time;
        private long m_id;
        private int m_frame = 1;
        private Action m_executor;
        private Func<bool> m_predicate;
        private Action m_callback;


        public static TickChannel Create(long id, Func<bool> pre, Action exe, Action call, int frame)
        {
            TickChannel channel = new TickChannel();
            channel.m_id = id;
            channel.m_executor = exe;
            channel.m_predicate = pre;
            channel.m_callback = call;
            channel.m_frame = frame;
            return channel;
        }

        public TickChannel()
        {
            m_IntervalTime = 1f / m_frame;
        }

        public void Update(float deltaTime)
        {
            m_Time += deltaTime;
            if (m_Time >= m_IntervalTime)
            {
                m_Time = 0;
                m_executor();
                if (m_predicate?.Invoke()??false)
                {
                    m_callback?.Invoke();
                    Dispose();
                } 
            }
        }

        public void Dispose()
        {
            SeinoTicker.Instance.Remove(Id);
        }
    }
}