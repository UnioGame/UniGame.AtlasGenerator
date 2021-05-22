using UnityEngine;

namespace UniModules.UniGame.AtlasGenerator.Editor
{
    public enum AtlasGeneratorRuleMatchType
    {
        [Tooltip("Simple wildcard.\n\"*\" matches any number of characters.\n\"?\" matches a single character.")]
        Wildcard = 0,

        [Tooltip("A regular expression pattern.")]
        Regex
    }
}