using System;
using UniModules.UniGame.Core.Editor.Tools;
using UnityEditor;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    
    public class AtlasGeneratorPostprocessor : AssetPostprocessor
    {
        
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UniEditorProfiler.LogTime(nameof(AtlasGenerator), () => PackIntoAtlases(importedAssets, movedAssets, movedFromAssetPaths));
        }
        
        public static void PackIntoAtlases(string[] importedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UniEditorProfiler.LogTime(nameof(AtlasGenerator), () => OnPackIntoAtlases(importedAssets, movedAssets, movedFromAssetPaths));
        }
       
        private static void OnPackIntoAtlases(string[] importedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        { 
            var dirty = false;

            try
            {
                AssetDatabase.StartAssetEditing();
                            
                // Apply generation rules.
                foreach (var importedAsset in importedAssets)
                {
                    dirty |= AtlasGeneratorCommands.ProcessAsset(importedAsset, null);
                }

                for (var i = 0; i < movedAssets.Length; i++)
                {
                    var movedAsset = movedAssets[i];
                    var movedFromAssetPath = movedFromAssetPaths[i];
                    dirty |= AtlasGeneratorCommands.ProcessAsset(movedAsset, movedFromAssetPath);
                }
                
            }
            finally
            {
                //By adding a call to StopAssetEditing inside
                //a "finally" block, we ensure the AssetDatabase
                //state will be reset when leaving this function
                AssetDatabase.StopAssetEditing();
            }


            if (dirty)
            {
                AssetDatabase.SaveAssets();
            }
        }

    }
}