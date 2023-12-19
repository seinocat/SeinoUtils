using System;
using System.Collections.Generic;
using Seino.Utils.Singleton;
using UnityEngine;

namespace Seino.Utils.Tick
{
    /// <summary>
    /// 用于update执行逻辑
    /// </summary>
    public class SeinoTicker : MonoSingleton<SeinoTicker>
    {
        private Dictionary<long, TickChannel> m_channels = new();
        private Queue<long> m_updates = new();

        private void Update()
        {
            int count = m_updates.Count;
            while (count-- > 0)
            {
                long id = m_updates.Dequeue();
                if (!m_channels.TryGetValue(id, out TickChannel channel)) continue;
                if (channel.IsPause) continue;
                
                channel.Update(Time.deltaTime);
                m_updates.Enqueue(id);
            }
        }
        
        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public TickChannel Create(Action executor, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, null, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public TickChannel Create(Func<bool> predicate, Action executor, Action callback, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, predicate, executor, null, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="executor"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public TickChannel Create(long id, Action executor, int framerate = 30)
        {
            return Create(id, null, executor, null, framerate);
        }
        
        /// <summary>
        /// 创建带有中断条件的执行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public TickChannel Create(long id, Func<bool> predicate, Action executor, Action callback, int framerate = 30)
        {
            TickChannel channel = TickChannel.Create(id, predicate, executor, callback, framerate);
            m_channels.Add(channel.Id, channel);
            m_updates.Enqueue(channel.Id);
            return channel;
        }
        
        /// <summary>
        /// 移除执行
        /// </summary>
        /// <param name="id"></param>
        public void Remove(long id)
        {
            if (m_channels.ContainsKey(id))
            {
                m_channels.Remove(id);
            }
        }

        /// <summary>
        /// 获取执行数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TickChannel GetChannel(long id)
        {
            if (m_channels.ContainsKey(id))
            {
                return m_channels[id];
            }
            return null;
        }
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="id"></param>
        public void Play(long id)
        {
            if (m_channels.ContainsKey(id))
            {
                m_channels[id].IsPause = false;
                if (!m_updates.Contains(id))
                {
                    m_updates.Enqueue(id);
                }
            }
        }
        
        /// <summary>
        /// 暂停执行
        /// </summary>
        /// <param name="id"></param>
        public void Pause(long id)
        {
            if (m_channels.ContainsKey(id))
            {
                m_channels[id].IsPause = true;
            }
        }

        /// <summary>
        /// 执行全部
        /// </summary>
        public void PlayAll()
        {
            foreach (var kv in m_channels)
            {
                kv.Value.IsPause = false;
                if (!m_updates.Contains(kv.Key))
                {
                    m_updates.Enqueue(kv.Key);
                }
            }
        }

        /// <summary>
        /// 暂停全部
        /// </summary>
        public void PasueAll()
        {
            foreach (var kv in m_channels)
            {
                kv.Value.IsPause = true;
            }
        }
    }
}