using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuContainer))]
[CanEditMultipleObjects]
public class MenuContainerEditor : Editor
{
    #region EditorData
    private class EditorData
    {
        public bool _isVisiable;
        public bool _isEditingCategory;
        public string _editingCategoryName;
        public GameObject _menuPrefab;
    }
    #endregion
    
    #region const
    private const string CONTROL_NAME_FOCUS_OUT = "focusOut";
    private const string CONTROL_NAME_NEW_CATEGORY = "newCategory";
    private const string CONTROL_NAME_EDIT_CATEGORY_TEXTFIELD = "editCategoryTextField";
    #endregion
    
    #region private
    private MenuContainer _menuContainer;
    private string _newCategoryName;
    
    private Dictionary<MenuContainer.CategoryData, EditorData> _editorDatas =
        new Dictionary<MenuContainer.CategoryData, EditorData>();
    #endregion

    #region method
    private void OnEnable()
    {
        _menuContainer = target as MenuContainer;

        foreach (var categoryData in _menuContainer.CategoryDataList)
        {
            bool exist = _editorDatas.ContainsKey(categoryData);
            if (!exist)
            {
                _editorDatas.Add(categoryData, new EditorData
                {
                    _isVisiable = true,
                    _isEditingCategory = false
                });
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        {
            DrawTopMenu();
            DrawFocusOutObject();
            DrawNewCategory();
            DrawField();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTopMenu()
    {
        GUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("+ Expand All")) 
            {
                foreach(var pair in _editorDatas)
                {
                    pair.Value._isVisiable = true;
                }
            }

            if(GUILayout.Button("- Collapse All")) 
            {
                foreach(var pair in _editorDatas)
                {
                    pair.Value._isVisiable = false;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
    private void DrawFocusOutObject()
    {
        GUI.SetNextControlName(CONTROL_NAME_FOCUS_OUT);
        GUI.Label(new Rect(-100, -100, 1, 1), "");
    }
    private void DrawNewCategory()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            GUI.SetNextControlName(CONTROL_NAME_NEW_CATEGORY);
            _newCategoryName = EditorGUILayout.TextField("New Category", _newCategoryName);
			
            SetGUIColor(Color.green);
            if(GUILayout.Button("Add", GUILayout.Width(50)) || IsReturnPressedOn(CONTROL_NAME_NEW_CATEGORY)) 
            {
                if(CheckValidCategoryName(_newCategoryName)) 
                {
                    AddNewCategory(_newCategoryName);
                } 

                _newCategoryName = "";
                ResetFocus();
                EditorUtility.SetDirty(target);
            }
            SetGUIColor(Color.white);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        Rect rect = GUILayoutUtility.GetLastRect();
        EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 1), Color.black);

        EditorGUILayout.Space();
    }
    
    private void ResetFocus() 
    {
        EditorGUI.FocusTextInControl(CONTROL_NAME_FOCUS_OUT);
    }
    
    private void AddNewCategory(string newCategoryName) 
    {
        if(_menuContainer.CategoryDataList.Find(data => data.category.Equals(newCategoryName)) == null) 
        {
            MenuContainer.CategoryData data = new MenuContainer.CategoryData();
            data.category = newCategoryName;
            data.PrefabList = new List<GameObject>();
			
            _menuContainer.CategoryDataList.Add(data);
            _editorDatas.Add(data, new EditorData
            {
                _isVisiable = true, 
                _isEditingCategory = false
            });
			
            EditorApplication.MarkSceneDirty();
        }
    }
    private bool CheckValidCategoryName(string categoryName)
    {
        if(string.IsNullOrEmpty(categoryName))
        {
            EditorUtility.DisplayDialog("Warning!", "카테고리 이름을 입력해주세요!", "Ok");
            return false;
        }
		
        categoryName = categoryName.Trim();
        if(string.IsNullOrEmpty(categoryName))
        {
            EditorUtility.DisplayDialog("Warning!", "카테고리 이름을 입력해주세요!", "Ok");
            return false;
        }
		
        if(IsSameCategoryNameExists(categoryName)) 
        {
            EditorUtility.DisplayDialog("Warning!", "같은 카테고리 이름이 이미 사용 중입니다!", "Ok");
            return false;
        }
		
        return true;
    }
    private bool IsSameCategoryNameExists(string categoryName) 
    {
        string lowerCategoryName = categoryName.ToLower();
        return _menuContainer.CategoryDataList.Find(data => data.category.ToLower().Equals(lowerCategoryName)) != null;
    }

    private void DrawField()
    {
        for(int i = 0; i < _menuContainer.CategoryDataList.Count; i++) 
        {
			MenuContainer.CategoryData categoryData = _menuContainer.CategoryDataList[i];
			EditorData editorData = _editorDatas[categoryData];

			string textFieldControlName = CONTROL_NAME_EDIT_CATEGORY_TEXTFIELD + i;

			EditorGUILayout.BeginHorizontal();
			{
				if(GUILayout.Button(editorData._isVisiable ? "-" : "+", GUILayout.Width(20))) 
				{
					editorData._isVisiable = !editorData._isVisiable;
				}
				
				if(editorData._isEditingCategory) 
				{
					GUI.SetNextControlName(textFieldControlName);
					editorData._editingCategoryName = EditorGUILayout.TextField(editorData._editingCategoryName);
				} 
				else 
				{
					EditorGUILayout.LabelField(categoryData.category + " (" + categoryData.PrefabList.Count + ")", EditorStyles.boldLabel);
				}
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			{
				if(editorData._isEditingCategory) 
				{
					if(GUILayout.Button("Save") || IsReturnPressedOn(textFieldControlName)) 
					{
						if(categoryData.category.ToLower().Equals(editorData._editingCategoryName.ToLower()))
						{
						}
						else if(CheckValidCategoryName(editorData._editingCategoryName))
						{
							categoryData.category = editorData._editingCategoryName;
						} 
						else 
						{
							editorData._editingCategoryName = categoryData.category;
						}

						editorData._isEditingCategory = false;

						ResetFocus();
						EditorUtility.SetDirty(target);
						EditorApplication.MarkSceneDirty();
					}
				} 
				else 
				{
					if(GUILayout.Button("Edit")) 
					{
						editorData._editingCategoryName = categoryData.category;
						editorData._isEditingCategory = true;

						EditorGUI.FocusTextInControl(textFieldControlName);
						EditorUtility.SetDirty(target);
					}
				}

				if(GUILayout.Button("Sort")) 
				{
					categoryData.PrefabList.Sort((first, second) =>
					{
						return first.name.CompareTo(second.name);
					});
					EditorApplication.MarkSceneDirty();
				}

				SetGUIColor(Color.red);
				if(GUILayout.Button("Delete")) 
				{
					if(EditorUtility.DisplayDialog("Warning!", "카테고리에 포함되어 있는 프리팹 리스트도 사라집니다. 정말 삭제하시겠습니까?", "Yes", "No")) 
					{
						_menuContainer.CategoryDataList.RemoveAt(i);
						i--;
						_editorDatas.Remove(categoryData);
						
						EditorApplication.MarkSceneDirty();
					}
				}
				SetGUIColor(Color.white);

				if(GUILayout.Button("▲", GUILayout.Width(30)))
				{
					if(SwapCategoryData(_menuContainer.CategoryDataList, i - 1, i))
					{
						EditorApplication.MarkSceneDirty();
					}
				}
				
				if(GUILayout.Button("▼", GUILayout.Width(30)))
				{
					if(SwapCategoryData(_menuContainer.CategoryDataList, i + 1, i))
					{
						EditorApplication.MarkSceneDirty();
					}
				}
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
			
			if(editorData._isVisiable) {
				EditorGUI.indentLevel += 1;
				{
					DrawCategoryDataList(categoryData, editorData);
					DrawNewPrefab(categoryData, editorData);
				}
				EditorGUI.indentLevel -= 1;
			}
		}
    }
    private bool SwapCategoryData(List<MenuContainer.CategoryData> list, int firstIndex, int secondIndex)
    {
	    if(firstIndex < 0 || firstIndex >= list.Count ||
	       secondIndex < 0 || secondIndex >= list.Count)
		    return false;

	    MenuContainer.CategoryData firstData = list[firstIndex];
	    MenuContainer.CategoryData secondData = list[secondIndex];
	    list[firstIndex] = secondData;
	    list[secondIndex] = firstData;

	    return true;
    }
    private void DrawCategoryDataList(MenuContainer.CategoryData categoryData, EditorData editData) 
	{
		//draw items
		for(int i = 0; i < categoryData.PrefabList.Count; i++) 
		{
			GameObject prefab = categoryData.PrefabList[i];

			//Contents
			EditorGUILayout.BeginHorizontal();
			{
				prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
				
				if(GUILayout.Button("X", GUILayout.Width(30))) 
				{
					categoryData.PrefabList.RemoveAt(i);
					i--;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private void DrawNewPrefab(MenuContainer.CategoryData categoryData, EditorData editData) 
	{
		//Add new prefab
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("New", GUILayout.Width(50));
			editData._menuPrefab = EditorGUILayout.ObjectField(editData._menuPrefab, typeof(GameObject), false) as GameObject;
			
			SetGUIColor(Color.green);
			string addButtonControlName = "addButton" + categoryData.category;
			GUI.SetNextControlName(addButtonControlName);
			if(GUILayout.Button("Add", GUILayout.Width(40))) 
			{
				if(editData._menuPrefab != null) 
				{
					if(IsSamePrefabExists(editData._menuPrefab)) 
					{
						editData._menuPrefab = null;
						EditorUtility.DisplayDialog("Warning!", "이미 등록된 Prefab입니다.", "Ok");
						ResetFocus();

						EditorUtility.SetDirty(target);
					} 
					else 
					{
						categoryData.PrefabList.Add(editData._menuPrefab);
						editData._menuPrefab = null;
					}
				} 
				else 
				{
					string msg = editData._menuPrefab == null ? "Prefab이 없습니다!\n" : "";
					EditorUtility.DisplayDialog("Warning!", msg, "Ok");
					ResetFocus();
				}
			}
			SetGUIColor(Color.white);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		Rect rect = GUILayoutUtility.GetLastRect();
		EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 1), Color.grey);

		EditorGUILayout.Space();
	}
    
	private bool IsSamePrefabExists(GameObject prefab) 
	{
		var pools = _menuContainer.CategoryDataList;
		for(int i = 0; i < pools.Count; i++) 
		{
			var pool = pools[i];
			if(pool.PrefabList.Find(pf => pf == prefab) != null)
				return true;
		}
		return false;
	}
	
    private bool IsReturnPressedOn(string controlName) 
    {
        return 	Event.current.isKey && 
                Event.current.keyCode == KeyCode.Return && 
                GUI.GetNameOfFocusedControl() == controlName;
    }

    private void SetGUIColor(Color color)
    {
        GUI.color = color;
    }
    #endregion
}