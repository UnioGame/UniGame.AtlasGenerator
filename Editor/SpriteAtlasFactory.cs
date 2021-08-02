using UniModules.Editor;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using UniModules.UniGame.AtlasGenerator.Editor.Abstract;
    using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas;
    using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas.Abstract;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine.U2D;
    
    public class SpriteAtlasFactory : ISpriteAtlasFactory
    {
        private readonly ISpriteAtlasSettings _atlasSettings;
        
        public SpriteAtlasFactory(ISpriteAtlasSettings atlasSettings)
        {
            _atlasSettings = atlasSettings;
        }

        public SpriteAtlas Create(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            
            var atlas = new SpriteAtlas();

            var atlasTextureSettings = new SpriteAtlasTextureSettings();
            atlasTextureSettings.generateMipMaps = _atlasSettings.GenerateMipMaps;
            atlasTextureSettings.filterMode      = _atlasSettings.FilterMode;
            atlasTextureSettings.readable        = _atlasSettings.ReadWriteEnabled;
            atlasTextureSettings.sRGB            = _atlasSettings.SRgb;

            var atlasPackingSettings = new SpriteAtlasPackingSettings();
            atlasPackingSettings.padding            = _atlasSettings.Padding;
            atlasPackingSettings.enableRotation     = _atlasSettings.AllowRotation;
            atlasPackingSettings.enableTightPacking = _atlasSettings.TightPacking;

            var platformSettings = new TextureImporterPlatformSettings();
            platformSettings.textureCompression  = _atlasSettings.Compression;
            platformSettings.maxTextureSize      = _atlasSettings.MaxTextureSize;
            platformSettings.format              = (TextureImporterFormat) _atlasSettings.Format;
            platformSettings.crunchedCompression = _atlasSettings.UseCrunchCompression;
            platformSettings.compressionQuality  = _atlasSettings.CompressionQuality;
                
            atlas.SetTextureSettings(atlasTextureSettings);
            atlas.SetPackingSettings(atlasPackingSettings);
            atlas.SetPlatformSettings(platformSettings);
            
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