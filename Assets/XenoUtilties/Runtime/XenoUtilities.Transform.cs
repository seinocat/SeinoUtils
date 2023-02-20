using System.Collections.Generic;
using UnityEngine;


namespace Xeno.Utilities
{
    /**
     * Transform相关
     */
    public static partial class XenoUtilities
    {
        /// <summary>
        /// 按名称查找节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static Transform GetTransformInChildren(this Transform root, string targetName, SearchOptions option = SearchOptions.Approximate)
        {
            if (option == SearchOptions.Approximate)
                return FindTransform_Approximate(root, targetName);
            return FindTransform_Accurate(root, targetName);
        }

        internal static Transform FindTransform_Approximate(Transform root, string targetName)
        {
            if (root.name.Contains(targetName)) return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Contains(targetName))
                {
                    return child;
                }

                Transform target = FindTransform_Approximate(child, targetName);
                if (target) return target;
            }

            return null;

        }
        
        internal static Transform FindTransform_Accurate(Transform root, string targetName)
        {
            if (root.name.Equals(targetName)) return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Equals(targetName))
                {
                    return child;
                }

                Transform target = FindTransform_Accurate(child, targetName);
                if (target) return target;
            }

            return null;
        }
    }
}