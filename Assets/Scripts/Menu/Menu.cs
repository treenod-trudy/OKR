using UnityEngine.UI;
using UnityEngine;

using TMPro;

using Random = UnityEngine.Random;

public class Menu : Popup
{
    #region public
    public RectTransform PopupTransform;
    public Image PopupImage;
    public TextMeshProUGUI PopupText;
    #endregion

    #region method
    public override void Init(PopupStyle style)
    {
        base.Init(style);
        
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        float a = style == PopupStyle.Screen ? 1f : 0.6f;
        
        PopupImage.color = new Color(r, g, b, a);
        PopupText.text = $"order : {transform.GetSiblingIndex()}";

        float scale = style == PopupStyle.Screen ? 1f : Random.Range(0.5f, 0.85f);
        PopupTransform.localScale = Vector3.one * scale;
    }
    #endregion
}