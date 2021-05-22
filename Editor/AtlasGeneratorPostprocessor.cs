// TODO: вынести зависимости от хога

using System.IO;
using UniModules.UniGame.Core.Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class AtlasGeneratorPostprocessor : AssetPostprocessor
    {
        const string psbExtension = ".psb";

        static void Log(LogType logType, string message)
        {
            var logMessage = $"[{nameof(AtlasGeneratorPostprocessor)}] {message}";
            Debug.unityLogger.Log(logType, logMessage);
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UniEditorProfiler.LogTime(nameof(AtlasGenerator), () => PackIntoAtlases(importedAssets, movedAssets, movedFromAssetPaths));
        }

        public static void PackIntoAtlases(string[] importedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        { 
            if (!LoadSettings(out AtlasGeneratorSettings generatorSettings, out AtlasGeneratorAtlasSettings atlasSettings))
            {
                return;
            }

            var dirty = false;

            // Apply generation rules.
            foreach (var importedAsset in importedAssets)
            {
                dirty |= ProcessAsset(importedAsset, null, generatorSettings, atlasSettings);
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                var movedAsset = movedAssets[i];
                var movedFromAssetPath = movedFromAssetPaths[i];
                dirty |= ProcessAsset(movedAsset, movedFromAssetPath, generatorSettings, atlasSettings);
            }

            if (dirty)
            {
                AssetDatabase.SaveAssets();
            }
        }

        static bool LoadSettings(out AtlasGeneratorSettings generatorSettings, out AtlasGeneratorAtlasSettings atlasSettings) {
            generatorSettings = AtlasGeneratorSettings.Asset;
            atlasSettings = null;
            if (generatorSettings == null)
            {
                Log(LogType.Error ,"Generator settings file not found");
                return false;
            }
            atlasSettings = AtlasGeneratorAtlasSettings.Asset;
            if (atlasSettings == null)
            {
                Log(LogType.Error, "Atlas settings file not found.");
                return false;
            }
            return true;
        }

        static bool ProcessAsset(string pathToAsset, string oldPathToAsset, AtlasGeneratorSettings generatorSettings, AtlasGeneratorAtlasSettings atlasSettings)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(pathToAsset) == typeof(Texture2D) && Path.GetExtension(pathToAsset.ToLower()) != psbExtension)
            {
                return ApplyGenerationRule(pathToAsset, oldPathToAsset, generatorSettings, atlasSettings);
            }
            return false;
        }

        static bool ApplyGenerationRule(
           string assetPath,
           string movedFromAssetPath,
           AtlasGeneratorSettings generatorSettings,
           AtlasGeneratorAtlasSettings atlasSettings)
        {
            var dirty = false;
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

        static bool TryGetMatchedRule(
            string assetPath,
            AtlasGeneratorSettings importSettings,
            out AtlasGeneratorRule rule)
        {
            if (importSettings.rules == null || importSettings.rules.Count == 0)
            {
                rule = null;
                return false;
            }

            foreach (var r in importSettings.rules)
            {
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

            if (dirty)
            {
                AtlasGenerator.RemoveFromAtlas(matchedRule, null, assetPath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}