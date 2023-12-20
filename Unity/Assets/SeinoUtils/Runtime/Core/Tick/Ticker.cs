using System;
using System.Collections.Generic;

namespace Seino.Utils.Tick
{
    public class Ticker
    {
        public TickStatus Status => m_Status;
        public long Id => m_id;
        
        private long m_id;
        private Queue<TickChannel> m_channels = new();
        private TickStatus m_Status;
        
        public static Ticker Create(long id, Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            Ticker ticker = new Ticker();
            TickChannel channel = TickChannel.Create(pre, exe, call, time, frame);
            ticker.m_id = id;
            ticker.m_Status = TickStatus.Idle;
            ticker.AddChannel(channel);
            return ticker;
        }
        
        public void Update(float deltaTime)
        {
            if (m_channels.Count > 0)
            {
                m_Status = TickStatus.Running;
                var channel = m_channels.Peek();
                if (channel.Status == TickStatus.Running) channel.Update(deltaTime);
                if (channel.Status == TickStatus.Complete) m_channels.Dequeue();
            }
            else
            {
                OnComplete();
            }
        }

        public void AddChannel(TickChannel channel)
        {
            m_channels.Enqueue(channel);
        }

        public void Play()
        {
            m_Status = TickStatus.Running;
            SeinoTicker.Instance.Add(m_id);
        }

        public void Pause()
        {
            m_Status = TickStatus.Pause;
        }

        public void OnComplete()
        {
            m_Status = TickStatus.Complete;
            SeinoTicker.Instance.Remove(this.m_id);
        }
    }
}