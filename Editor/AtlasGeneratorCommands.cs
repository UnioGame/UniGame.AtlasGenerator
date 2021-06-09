using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public static class AtlasGeneratorCommands
    {
        public const string PsbExtension = ".psb";

        public static void Log(LogType logType, string message)
        {
            var logMessage = $"[{nameof(AtlasGeneratorPostprocessor)}] {message}";
            Debug.unityLogger.Log(logType, logMessage);
        }
 
        public static bool ProcessAsset(string pathToAsset, string oldPathToAsset)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(pathToAsset) == typeof(Texture2D) && Path.GetExtension(pathToAsset.ToLower()) != PsbExtension)
            {
                return ApplyGenerationRule(pathToAsset, oldPathToAsset);
            }
            return false;
        }
        
        public static bool LoadSettings(out AtlasGeneratorSettings generatorSettings, out AtlasGeneratorAtlasSettings atlasSettings) 
        {
            generatorSettings = AtlasGeneratorSettings.Asset;
            atlasSettings     = AtlasGeneratorAtlasSettings.Asset;
            
            if (generatorSettings == null)
            {
                Log(LogType.Error ,"Generator settings file not found");
                return false;
            }
            
            if (atlasSettings == null)
            {
                Log(LogType.Error, "Atlas settings file not found.");
                return false;
            }
            
            return true;
        }
        
        public static bool ApplyGenerationRule(
            string assetPath,
            string movedFromAssetPath)
        {
            var dirty = false;
            var generatorSettings = AtlasGeneratorSettings.Asset;
            var atlasSettings     = AtlasGeneratorAtlasSettings.Asset;
            
            if (!string.IsNullOrEmpty(movedFromAssetPath) && TryGetMatchedRule(movedFromAssetPath, generatorSettings, out var oldMatchedRule))
            {
                dirty |= AtlasGenerator.RemoveFromAtlas(oldMatchedRule, movedFromAssetPath, assetPath);
            }

            if (TryGetMatchedRule(assetPath, generatorSettings, out var matchedRule))
            {
                var atlas = AtlasGenerator.CreateOrUpdateAtlas(generatorSettings, atlasSettings, matchedRule, assetPath);
                dirty = true;
                Log(LogType.Log, $"Added sprite {assetPath} to atlas {atlas.name}");
            }

            return dirty;
        }
        
        public static bool TryGetMatchedRule(string assetPath, AtlasGeneratorSettings importSettings, out AtlasGeneratorRule rule)
        {
            if (importSettings.rules == null || importSettings.rules.Count == 0)
            {
                rule = null;
                return false;
            }

            foreach (var r in importSettings.rules)
            {
                if(r.enabled == false)
                    continue;
                
                if (!r.Match(assetPath))
                    continue;
                
                rule = r;
                return true;
            }

            rule = null;
            return false;
        }
        
        public static void RemoveSpriteFromAtlas(string assetPath)
        {
            if (!LoadSettings(out AtlasGeneratorSettings generatorSettings, out AtlasGeneratorAtlasSettings atlasSettings))
            {
                return;
            }

            var dirty = TryGetMatchedRule(assetPath, generatorSettings, out var matchedRule);

            if (!dirty) return;
            
            AtlasGenerator.RemoveFromAtlas(matchedRule, null, assetPath);
            AssetDatabase.SaveAssets();
        }
    }
}