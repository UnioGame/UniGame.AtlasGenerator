using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public struct SpriteAssetFactoryInputData
    {
        public string Name;
        public string Path;

        public Texture2D Reference;

        public TextureImporterSettings ImporterSettings;
        public List<TextureImporterPlatformSettings> ImporterPlatformSettings;
    }
}