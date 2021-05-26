<h1>Unity SpriteAtlas Generator</h1>

A simple rule-based sprite atlas generator. The generator create atlased by matching the rule pattern.

Table of Contents

- [Install Package](#install-package)
  - [Install via Package Manager](#Install-via-package-manager)
  - [Install via Git URL](#install-via-git-url)
- [How to Use](#how-to-use)

## Install Package

### Install via Package Manager

Add to your project manifiest by path [%UnityProject%]/Packages/manifiest.json new Scope:

```json
{
  "scopedRegistries": [
    {
      "name": "UniGame",
      "url": "http://packages.unigame.pro:4873/",
      "scopes": [
        "com.unigame",
        "com.littlebigfun",
        "com.alelievr"
      ]
    },
    
    "__comment":"another scoped registers",
    
  ],
}

```

Now install via Package Manager

### Install via Git URL

Open *Packages/manifest.json* with your favorite text editor. Add the following line to the dependencies block.

```json
    {
        "dependencies": {
            "com.unigame.atlasgenerator": "https://github.com/UniGameTeam/UniGame.AtlasGenerator.git"
        }
    }
    
```

## How to Use


### Default Sprite Atlas Settings

![](https://github.com/UniGameTeam/UniGame.AtlasGenerator/blob/master/GitAssets/default_settings_1.png)

### Atlas Generator Rules

![](https://github.com/UniGameTeam/UniGame.AtlasGenerator/blob/master/GitAssets/settings_1.png)

- Sprites Root
  Root folder for scaning all defined rules

- `Match Type`
  - Wildcard, `*` matches any number of characters, `?` matches a single character.
  - Regex.
- Path, path or regexpr to files
- Path to Altlas, location where generated atlases will be saved, support regexp groups
- Apply Custom Settings, allow you override default atlas settings


### Rule Examples

| Type     | Example                                                                         |
|----------|---------------------------------------------------------------------------------|
| Wildcard | `Asset/Sprites/Icons/*`                                                         |
| Wildcard | `Asset/Sprites/Level??/*.asset`                                                 |
| Regex    | `^Assets/Models/.*\.fbx`                                                        |
| Regex    | `Assets/Weapons/(?<prefix>(?<category>[^/]+)/(.*/)*)(?<asset>.*_Data.*\.asset)` |


| Rule Path         | Rule Type | Results             | Comments |
|-------------------|-----------|---------------------|----------|
| `Assets/t1`       | Wildcard  | `t1` and `t1/1.txt` | bad      |
| `Assets/t1/*.txt` | Wildcard  | `t1/1.txt`          | good     |
| `^Assets/t1$`     | Regex     | `t1`                | good     |
| `^Assets/t1/.*`   | Regex     | `t1/1.txt`          | good     |
