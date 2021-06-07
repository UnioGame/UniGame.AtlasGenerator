using System.IO;
using System.Linq;
using UniModules.UniGame.Core.EditorTools.Editor.Tools;
using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class AtlasGenerator
    {
        static void Log(LogType logType, string message)
        {
            var logMessage = $"[{nameof(AtlasGenerator)}] {message}";
            Debug.unityLogger.Log(logType, logMessage);
        }
        
        public static SpriteAtlas CreateOrUpdateAtlas(
            AtlasGeneratorSettings generatorSettings,
            AtlasGeneratorAtlasSettings atlasSettings,
            AtlasGeneratorRule rule,
            string assetPath)
        {
            // Set atlas
            SpriteAtlas atlas;
            var pathToAtlas = rule.ParseAtlasReplacement(assetPath);
            pathToAtlas = rule.GetFullPathToAtlas(pathToAtlas);
            
            var parts = pathToAtlas.SplitPath();
            var trimIndex = rule.trimLeadingFragments - 1;
            trimIndex = trimIndex < parts.Length ? trimIndex : parts.Length - 1;
            var name = string.Join("-",parts, trimIndex, parts.Length - trimIndex);
            pathToAtlas = Path.GetDirectoryName(pathToAtlas) + Path.DirectorySeparatorChar + name;
            
            var newAtlas = false;
            
            if (string.IsNullOrWhiteSpace(pathToAtlas))
            {
                Log(LogType.Warning, $"Asset {assetPath} wasn't packed because its rule has no atlas path");
                return null;
            }

            var appliedSettings = rule.applyCustomSettings ? rule.atlasSettings : atlasSettings.defaultAtlasSettings;

            if (!TryGetAtlas(pathToAtlas, out atlas))
            {
                atlas = CreateAtlas(pathToAtlas, appliedSettings);
                newAtlas = true;
            }

            // Set atlas settings from template if necessary
            if (!newAtlas && ((rule.applyCustomSettings && rule.atlasSettingsApplicationMode == AtlasSettingsApplicationMode.AlwaysOverwriteAtlasSettings) ||
                              atlasSettings.atlasSettingseApplicationMode == AtlasSettingsApplicationMode.AlwaysOverwriteAtlasSettings))
            {
                atlas.ApplySettings(appliedSettings);
            }

            var packedAsset = new[] { AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath) };
            var packedAssets = atlas.GetPackables();
            if (!packedAssets.Contains(packedAsset[0]))
            {
                atlas.Add(packedAsset);
            }

            return atlas;
        }

        public static SpriteAtlas CreateAtlas(string pathToAtlas, SpriteAtlasSettings atlasSettings)
        {
            var atlasNames = AtlasGeneratorSettings.Asset.generatedAtlases
                .Select(Path.GetFileName)
                .ToList();
            var createdAtlasName = Path.GetFileName(pathToAtlas);
            if (atlasNames.Contains(createdAtlasName))
            {
                Debug.LogError($"[{nameof(AtlasGenerator)}] Two generated atlases have the same name: {createdAtlasName}");
                AtlasGeneratorSettings.Asset.generatedAtlases.RemoveAll(atlasPath => atlasPath == pathToAtlas);
            }

            var atlasFactory = new SpriteAtlasFactory(atlasSettings);
            var atlas = atlasFactory.Create(pathToAtlas);
            AtlasGeneratorSettings.Asset.generatedAtlases.Add(pathToAtlas);
            EditorUtility.SetDirty(AtlasGeneratorSettings.Asset);
            return atlas;
        }

        public static bool RemoveFromAtlas(
            AtlasGeneratorRule rule,
            string movedFromAssetPath,
            string assetPath)
        {
            var pathToAtlas = rule.ParseAtlasReplacement(movedFromAssetPath == null ? assetPath : movedFromAssetPath);
            pathToAtlas = rule.GetFullPathToAtlas(pathToAtlas);
            if (!TryGetAtlas(pathToAtlas, out var atlas))
            {
                Log(LogType.Warning, $"Failed to find atlas {rule.pathToAtlas} when removing {assetPath} from it");
                return false;
            }
            atlas.RemoveSprite(assetPath);
            return DeleteEmptyAtlas(atlas);
        }

        public static bool DeleteEmptyAtlas(SpriteAtlas atlas)
        {
            var packables = atlas.GetPackables();
            if (packables.Length != 0) return false;
            var path = AssetDatabase.GetAssetPath(atlas);
            AtlasGeneratorSettings.Asset.generatedAtlases.RemoveAll(atlasPath => atlasPath == path);
            EditorUtility.SetDirty(AtlasGeneratorSettings.Asset);
            AssetDatabase.DeleteAsset(path);
            return true;
        }

        public static bool TryGetAtlas(string pathToAtlas, out SpriteAtlas atlas)
        {
            return ((atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(pathToAtlas)) != null);
        }
    }
}