using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IOTools 
{
    public static string GetDirectoryPath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || Directory.Exists(filePath))
        {
            return filePath;
        }
        int lastIndex = filePath.LastIndexOf('/');
        return lastIndex < 0 ? "" : filePath.Substring(0, lastIndex);
    }

    public static string GetExtensionByBath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return "";
        }
        int lastIndex = filePath.LastIndexOf('.');
        return lastIndex < 0 ? null : filePath.Substring(lastIndex);
    }
}
