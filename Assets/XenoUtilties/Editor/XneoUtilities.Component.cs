using UnityEngine;

namespace XenoUtilities
{
    public static partial class XenoUtilities
    {
        /// <summary>
        /// 获取组件，没有就添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RequireComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (!component) component = obj.AddComponent<T>();
            return component;
        }
        
    }
}