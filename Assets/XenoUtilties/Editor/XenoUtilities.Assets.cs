using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace XenoUtilities
{
    public static partial class XenoUtilities
    {
        /// <summary>
        /// 获取路径下所有指定类型的资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> LoadAllAssets<T>(string path) where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
                
                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".meta")) continue;

                    string assetName = file.FullName;
                    string assetPath = assetName.Substring(assetName.IndexOf("Assets", StringComparison.Ordinal));
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset)
                    {
                        list.Add(asset);
                    }
                }
            }
            
            return list;
        } 
    }
}