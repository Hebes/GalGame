﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 画廊配置信息
/// </summary>
[System.Serializable]
class GalleryConfig
{
    public static GalleryConfig activeConfig;
    public const bool ENCRYPT = false;

    public static string filePath => $"{FilePaths.root}gallery.vng";

    public List<string> unlockedImages = new List<string>();

    public static void Load()
    {
        if (File.Exists(filePath))
        {
            activeConfig = FileSystem.Load<GalleryConfig>(filePath, encrypt: ENCRYPT);
        }
        else
        {
            activeConfig = new GalleryConfig();
        }
    }

    public static void Save() => FileSystem.Save(filePath, JsonUtility.ToJson(activeConfig), encrypt: ENCRYPT);

    public static void Erase()
    {
        activeConfig ??= new GalleryConfig();
        activeConfig.unlockedImages = new List<string>();
        Save();
    }

    public static void UnlockImage(string imageName)
    {
        if (activeConfig == null)
            Load();

        if (!activeConfig.unlockedImages.Contains(imageName))
        {
            activeConfig.unlockedImages.Add(imageName);

            Save();
        }
    }

    public static bool ImageIsUnlocked(string imageName)
    {
        if (activeConfig == null)
            Load();

        return activeConfig.unlockedImages.Contains(imageName);
    }
}