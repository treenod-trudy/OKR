using UnityEngine;

using Zenject;

public class Test : MonoBehaviour
{
    [Inject] private MenuManager _menuManager;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _menuManager.Make("MenuPopup", PopupStyle.Layout);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _menuManager.Make("MenuPopup", PopupStyle.Screen);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            _menuManager.Remove();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _menuManager.Clear();
        }
    }
}