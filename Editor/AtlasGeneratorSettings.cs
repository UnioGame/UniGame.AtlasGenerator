using System.Collections.Generic;
using UniModules.UniGame.AtlasGenerator.Editor.Attributes.UnityAtlasGenerator.Helper;
using UniModules.UniGame.Core.Editor.EditorProcessors;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using System;
    using System.Collections;
    using UniCore.EditorTools.Editor.Utility;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [GeneratedAssetInfo("AtlasGenerator/Editor")]
    public class AtlasGeneratorSettings : GeneratedAsset<AtlasGeneratorSettings>
    {
        [Tooltip("Creates an atlas if the specified atlas doesn't exist.")]
        public bool allowAtlasCreation = false;

        [Tooltip("Root directory to look for sprites. Assets/ is used if left empty.")] 
        [SerializeField]
        public string spritesRoot = string.Empty;

        [Tooltip("Rules for managing imported assets.")]
        [SerializeField]
#if ODIN_INSPECTOR
        [ListDrawerSettings(HideAddButton = false, Expanded = false, DraggableItems = true, HideRemoveButton = false)]
#endif
#if ODIN_INSPECTOR_3
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
#endif
        public List<AtlasGeneratorRule> rules = new List<AtlasGeneratorRule>();

        public List<string> generatedAtlases = new List<string>();

        [ButtonMethod]
        public void Save()
        {
            AssetDatabase.SaveAssets();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Validate()
        {
            OnValidate();
        }
        
        private void OnValidate()
        {
            generatedAtlases.Sort(Comparer<string>.Default);
            this.MarkDirty();
        }
    }
}