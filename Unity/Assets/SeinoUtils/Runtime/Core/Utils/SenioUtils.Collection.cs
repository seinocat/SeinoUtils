using System;

namespace Seino.Utils
{
    public static partial class SenioUtils
    {
        private static void MoveElement<T>(ref T[] array, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Source index is out of range.");
            }

            if (targetIndex < 0 || targetIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(targetIndex), "Target index is out of range.");
            }

            // 保存要移动的元素
            T elementToMove = array[sourceIndex];

            // 移动元素
            if (sourceIndex < targetIndex)
            {
                for (int i = sourceIndex; i < targetIndex; i++)
                {
                    array[i] = array[i + 1];
                }
            }
            else if (sourceIndex > targetIndex)
            {
                for (int i = sourceIndex; i > targetIndex; i--)
                {
                    array[i] = array[i - 1];
                }
            }

            // 将元素放入目标位置
            array[targetIndex] = elementToMove;
        }
    }
}