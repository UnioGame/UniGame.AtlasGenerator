using System.Linq;
using UniModules.UniGame.GraphicsTools.Editor.SplitTexture.Abstract;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    using GraphicsTools.Editor.SpriteAtlas;

    public class SplitTextureFactory : TextureFactory
    {
        private readonly ITextureSplitter _textureSplitter;
        private readonly int _maxSize;
        
        public SplitTextureFactory(ITextureSplitter textureSplitter, SpriteAtlasSettings atlasSettings)
        {
            _textureSplitter = textureSplitter;

            var platformSettings = atlasSettings.ImportSettings.GetTextureImporterPlatformSettings();
            
            _maxSize = platformSettings.maxTextureSize;
        }

        public override Texture2D[] Create(Sprite data)
        {
            if (data != null && _textureSplitter != null) {
                var texture = base.Create(data)[0];
                
                var rect = data.textureRect;
                if (rect.width > _maxSize || rect.height > _maxSize) {
                    return _textureSplitter.SplitTexture(texture, new Vector2Int(_maxSize, _maxSize)).ToArray();
                }

                return new[] {texture};
            }

            return null;
        }
    }
}