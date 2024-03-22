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

        /// <summary>
        /// 屏幕坐标转UI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="rect"></param>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public static Vector2 ScreenToUIPos(this Camera camera, RectTransform rect, Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos,
                camera, out Vector2 uiPos);
            return uiPos;
        }
        
        /// <summary>
        /// 十六进制转Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}