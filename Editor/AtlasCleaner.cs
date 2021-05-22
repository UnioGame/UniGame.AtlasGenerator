using System.Collections.Generic;
using System.IO;
using UniModules.UniGame.Core.Editor.Tools;
using UniModules.UniGame.Core.EditorTools.Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class AtlasCleaner : UnityEditor.AssetModificationProcessor
    {
        const string psbExtension = ".psb";

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            UniEditorProfiler.LogTime(nameof(AtlasCleaner), () => RemoveFromAtlases(assetPath));
            return AssetDeleteResult.DidNotDelete;
        }

        static void RemoveFromAtlases(string assetPath)
        {
            var assetPaths = new List<string>();
            if (assetPath.IsFilePath())
            {
                assetPaths.Add(assetPath);
            } 
            else if(Directory.Exists(assetPath))
            {
                string[] filePaths = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
                assetPaths.AddRange(filePaths);
            }
            foreach (var asset in assetPaths)
            {
                if (AssetDatabase.GetMainAssetTypeAtPath(asset) == typeof(Texture2D) && Path.GetExtension(asset.ToLower()) != psbExtension)
                {
                    AtlasGeneratorPostprocessor.RemoveSpriteFromAtlas(asset);
                }
            }
        }
    }
}