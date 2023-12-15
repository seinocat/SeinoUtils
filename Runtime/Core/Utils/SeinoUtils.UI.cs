using UnityEngine;

namespace Seino.Utils
{
    public partial class SeinoUtils
    {
        /// <summary>
        /// 世界坐标转换UGUI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 WorldToUguiPoint(this Camera camera, Vector3 position, Rect rect)
        {
            return WorldToUguiPoint(camera, position, rect, Vector2.zero);
        }
        
        /// <summary>
        /// 世界坐标转换UGUI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Vector2 WorldToUguiPoint(this Camera camera, Vector3 position, Rect rect, Vector2 offset)
        {
            Vector3 viewPos = camera.WorldToViewportPoint(position);
            Vector2 uiPos = new Vector2(viewPos.x * rect.width, viewPos.y * rect.height) + offset;
            return uiPos;
        }
    }
}