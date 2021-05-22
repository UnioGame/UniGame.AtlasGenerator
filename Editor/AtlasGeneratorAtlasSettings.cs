using Sirenix.OdinInspector;
using UniModules.UniGame.Core.Editor.EditorProcessors;
using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
#if ODIN_INSPECTOR
#endif

    [GeneratedAssetInfo(AtlasGeneratorAtlasConstants.DefaultSettingsPath)]
    public class AtlasGeneratorAtlasSettings : GeneratedAsset<AtlasGeneratorAtlasSettings>
    {
        /// <summary>
        /// Controls wether atlas settings will be applied only on atlas creation, or also to already created atlases.
        /// </summary>
        [Tooltip(
            "Defines if the atlas settings will only be applied to new atlases, or will also overwrite existing atlas settings.")]
        public AtlasSettingsApplicationMode atlasSettingseApplicationMode =
            AtlasSettingsApplicationMode.ApplyOnAtlasCreationOnly;

        [Tooltip("Settings that will be applied to generated atlases.")]
#if ODIN_INSPECTOR
        [HideLabel]
#endif
        public SpriteAtlasSettings defaultAtlasSettings = new SpriteAtlasSettings();
    }
}