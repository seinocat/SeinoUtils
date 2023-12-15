using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public int FrameRate = 1;

        private float m_IntervalTime;
        private float m_Time;
        public Action Executor;

        public TickChannel()
        {
            m_IntervalTime = 1f / FrameRate;
        }

        public void Update(float deltaTime)
        {
            m_Time += deltaTime;
            if (m_Time < m_IntervalTime) return;
            m_Time = 0;
            Executor();
        }
    }
}