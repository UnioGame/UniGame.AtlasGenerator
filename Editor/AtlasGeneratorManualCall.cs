using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class AtlasGeneratorManualCall
    {
        [MenuItem("Assets/Pack To Atlas")]
        private static void PackToAtlas()
        {
            var assetPathList = new List<string>();

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    assetPathList.Add(path);
            }

            AtlasGeneratorPostprocessor.PackIntoAtlases(assetPathList.ToArray(), new string[] { }, new string[] { });
        }
    }
}