using System;
using System.Collections.Generic;

namespace Seino.Utils.Tick
{
    public class Ticker
    {
        public bool IsPause => m_pause;
        public long Id => m_id;
        private long m_id;
        private bool m_pause;
        private Queue<TickChannel> m_channels = new();
        
        public static Ticker Create(long id, Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            Ticker ticker = new Ticker();
            TickChannel channel = TickChannel.Create(pre, exe, call, time, frame);
            ticker.m_id = id;
            ticker.m_pause = true;
            ticker.AddChannel(channel);
            return ticker;
        }
        
        public void Update(float deltaTime)
        {
            if (m_channels.Count > 0)
            {
                var channel = m_channels.Peek();
                if (channel.Status == TickStatus.Running) channel.Update(deltaTime);
                if (channel.Status == TickStatus.Complete) m_channels.Dequeue();
            }
            else
            {
                SeinoTicker.Instance.Remove(this.m_id);
            }
        }

        public void AddChannel(TickChannel channel)
        {
            m_channels.Enqueue(channel);
        }

        public void Play()
        {
            m_pause = false;
        }

        public void Pause()
        {
            m_pause = true;
        }
    }
}