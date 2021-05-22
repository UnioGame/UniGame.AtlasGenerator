using UniModules.UniGame.AtlasGenerator.Editor.Abstract;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public class TextureFactory : ITextureFactory
    {
        public virtual Texture2D[] Create(Sprite data)
        {
            if (data != null) {
                var texture = new Texture2D((int)data.textureRect.width, (int)data.textureRect.height, TextureFormat.ARGB32, false);
                var pixels = data.texture.GetPixels((int) data.textureRect.x, (int) data.textureRect.y,
                    (int) data.textureRect.width, (int) data.textureRect.height);
                
                texture.SetPixels(pixels);
                texture.Apply();
                
                texture.name = data.name;

                return new[]{texture};
            }

            return null;
        }
    }
}