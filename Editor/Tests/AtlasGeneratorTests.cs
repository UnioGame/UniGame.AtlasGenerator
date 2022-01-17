using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

namespace UniModules.UniGame.AtlasGenerator.Editor.Tests
{
    using NUnit.Framework;
    using UnityEngine;
    
    public class AtlasGeneratorTests
    {
        [Test]
        public void PackToNewAtlasTest()
        {
            //info
            var firstSpritePath = "Assets/UniModules/UniGame.AtlasGenerator/Editor/Tests/Sprites/first.png";
            var secondSpritePath = "Assets/UniModules/UniGame.AtlasGenerator/Editor/Tests/Sprites/second.png";
            
            //validation
            var firstSprite = AssetDatabase.LoadAssetAtPath<Sprite>(firstSpritePath);
            var secondSprite = AssetDatabase.LoadAssetAtPath<Sprite>(secondSpritePath);

            if (firstSprite == null || secondSprite == null)
            {
                Assert.Fail("Sprites not found");
            }
            
            var firstRuleFound  = AtlasGeneratorCommands.TryGetMatchedRule(firstSpritePath, AtlasGeneratorSettings.Asset, out var firstRule);
            var secondRuleFound = AtlasGeneratorCommands.TryGetMatchedRule(secondSpritePath, AtlasGeneratorSettings.Asset, out var secondRule);

            if (!firstRuleFound || !secondRuleFound || firstRule != secondRule)
            {
                Assert.Fail("No matching rule for one of the sprites or different rules for two sprites");
            }
            
            var atlasPath = firstRule.ParseAtlasReplacement(firstSpritePath);
            atlasPath = firstRule.GetFullPathToAtlas(atlasPath);
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

            if (atlas != null)
            {
                Assert.Fail("Atlas already exists");
            }
            
            //action
            AtlasGeneratorPostprocessor.PackIntoAtlases(new[] {firstSpritePath, secondSpritePath}, new string[0], new string[0]);
            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            var packedSprites = atlas.GetPackables();
            var packSuccess = packedSprites.Contains(firstSprite.texture) && packedSprites.Contains(secondSprite.texture);
            AssetDatabase.DeleteAsset(atlasPath);
            AtlasGeneratorSettings.Asset.generatedAtlases.Remove(atlasPath);
            Assert.True(packSuccess);
        }
    }
}