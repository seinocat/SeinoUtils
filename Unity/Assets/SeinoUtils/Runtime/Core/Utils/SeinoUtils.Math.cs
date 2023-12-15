using System;
using UnityEngine;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="increaseSpeed"></param>
        /// <param name="decreaseSpeed"></param>
        /// <returns></returns>
        public static float LerpValue(float value, float target, float increaseSpeed, float decreaseSpeed) {
            if (Math.Abs(value - target) < 0.01f) return target; 
            if (value < target) return Mathf.Clamp(value + Time.deltaTime * increaseSpeed, -Mathf.Infinity, target);
            return Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, Mathf.Infinity);
        }
        
        /// <summary>
        /// 平滑插值 shader算法
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SmoothStep(float t1, float t2, float x)
        {
            x = Mathf.Clamp((x - t1) / (t2 - t1), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }

        /// <summary>
        /// 非线性插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float LerpDamper(float value, float target, ref float velocity, float time, LerpType type)
        {
            switch (type)
            {
                case LerpType.SampleDamper:
                    return SampleDamper(value, target, ref velocity, time);
                case LerpType.Damper:
                    return Mathf.SmoothDamp(value, target, ref velocity, time);
            }

            return 0;
        }
        
        /// <summary>
        /// 简化临界阻尼插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float SampleDamper(float value, float target, ref float velocity, float time)
        {
            var omega = 2.0f / time;
            float x = omega * Time.deltaTime;
            float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = value - target;
            float temp = (velocity + omega * change) * Time.deltaTime;
            velocity = (velocity - omega * temp) * exp;
            return target + (change + temp) * exp;
        }
        
        /// <summary>
        /// 求直线和平面的交点
        /// p = R + tV
        /// </summary>
        /// <param name="linePoint">直线上的点</param>
        /// <param name="lineDir">直线方向向量(需要归一化)</param>
        /// <param name="planeNormal">平面法线</param>
        /// <param name="planePoint">平面上的点</param>
        /// <returns></returns>
        public static Vector3 LineToPlane(Vector3 linePoint, Vector3 lineDir, Vector3 planeNormal, Vector3 planePoint)
        {
            lineDir = lineDir.normalized;
            float dot1 = Vector3.Dot(planeNormal, planePoint - linePoint);
            float dot2 = Vector3.Dot(planeNormal, lineDir);

            if (dot2 == 0f) return Vector3.zero;
    
            float t = dot1 / dot2;
            return linePoint + t * lineDir;
        }
        
        /// <summary>
        /// 三角余弦定律，求角度(注意不是弧度)
        /// </summary>
        /// <param name="sideA"></param>
        /// <param name="sideB"></param>
        /// <param name="sideC"></param>
        /// <returns></returns>
        public static float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA) * Mathf.Rad2Deg;
        }
    }
}