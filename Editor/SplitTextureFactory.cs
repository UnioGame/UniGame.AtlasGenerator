using System.Linq;
using UniModules.UniGame.GraphicsTools.Editor.SplitTexture.Abstract;
using UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas.Abstract;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class SplitTextureFactory : TextureFactory
    {
        private readonly ITextureSplitter _textureSplitter;
        private readonly ISpriteAtlasSettings _atlasSettings;
        
        public SplitTextureFactory(ITextureSplitter textureSplitter, ISpriteAtlasSettings atlasSettings)
        {
            _textureSplitter = textureSplitter;
            _atlasSettings = atlasSettings;
        }

        public override Texture2D[] Create(Sprite data)
        {
            if (data != null && _textureSplitter != null) {
                var texture = base.Create(data)[0];
                
                var rect = data.textureRect;
                if (rect.width > _atlasSettings.MaxTextureSize || rect.height > _atlasSettings.MaxTextureSize) {
                    return _textureSplitter.SplitTexture(texture, new Vector2Int(_atlasSettings.MaxTextureSize, _atlasSettings.MaxTextureSize)).ToArray();
                }

                return new[] {texture};
            }

            return null;
        }
    }
}