#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;

namespace PKWork
{
    public class PokoResourceWork
    {
        
        #region 텍스쳐설정 값 추출 (scriptableObject)
        [MenuItem("Assets/Work/선택된 리소스(단일)의 텍스쳐설정 값 추출", false, 1)]
        static void ExportTextureImporterObject()
        {
            Texture texture = Selection.activeObject as Texture;

            bool isExist = texture != null;
            if (!isExist)
                return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null)
            {
                TextureImporterSettings settings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(settings);
                
                TextureImporterPlatformSettings platformSettings = textureImporter.GetDefaultPlatformTextureSettings();

                TextureImporterObject obj = ScriptableObject.CreateInstance<TextureImporterObject>();
                obj.Settings = settings;
                obj.PlatformSettings = platformSettings;
                
                string path = "Assets/0. __WORK_EDITOR/Output/new(rename).asset";

                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.SaveAssets();

                Selection.activeObject = obj;
            }
        }
        [MenuItem("Assets/Work/선택된 리소스(단일)에 추출된 텍스쳐설정 값 적용(for test)", false, 1)]
        static void ApplyTextureSettingsMenu()
        {
            Texture texture = Selection.activeObject as Texture;

            bool isExist = texture != null;
            if (!isExist)
                return;

            ApplyTextureSettings(new Texture[] { texture }, "Assets/0. __WORK_EDITOR/Output/new(rename).asset");
        }
        public static void ApplyTextureSettings(Texture[] textures, string path)
        {
            TextureImporterObject importerObject = AssetDatabase.LoadAssetAtPath<TextureImporterObject>(path);
            if (importerObject != null && importerObject.Settings != null && importerObject.PlatformSettings != null)
            {
                foreach (var texture in textures)
                {
                    ApplyTextureSettings(texture, importerObject);
                }
            }
        }
        private static void ApplyTextureSettings(Texture texture, TextureImporterObject importerObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.SetTextureSettings(importerObject.Settings);
                textureImporter.SetPlatformTextureSettings(importerObject.PlatformSettings);
                
                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh();
            }
        }
        #endregion
        
        #region 텍스쳐 변경점 저장 (scriptableObject)
        public static void SaveTextureContentsHash(string pathOfResources, string pathToBeSaved)
        {
            List<AssetHistoryData> list = new List<AssetHistoryData>();
            
            List<UnityEngine.Object> assets = LoadAllAssetsAtPath(pathOfResources);
            if (assets != null)
            {
                foreach (var asset in assets)
                {
                    AssetHistoryData data = new AssetHistoryData();
                    data.AssetName = asset.name;

                    Texture texture = asset as Texture;
                    data.ContentsHash = texture.imageContentsHash.GetHashCode();
                    
                    list.Add(data);
                }
            }
            
            AssetHistoryStorage obj = ScriptableObject.CreateInstance<AssetHistoryStorage>();
            obj.AssetHistory = list.ToArray();
                
            AssetDatabase.CreateAsset(obj, pathToBeSaved);
            AssetDatabase.SaveAssets();
        }
        public static void SaveTextureContentsHash(Dictionary<Object, string> resources, string pathToBeSaved)
        {
            List<AssetHistoryData> list = new List<AssetHistoryData>();
            
            foreach (var asset in resources.Keys)
            {
                Texture texture = asset as Texture;

                AssetHistoryData data = new AssetHistoryData();
                data.AssetName = resources[asset];
                data.ContentsHash = texture.imageContentsHash.GetHashCode();
                    
                list.Add(data);
            }
            
            AssetHistoryStorage obj = ScriptableObject.CreateInstance<AssetHistoryStorage>();
            obj.AssetHistory = list.ToArray();
                
            AssetDatabase.CreateAsset(obj, pathToBeSaved);
            AssetDatabase.SaveAssets();
        }
        public static List<UnityEngine.Object> LoadAllAssetsAtPath (string path) 
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            
            if (Directory.Exists(path)) 
            {
                string[] assets = Directory.GetFiles(path);
                
                foreach (string assetPath in assets)
                {
                    bool isTargetFile = assetPath.ToLower().Contains(".meta") == false;
                    if (isTargetFile)
                    {
                        objects.Add(AssetDatabase.LoadMainAssetAtPath(assetPath));
                    }
                }
            }
            return objects;
        }
        #endregion
        
        #region 아틀라스 갱신
        public static void UpdateAtlas(UIAtlas atlas, List<Texture> textures, bool keepSprites)
        {
            List<UIAtlasMaker.SpriteEntry> sprites = UIAtlasMaker.CreateSprites(textures);

            if (keepSprites)
            {
                UIAtlasMaker.ExtractSprites(atlas, sprites);
            }

            UIAtlasMaker.UpdateAtlas(atlas, sprites);
        }
        #endregion
        
    }
}
#endif