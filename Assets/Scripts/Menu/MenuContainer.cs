using UnityEngine;

using System.Collections.Generic;
using System;

public class MenuContainer : MonoBehaviour
{
    #region CategoryData
    [Serializable]
    public class CategoryData
    {
        #region public
        public string category;
        public List<GameObject> PrefabList;
        #endregion
    }
    #endregion
    
    #region public
    public List<CategoryData> CategoryDataList = new List<CategoryData>();
    #endregion
    
    #region private
    private readonly Dictionary<string, GameObject> _prefabNameMap = new Dictionary<string, GameObject>();
    #endregion

    #region method
    public void Init()
    {
        foreach (var data in CategoryDataList)
        {
            foreach (var prefab in data.PrefabList)
            {
                RecordPrefabName(prefab);
            }
        }
    }
    
    private void RecordPrefabName(GameObject prefab)
    {
        bool exist = _prefabNameMap.ContainsKey(prefab.name);
        if (exist)
        {
            return;
        }
        
        _prefabNameMap.Add(prefab.name, prefab);
    }

    public GameObject GetPrefab(string prefabName)
    {
        bool exist = _prefabNameMap.ContainsKey(prefabName);
        return exist ? _prefabNameMap[prefabName] : null;
    }
    #endregion
}