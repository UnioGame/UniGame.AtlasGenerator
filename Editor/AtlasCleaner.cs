

using UniModules.Editor;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using UniModules.UniGame.Core.Editor.Tools;
    using UnityEditor;
    using UnityEngine;
    
    public class AtlasCleaner : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            AtlasGeneratorSettings.Load(x => ProcessOnWillDeleteAsset(x,assetPath,options));
            return AssetDeleteResult.DidNotDelete;
        }

        public static void RemoveFromAtlases(string assetPath)
        {
            var assetPaths = new List<string>();
            if (assetPath.IsFilePath())
                assetPaths.Add(assetPath);
            
            else if(Directory.Exists(assetPath))
            {
                string[] filePaths = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
                assetPaths.AddRange(filePaths);
            }
            
            foreach (var asset in assetPaths)
            {
                if (AssetDatabase.GetMainAssetTypeAtPath(asset) == typeof(Texture2D) && Path.GetExtension(asset.ToLower()) != AtlasGeneratorCommands.PsbExtension)
                {
                    AtlasGeneratorCommands.RemoveSpriteFromAtlas(asset);
                }
            }
            
        }

        private static void ProcessOnWillDeleteAsset(AtlasGeneratorSettings settings,string assetPath, RemoveAssetOptions options)
        {
            if (!settings.allowPostProcessing) return;
            UniEditorProfiler.LogTime(nameof(AtlasCleaner), () => RemoveFromAtlases(assetPath));
        }
    }
}