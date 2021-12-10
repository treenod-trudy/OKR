#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AnimalMeshViewer : MonoBehaviour
{
    #region const
    private const int PAGE_UNIT_COUNT = 10;
    #endregion
    
    #region public
    public UILabel ViewerLabel;
    public UIGrid GroupGrid;
    public GameObject GroupPrefab;
    #endregion
    
    #region private
    private readonly List<AnimalInfo> _animalInfoList = new List<AnimalInfo>();
    private readonly List<AnimalMeshViewGroup> _viewGroupList = new List<AnimalMeshViewGroup>();
    #endregion
    
    #region properties
    public int ViewPage { get; private set; }
    public bool IsEvo { get; private set; }
    #endregion

    #region method
    private void Start()
    {
        DataInit();
        PageInit();
    }
    private void DataInit()
    {
        _animalInfoList.Clear();
        _animalInfoList.AddRange(Global.instance._animalLevelData);
    }

    private void Clear()
    {
        while (_viewGroupList.Count > 0)
        {
            _viewGroupList[0].Hide();
            _viewGroupList.RemoveAt(0);
        }
        _viewGroupList.Clear();
    }

    private void Set()
    {
        Clear();
        Compose();
    }
    private void Compose()
    {
        ViewerLabel.text = IsEvo ? "IsEvo : O" : "IsEvo : X";

        int unitCount = Mathf.Min(_animalInfoList.Count, ViewPage * PAGE_UNIT_COUNT);
        
        for (int i = (ViewPage - 1) * PAGE_UNIT_COUNT; i < unitCount; i++)
        {
            AnimalInfo animalInfo = _animalInfoList[i];
            SetAnimalMeshViewGroups(animalInfo);
        }

        GroupGrid.repositionNow = true;
    }

    private void SetAnimalMeshViewGroups(AnimalInfo template)
    {
        AnimalInfo[] revisedArray = new AnimalInfo[3];
        revisedArray[0] = template.Clone() as AnimalInfo;
        revisedArray[0]._upgrade = 1;
        revisedArray[0]._isEvo = IsEvo;

        revisedArray[1] = template.Clone() as AnimalInfo;
        revisedArray[1]._upgrade = 9;
        revisedArray[1]._isEvo = IsEvo;

        revisedArray[2] = template.Clone() as AnimalInfo;
        revisedArray[2]._upgrade = 10;
        revisedArray[2]._isEvo = IsEvo;
        
        GameObject go = NGUITools.AddChild(GroupGrid.gameObject, GroupPrefab);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;

        AnimalMeshViewGroup viewGroup = go.GetComponent<AnimalMeshViewGroup>();
        if (viewGroup != null)
        {
            viewGroup.Show(template, revisedArray);
            
            _viewGroupList.Add(viewGroup);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            PageUp();
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            PageDown();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PageInit();
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            PageLast();
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            ApplyEvo();
        }
    }
    private void PageUp()
    {
        int v = _animalInfoList.Count / PAGE_UNIT_COUNT;
        int r = _animalInfoList.Count % PAGE_UNIT_COUNT;
        int t = r > 0 ? v + 1 : v;

        ViewPage = Mathf.Min(ViewPage + 1, t);
        
        Set();
    }
    private void PageDown()
    {
        ViewPage = Mathf.Max(1, ViewPage - 1);
        
        Set();
    }
    private void PageInit()
    {
        ViewPage = 1;
        IsEvo = false;
        
        Set();
    }
    private void PageLast()
    {
        int v = _animalInfoList.Count / PAGE_UNIT_COUNT;
        int r = _animalInfoList.Count % PAGE_UNIT_COUNT;
        int t = r > 0 ? v + 1 : v;

        ViewPage = t;
        
        Set();
    }
    private void ApplyEvo()
    {
        IsEvo = !IsEvo;
        
        Set();
    }
    #endregion
}
#endif