using UniModules.Editor;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using Abstract;
    using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine.U2D;
    
    public class SpriteAtlasFactory : ISpriteAtlasFactory
    {
        private readonly SpriteAtlasSettings _atlasSettings;
        
        public SpriteAtlasFactory(SpriteAtlasSettings atlasSettings)
        {
            _atlasSettings = atlasSettings;
        }

        public SpriteAtlas Create(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            
            var atlas = new SpriteAtlas();

            var textureSettings = _atlasSettings.ImportSettings.GetTextureImporterSettings();
            
            var atlasTextureSettings = new SpriteAtlasTextureSettings();
            atlasTextureSettings.generateMipMaps = textureSettings.mipmapEnabled;
            atlasTextureSettings.filterMode      = textureSettings.filterMode;
            atlasTextureSettings.readable        = textureSettings.readable;
            atlasTextureSettings.sRGB            = textureSettings.sRGBTexture;

            var atlasPackingSettings = new SpriteAtlasPackingSettings();
            atlasPackingSettings.padding            = _atlasSettings.Padding;
            atlasPackingSettings.enableRotation     = _atlasSettings.AllowRotation;
            atlasPackingSettings.enableTightPacking = _atlasSettings.TightPacking;

            var platformSettingsDefault = _atlasSettings.ImportSettings.GetTextureImporterPlatformSettings();
            var platformSettingsStandalone =
                _atlasSettings.ImportSettings.GetTextureImporterPlatformSettings(BuildTargetGroup.Standalone);
            var platformSettingsAndroid =
                _atlasSettings.ImportSettings.GetTextureImporterPlatformSettings(BuildTargetGroup.Android);

            atlas.SetTextureSettings(atlasTextureSettings);
            atlas.SetPackingSettings(atlasPackingSettings);
            
            atlas.SetPlatformSettings(platformSettingsDefault);
            atlas.SetPlatformSettings(platformSettingsStandalone);
            atlas.SetPlatformSettings(platformSettingsAndroid);

            atlas.SetIncludeInBuild(_atlasSettings.IncludeInBuild);
            atlas.SetIsVariant(_atlasSettings.Type == SpriteAtlasType.Variant);
            
            EditorFileUtils.CreateDirectories(fullPath);
            AssetDatabase.CreateAsset(atlas, fullPath);
            
            AssetDatabase.SaveAssets();

            AssetDatabase.WriteImportSettingsIfDirty(fullPath);

            return atlas;
        }
    }
}