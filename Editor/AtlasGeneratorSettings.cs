using System.Collections.Generic;
using UniModules.UniGame.AtlasGenerator.Editor.Attributes.UnityAtlasGenerator.Helper;
using UniModules.UniGame.Core.Editor.EditorProcessors;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using UniModules.Editor;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [GeneratedAssetInfo("AtlasGenerator/Editor")]
    public class AtlasGeneratorSettings : GeneratedAsset<AtlasGeneratorSettings>
    {
        public bool allowPostProcessing = false;

        [Tooltip("Is Empty generated atlas should be deleted")]
        public bool deleteEmptyAtlas = false;

        [Tooltip("Creates an atlas if the specified atlas doesn't exist.")]
        public bool allowAtlasCreation = false;

        [Tooltip("Root directory to look for sprites. Assets/ is used if left empty.")] [SerializeField]
        public string spritesRoot = string.Empty;

        [Tooltip("Rules for managing imported assets.")]
        [SerializeField]
#if ODIN_INSPECTOR
        [ListDrawerSettings(HideAddButton = false, Expanded = false, DraggableItems = true, HideRemoveButton = false)]
#endif

#if ODIN_INSPECTOR_3
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface, Recursive = true)]
#endif
        public List<AtlasGeneratorRule> rules = new List<AtlasGeneratorRule>();

        [Space]
#if ODIN_INSPECTOR_3
        [Searchable]
#endif
        public List<string> generatedAtlases = new List<string>();

        [ButtonMethod]
        public void Save() => AssetDatabase.SaveAssets();

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Validate()
        {
            generatedAtlases.RemoveAll(x => string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(x)));
            generatedAtlases.Sort(Comparer<string>.Default);
            this.MarkDirty();
        }
    }
}