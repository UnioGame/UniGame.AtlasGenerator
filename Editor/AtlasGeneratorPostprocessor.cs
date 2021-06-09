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
            var dirty = false;

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

            if (dirty)
            {
                AssetDatabase.SaveAssets();
            }
        }

    }
}