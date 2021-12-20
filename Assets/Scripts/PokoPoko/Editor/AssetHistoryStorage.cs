#if UNITY_EDITOR
using UnityEngine;

using System;

namespace PKWork
{
    public class AssetHistoryStorage : ScriptableObject
    {
        public AssetHistoryData[] AssetHistory;

        public bool Validate(string assetName, int contentsHash)
        {
            foreach (var historyData in AssetHistory)
            {
                if (historyData.AssetName.Equals(assetName) && 
                    historyData.ContentsHash.Equals(contentsHash))
                {
                    return true;
                }
            }
            return false;
        }
    }
    [Serializable]
    public class AssetHistoryData
    {
        public string AssetName;
        public int ContentsHash;
    }
}
#endif