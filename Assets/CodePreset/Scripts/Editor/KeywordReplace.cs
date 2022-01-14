#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System.IO;
using System;
using System.Globalization;

public class KeywordReplace : AssetModificationProcessor
{
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");

        int index = path.LastIndexOf(".");
        if (index < 0)
            return;

        string file = path.Substring(index);
        if (file != ".cs")
            return;

        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;

        if (!File.Exists(path))
            return;

        string fileContents = File.ReadAllText(path);
        
        fileContents = fileContents.Replace("#DATE#", GetDate());
        fileContents = fileContents.Replace("#AUTHOR#", Environment.UserName);
        
        File.WriteAllText(path, fileContents);
        
        AssetDatabase.Refresh();
    }

    private static string GetDate()
    {
        return DateTime.Now.ToString("yyyy년 MM월 dd일", CultureInfo.CreateSpecificCulture("ko-KR"));
    }
}
#endif