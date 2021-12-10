#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using JsonFx.Json;
using UnityEditor;

public class AnimalMeshCapture : MonoBehaviour
{
    #region enum
    public enum Type
    {
        Animal,
        Boss,
    }
    #endregion
    
    #region public
    public static List<string> AnimalTexturePathList = new List<string>();

    public Type MeshType = Type.Animal;
    public bool Refresh = false;
    public GameObject MeshFBX;
    #endregion
    
    #region private
    private readonly List<AnimalMeshData> _animalMeshDataList = new List<AnimalMeshData>();
    private readonly List<AnimalMesh> _animaMeshList = new List<AnimalMesh>();
    #endregion

    #region method
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CAPTURE();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                LOCAL_TEXTURES();
                WRITE_TEXTURES();
            }
        }
    }
    
    public void CAPTURE()
    {
        switch (MeshType)
        {
            case Type.Animal:
                COMPOSE_ANIMAL();
                WRITE_ANIMAL();
                break;
            case Type.Boss:
                COMPOSE_BOSS();
                WRITE_BOSS();
                break;
        }
    }

    private void COMPOSE_ANIMAL()
    {
        foreach (var animalInfo in Global.instance._animalLevelData)
        {
            _animaMeshList.Clear();

            string prefabName = FIX_MESH_PREAFB_NAME(animalInfo._mesh);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "mesh", PrefabName = prefabName });
            }
            prefabName = FIX_MESH_PREAFB_NAME(animalInfo._mesh2);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "mesh2", PrefabName = prefabName });
            }
            prefabName = FIX_MESH_PREAFB_NAME(animalInfo._mesh3);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "mesh3", PrefabName = prefabName });
            }
            prefabName = FIX_MESH_PREAFB_NAME(animalInfo._masterMesh);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "master", PrefabName = prefabName });
            }
            
            _animalMeshDataList.Add(new AnimalMeshData { Type = (int)animalInfo._animalType, AnimalMeshs = _animaMeshList.ToArray() });
        }
    }

    private void WRITE_ANIMAL()
    {
        AnimalMeshMap animalMeshMap = new AnimalMeshMap { AnimalMeshDatas = _animalMeshDataList.ToArray() };
        File.WriteAllText(Application.dataPath + "/__ANIMAL_MESH_VIEWER/Resources/animalMeshMap.json", JsonWriter.Serialize(animalMeshMap));
    }

    private void COMPOSE_BOSS()
    {
        if (Refresh)
        {
            TextAsset asset = Resources.Load<TextAsset>("bossMeshMap");
            if (asset != null)
            {
                AnimalMeshMap animalMeshMap = JsonReader.Deserialize<AnimalMeshMap>(asset.text);
                if (animalMeshMap != null)
                {
                    _animalMeshDataList.Clear();
                    _animalMeshDataList.AddRange(animalMeshMap.AnimalMeshDatas);
                }
            }

            int credit = -1;
            
            foreach (var creditData in Global.instance._creditData)
            {
                _animaMeshList.Clear();
                
                string prefabName = FIX_MESH_PREAFB_NAME(creditData._mesh);
                if (!string.IsNullOrEmpty(prefabName))
                {
                    _animaMeshList.Add(new AnimalMesh { MeshKey = "mesh", PrefabName = prefabName });
                }
                
                bool isExist = _animaMeshList.Count > 0;
                if (isExist)
                {
                    _animalMeshDataList.Add(new AnimalMeshData { Type = credit--, AnimalMeshs = _animaMeshList.ToArray() });
                }
            }
            return;
        }

        int index = 0;
        
        foreach (var bossData in Global.instance._bossData)
        {
            _animaMeshList.Clear();

            string prefabName = FIX_MESH_PREAFB_NAME(bossData._mesh);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "mesh", PrefabName = prefabName });
            }
            prefabName = FIX_MESH_PREAFB_NAME(bossData._meshBoss);
            if (!string.IsNullOrEmpty(prefabName))
            {
                _animaMeshList.Add(new AnimalMesh { MeshKey = "meshBoss", PrefabName = prefabName });
            }

            bool isExist = _animaMeshList.Count > 0;
            if (isExist)
            {
                _animalMeshDataList.Add(new AnimalMeshData { Type = index++, AnimalMeshs = _animaMeshList.ToArray() });
            }
        }
    }
    private void WRITE_BOSS()
    {
        AnimalMeshMap animalMeshMap = new AnimalMeshMap { AnimalMeshDatas = _animalMeshDataList.ToArray() };
        File.WriteAllText(Application.dataPath + "/__ANIMAL_MESH_VIEWER/Resources/bossMeshMap.json", JsonWriter.Serialize(animalMeshMap));
    }
    
    private string FIX_MESH_PREAFB_NAME(Mesh mesh)
    {
        if (MeshFBX != null)
        {
            MeshFilter[] meshFilters = MeshFBX.GetComponentsInChildren<MeshFilter>();
            if (meshFilters != null)
            {
                foreach (var meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh == mesh)
                    {
                        return meshFilter.gameObject.name;
                    }
                }
            }
        }
        else
        {
            string name = mesh != null ? mesh.name : string.Empty;

            bool isExist = !string.IsNullOrEmpty(name);
            if (isExist)
            {
                string path = AssetDatabase.GetAssetPath(mesh);
                string[] depd = AssetDatabase.GetDependencies(path);

                foreach (var p in depd)
                {
                    if (p.Contains(".fbx") || p.Contains(".FBX"))
                    {
                        Object target = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                        return string.Format("{0}/{1}", target.name, name);
                    }
                }   
            }
        }
        return string.Empty;
    }

    private void LOCAL_TEXTURES()
    {
        TextAsset asset = Resources.Load<TextAsset>("animalTextureMap");
        if (asset != null)
        {
            AnimalTextureMap animalTextureMap = JsonReader.Deserialize<AnimalTextureMap>(asset.text);
            if (animalTextureMap != null)
            {
                AnimalTexturePathList.Clear();
                AnimalTexturePathList.AddRange(animalTextureMap.TexturePaths);
            }
        }

        string path = string.Format("{0}/Resources/Texture/Animal", Application.dataPath);

        DirectoryInfo dirInfo = new DirectoryInfo(path);

        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if (fileInfo.Name.Contains(".meta") || fileInfo.Name.Contains(".META"))
            {
                continue;
            }

            string fileName = fileInfo.Name.Replace(".png", "").Replace(".psd", "");

            bool isExist = AnimalTexturePathList.Contains(fileName);
            if (!isExist)
            {
                AnimalTexturePathList.Add(fileName);
            }
        }
    }
    private void WRITE_TEXTURES()
    {
        AnimalTextureMap animalTextureMap = new AnimalTextureMap { TexturePaths = AnimalTexturePathList.ToArray() };
        File.WriteAllText(Application.dataPath + "/__ANIMAL_MESH_VIEWER/Resources/animalTextureMap.json", JsonWriter.Serialize(animalTextureMap));
    }
    #endregion
}
#endif