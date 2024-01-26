using System;
using System.Collections.Generic;

namespace Seino.Utils.Tick
{
    /// <summary>
    /// TickChannel的队列
    /// </summary>
    public class Ticker
    {
        public TickStatus Status => m_status;
        public long Id => m_id;
        public Action OnComplete;
        
        private long m_id;
        private Queue<TickChannel> m_channels = new();
        private TickStatus m_status;
        
        public static Ticker Create(long id, Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            Ticker ticker = new Ticker();
            TickChannel channel = TickChannel.Create(pre, exe, call, time, frame);
            ticker.m_id = id;
            ticker.m_status = TickStatus.Idle;
            ticker.AddChannel(channel);
            return ticker;
        }
        
        public void Update(float deltaTime)
        {
            if (m_status == TickStatus.Dispose)
            {
                m_channels.Clear();
                return;
            }
            
            if (m_channels.Count > 0)
            {
                m_status = TickStatus.Running;
                var channel = m_channels.Peek();
                if (channel.Status == TickStatus.Idle) channel.Play();
                if (channel.Status == TickStatus.Running) channel.Update(deltaTime);
                if (channel.Status == TickStatus.Complete) m_channels.Dequeue();
            }
            else
            {
                Complete();
            }
        }

        public void AddChannel(TickChannel channel)
        {
            m_channels.Enqueue(channel);
        }

        
        public void AddChannel(Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            AddChannel(TickChannel.Create(pre, exe, call, time, frame));
        }

        public void Play()
        {
            m_status = TickStatus.Running;
            SeinoTicker.Instance.Add(m_id);
        }

        public void Pause()
        {
            m_status = TickStatus.Pause;
        }
        
        public void Stop()
        {
            m_status = TickStatus.Stop;
        }
        
        public void Complete()
        {
            m_status = TickStatus.Complete;
            OnComplete?.Invoke();
            OnComplete = null;
        }
        
        public void Dispose()
        {
            m_status = TickStatus.Dispose;
        }
    }
}