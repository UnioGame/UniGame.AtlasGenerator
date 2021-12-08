namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using Abstract;
    using GraphicsTools.Editor.SpriteAtlas;
    using UniModules.Editor;
    using UnityEditor;
    using UnityEngine;
    
    public class SpriteAssetFactory : ISpriteAssetFactory
    {
        private readonly SpriteAtlasSettings _settings;
        private TextureImporterSettings _importerSettings;
        private List<TextureImporterPlatformSettings> _importerPlatformSettings = new List<TextureImporterPlatformSettings>();

        public SpriteAssetFactory(SpriteAtlasSettings settings)
        {
            _settings = settings;
        }

        public Sprite Create(SpriteAssetFactoryInputData data)
        {
            if (!string.IsNullOrEmpty(data.Name) && !string.IsNullOrEmpty(data.Path) && data.Reference != null) {
                
                EditorFileUtils.CreateDirectories(data.Path);
                
                var directoryInfo = new DirectoryInfo(data.Path);
                var fullPath = Path.Combine(directoryInfo.FullName, $"{data.Name}.png");
                var relativePath = Path.Combine(data.Path, $"{data.Name}.png");

                var bytes = data.Reference.EncodeToPNG();
                File.WriteAllBytes(fullPath, bytes);

                AssetDatabase.Refresh();

                _importerSettings = data.ImporterSettings;
                _importerPlatformSettings = data.ImporterPlatformSettings;

                var importer = AssetImporter.GetAtPath(relativePath);
                if (importer != null && importer is TextureImporter textureImporter)
                {
                    if (_importerSettings != null)
                    {
                        textureImporter.textureType         = _importerSettings.textureType;
                        textureImporter.spritePixelsPerUnit = _importerSettings.spritePixelsPerUnit;
                        textureImporter.spritePivot         = _importerSettings.spritePivot;
                        textureImporter.sRGBTexture         = _importerSettings.sRGBTexture;
                        textureImporter.alphaSource         = _importerSettings.alphaSource;
                        textureImporter.alphaIsTransparency = _importerSettings.alphaIsTransparency;
                        textureImporter.isReadable          = _importerSettings.readable;
                        textureImporter.mipmapEnabled       = _importerSettings.mipmapEnabled;
                        textureImporter.wrapMode            = _importerSettings.wrapMode;
                        textureImporter.filterMode          = _importerSettings.filterMode;
                        textureImporter.anisoLevel          = _importerSettings.aniso;

                        var importerSettings = new TextureImporterSettings();
                        textureImporter.ReadTextureSettings(importerSettings);

                        importerSettings.spriteMode                         = _importerSettings.spriteMode;
                        importerSettings.spriteMeshType                     = _importerSettings.spriteMeshType;
                        importerSettings.spriteExtrude                      = _importerSettings.spriteExtrude;
                        importerSettings.spriteAlignment                    = _importerSettings.spriteAlignment;
                        importerSettings.spriteGenerateFallbackPhysicsShape = _importerSettings.spriteGenerateFallbackPhysicsShape;

                        textureImporter.SetTextureSettings(importerSettings);

                        foreach (var platformSettings in _importerPlatformSettings)
                        {
                            var existingSettings = textureImporter.GetPlatformTextureSettings(platformSettings.name);

                            existingSettings.overridden                     = platformSettings.overridden;
                            existingSettings.maxTextureSize                 = platformSettings.maxTextureSize;
                            existingSettings.resizeAlgorithm                = platformSettings.resizeAlgorithm;
                            existingSettings.format                         = platformSettings.format;
                            existingSettings.textureCompression             = platformSettings.textureCompression;
                            existingSettings.crunchedCompression            = platformSettings.crunchedCompression;
                            existingSettings.compressionQuality             = platformSettings.compressionQuality;
                            existingSettings.allowsAlphaSplitting           = platformSettings.allowsAlphaSplitting;
                            existingSettings.androidETC2FallbackOverride    = platformSettings.androidETC2FallbackOverride;

                            textureImporter.SetPlatformTextureSettings(existingSettings);
                        }
                    }
                    else
                    {
                        textureImporter.SetTextureSettings(_settings.ImportSettings.GetTextureImporterSettings());
                        
                        textureImporter.SetPlatformTextureSettings(_settings.ImportSettings.GetTextureImporterPlatformSettings(BuildTargetGroup.Standalone));
                        textureImporter.SetPlatformTextureSettings(_settings.ImportSettings.GetTextureImporterPlatformSettings(BuildTargetGroup.Android));
                    }

                    textureImporter.SaveAndReimport();
                }

                AssetDatabase.Refresh();
                AssetDatabaseHelper.TryGetAsset<Sprite>(relativePath, out var sprite);

                return sprite;
            }

            return null;
        }
    }
}