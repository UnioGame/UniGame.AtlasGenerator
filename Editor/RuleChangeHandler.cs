using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
using UniModules.UniGame.Core.EditorTools.Editor.Tools;
using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class RuleChangeHandler
    {
        private const string spriteAtlasExtension = "*.spriteatlas";

        public static void ApplyRules(AtlasGeneratorSettings settings)
        {
            var atlasSettings = AtlasGeneratorAtlasSettings.Asset;
            if (atlasSettings == null)
            {
                Debug.LogError($"[{nameof(RuleChangeHandler)}] Atlas settings file not found.");
                return;
            }
            var defaultSettings = atlasSettings.defaultAtlasSettings;

            var dirty = false;

            var packedSprites = new HashSet<string>();
            var existingAtlases = (settings.generatedAtlases)
                .ToList()
                .Select(path  => LoadAtlas(path, settings))
                .Where(atlas => atlas != null)
                .ToList();
            var spritesRoot = string.IsNullOrEmpty(settings.spritesRoot.Trim()) ? "Assets" : settings.spritesRoot.TrimEndPath();
            var existingSprites = AssetEditorTools.GetAssetsPaths<Texture2D>(new string[] { spritesRoot });

            foreach (var atlas in existingAtlases)
            {
                var packables = atlas.GetPackables();
                var spritesToRemove = new List<string>();
                var currentPathToAtlas = AssetDatabase.GetAssetPath(atlas);
                foreach (var packable in packables)
                {
                    if (!(packable is Texture2D))
                    {
                        continue;
                    }

                    var spritePath = AssetDatabase.GetAssetPath(packable);
                    spritesToRemove.Add(spritePath);
                    foreach (var rule in settings.rules)
                    {
                        if (!rule.Match(spritePath))
                        {
                            continue;
                        }

                        var pathToAtlas = rule.ParseAtlasReplacement(spritePath);
                        pathToAtlas = rule.GetFullPathToAtlas(pathToAtlas);
                        if (!pathToAtlas.Equals(currentPathToAtlas, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        spritesToRemove.Remove(spritePath);
                        packedSprites.Add(spritePath);

                        var appliedSettings = rule.applyCustomSettings ? rule.atlasSettings : defaultSettings;
                        if ((rule.applyCustomSettings &&          rule.atlasSettingseApplicationMode == AtlasSettingsApplicationMode.AlwaysOverwriteAtlasSettings) ||
                           (!rule.applyCustomSettings && atlasSettings.atlasSettingseApplicationMode == AtlasSettingsApplicationMode.AlwaysOverwriteAtlasSettings))
                        {
                            if (!atlas.CheckSettings(appliedSettings))
                            {
                                atlas.ApplySettings(appliedSettings);
                                dirty = true;
                            }
                        }

                        break;
                    }
                }
                atlas.RemoveSprites(spritesToRemove.ToArray());
                dirty |= AtlasGenerator.DeleteEmptyAtlas(atlas);
            }

            if (dirty)
            {
                AssetDatabase.SaveAssets();
            }

            var spritesToPack = existingSprites.Except(packedSprites).ToArray();
            AtlasGeneratorPostprocessor.PackIntoAtlases(spritesToPack, new string[] { }, new string[] { });

            CheckForCollisions(settings.generatedAtlases);
        }

        private static SpriteAtlas LoadAtlas(string path, AtlasGeneratorSettings settings)
        {
            var asset = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (asset == null)
            {
                Debug.LogError($"[{nameof(RuleChangeHandler)}] Generated atlas at {path} not found.");
                settings.generatedAtlases.Remove(path);
            }
            return asset;
        }

        private static void CheckForCollisions(List<string> generatedAtlases)
        {
            var fixedGeneratedAtlases = generatedAtlases
                .Select(path => path.FixUnityPath())
                .ToList();

            var existingAtlases = Directory
                .GetFiles(Application.dataPath, spriteAtlasExtension, SearchOption.AllDirectories)
                .Select(path => EditorFileUtils.ToProjectPath(path))
                .Except(fixedGeneratedAtlases)
                .ToList();

            var existingNames = existingAtlases
                .Select(path => Path.GetFileName(path))
                .ToList();

            var generatedNames = generatedAtlases
                .Select(path => Path.GetFileName(path))
                .ToList();

            foreach (var collision in existingNames.Intersect(generatedNames))
            {
                Debug.LogError($"[{nameof(RuleChangeHandler)}] Generated atlas was assigned the name that's already taken: {collision}");
            }
        }
    }
}