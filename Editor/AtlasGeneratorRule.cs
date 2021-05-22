﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
#if ODIN_INSPECTOR
#endif

    [Serializable]
    public class AtlasGeneratorRule : ISearchFilterable
    {
        private const string spriteAtlasExt = ".spriteatlas";

        [Tooltip("The assets in this path will be processed.")]
        public string path = string.Empty;

        [Tooltip("The path parsing method.")] public AtlasGeneratorRuleMatchType matchType;

        [Tooltip("The path to atlas in which the sprites will be added.")]
        public string pathToAtlas = string.Empty;

        public string CleanedPathToAtlas => pathToAtlas.Trim();

        private List<Func<string, bool>> _filters = new List<Func<string, bool>>();
        
        public IReadOnlyList<Func<string, bool>> Filters => _filters = _filters ?? new List<Func<string, bool>>()
        {
            ValidateAtlasName, ValidateRulePath
        };

#if ODIN_INSPECTOR
        [OnValueChanged("ToggleCustomSettings")]
#endif
        public bool applyCustomSettings = false;

        private void ToggleCustomSettings()
        {
            atlasSettings = applyCustomSettings ? new SpriteAtlasSettings() : null;
        }

        [Tooltip("Defines if the atlas settings will only be applied to new atlases, or will also overwrite existing atlas settings.")]
#if ODIN_INSPECTOR
        [ShowIf("applyCustomSettings")]
#endif
        public AtlasSettingsApplicationMode atlasSettingseApplicationMode =
            AtlasSettingsApplicationMode.ApplyOnAtlasCreationOnly;

        [Tooltip("Settings that will be applied to the atlas. Leave none to use the default settings.")]
        [HideLabel, ShowIf("applyCustomSettings")]
        public SpriteAtlasSettings atlasSettings = null;

        public bool Match(string assetPath)
        {
            path = path.Trim();
            if (string.IsNullOrEmpty(path))
                return false;
            
            if (matchType != AtlasGeneratorRuleMatchType.Wildcard)
                return matchType == AtlasGeneratorRuleMatchType.Regex &&
                       Regex.IsMatch(assetPath, path);
            
            if (!path.Contains("*") && !path.Contains("?"))
                return assetPath.StartsWith(path);
                
            var regex = "^" + Regex.Escape(path).Replace(@"\*", ".*").Replace(@"\?", ".");
            return Regex.IsMatch(assetPath, regex);

        }

        public string ParseAtlasReplacement(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(pathToAtlas))
                return null;
            // Parse path elements.
            var replacement = AtlasGeneratorRegex.ParsePath(assetPath, CleanedPathToAtlas);
            // Parse this.path regex.
            if (matchType != AtlasGeneratorRuleMatchType.Regex) return replacement;
            
            replacement = Regex.Replace(assetPath, path, replacement);

            return replacement;
        }

        public string GetFullPathToAtlas(string path)
        {
            return string.IsNullOrWhiteSpace(path) 
                ? path : Path.ChangeExtension(path, spriteAtlasExt);
        }

        public bool IsMatch(string searchString) => string.IsNullOrEmpty(searchString) || Filters.Any(x => x(searchString));
        
        public bool ValidateAtlasName(string filter) => pathToAtlas.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;

        public bool ValidateRulePath(string filter) => path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}