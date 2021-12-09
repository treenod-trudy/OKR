using UnityEngine;

public enum PopupStyle
{
    Layout,
    Screen,
}

public class Popup : MonoBehaviour
{
    #region public
    public PopupStyle Style;
    #endregion
    
    #region method
    public virtual void Init(PopupStyle style)
    {
        Style = style;
    }

    public virtual void Show()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        if (gameObject.activeSelf == true)
            gameObject.SetActive(false);
    }
    #endregion
}