using System;
using System.Collections.Generic;
using Seino.Utils.Singleton;

namespace Seino.Utils.Tick
{
    public class Ticker : MonoSingleton<Ticker>
    {
        private void Update()
        {
            
        }

        public TickChannel CreateChannel<T>(IList<T> dataList, Action executor) where T : class
        {
            TickChannel channel = new TickChannel();
            
        }
        
        // public TickChannel CreateChannel<T1, T2>(IDictionary<T1, T2> dataList, Func<T1, T2> executer) where T2 : class
        // {
        //     Dictionary<>
        // }
    }
}