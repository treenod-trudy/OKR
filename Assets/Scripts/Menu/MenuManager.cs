using UnityEngine;

using Zenject;

public class MenuManager
{
    #region private
    [Inject] private MenuFactory _menuFactory;
    [Inject] private PopupStack _popupStack;

    private readonly Transform _parentTransform;
    #endregion

    #region initialization
    public MenuManager(Transform parentTransform)
    {
        _parentTransform = parentTransform;
    }
    #endregion
    
    #region method
    public void Make(string menuName, PopupStyle style)
    {
        GameObject go = _menuFactory.Create(menuName);
        go.transform.SetParent(_parentTransform, false);
        go.transform.SetSiblingIndex(_popupStack.Depth);

        Menu menuPopup = go.GetComponent<Menu>();
        menuPopup.Init(style);
        
        _popupStack.Push(menuPopup);
    }

    public void Remove()
    {
        _popupStack.Pop();
    }

    public void Clear()
    {
        _popupStack.ClearAll();
    }
    #endregion
}
public class MenuFactory
{
    #region private
    private DiContainer _diContainer;
    private MenuContainer _menuContainer;
    #endregion

    #region initialzation
    [Inject]
    public MenuFactory(DiContainer diContainer, MenuContainer menuContainer)
    {
        _diContainer = diContainer;
        
        _menuContainer = menuContainer;
        _menuContainer.Init();
    }
    #endregion
    
    #region method
    public GameObject Create(string menuName)
    {
        GameObject prefab = _menuContainer.GetPrefab(menuName);

        bool existPrefab = prefab != null;
        return existPrefab ? _diContainer.InstantiatePrefab(prefab) : null;
    }
    #endregion
}