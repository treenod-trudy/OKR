using System.Collections.Generic;
using System.IO;

using Pokopang.Utils;

using UnityEngine;
using UnityEditor;

public class AnimalTextureFinder : EditorWindow
{
    private AnimalTextureMap _animalTextureMap;

    private readonly List<Object> _findedTextureList = new List<Object>();
    
    [MenuItem("Treenod/AnimalTextureFinder")]
    static void Init()
    {
        AnimalTextureFinder window = (AnimalTextureFinder)EditorWindow.GetWindow(typeof(AnimalTextureFinder));
        window.Show();
    }

    void OnGUI()
    {
        DrawDataLoadMenu();
        DrawTextureFileMove();
        DrawTexturePathMenu();
    }

    private void DrawDataLoadMenu()
    {
        GUI.backgroundColor = Color.green;
        {
            if (GUILayout.Button("DATA LOAD"))
            {
                TextAsset textAsset = Resources.Load<TextAsset>("animalTextureMap");
                if (textAsset != null)
                {
                    _animalTextureMap = JsonReader.Deserialize<AnimalTextureMap>(textAsset.text);
                    
                    FindTextures();
                }
            }
        }
        GUI.backgroundColor = Color.white;
    }
    private void FindTextures()
    {
        _findedTextureList.Clear();
                    
        foreach (var texturePath in _animalTextureMap.TexturePaths)
        {
            Object texture = Resources.Load(texturePath);
            if (texture != null)
            {
                _findedTextureList.Add(texture);
            }
        }
    }

    private void DrawTextureFileMove()
    {
        if (_findedTextureList.Count > 0)
        {
            GUI.backgroundColor = Color.red;
            {
                if (GUILayout.Button("FILE MOVE"))
                {
                    foreach (var texture in _findedTextureList)
                    {
                        string filePath = AssetDatabase.GetAssetPath(texture);
                        
                        string[] split = filePath.Split('/');
                        string fileName = split[split.Length - 1];
                        string movePath = string.Concat("Assets/__CONVERTOR/Resources/Texture/Animal/", fileName);
                        
                        Debug.Log(AssetDatabase.MoveAsset(filePath, movePath));
                        AssetDatabase.Refresh();
                    }
                }
            }
            GUI.backgroundColor = Color.white;   
        }
    }

    private Vector2 _scroll;
    private void DrawTexturePathMenu()
    {
        if (_animalTextureMap != null)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            {
                foreach (var texture in _findedTextureList)
                {
                    EditorGUILayout.ObjectField(texture.name, texture, typeof(Object));
                }
            }
            GUILayout.EndScrollView();
        }
    }
}