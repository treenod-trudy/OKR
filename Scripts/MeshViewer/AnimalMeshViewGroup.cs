#if UNITY_EDITOR
using UnityEngine;

public class AnimalMeshViewGroup : MonoBehaviour
{
    #region public
    public UILabel NameLabel;
    public Animal[] Animals;
    #endregion
    
    #region method
    public void Show(AnimalInfo template, AnimalInfo[] animalInfos)
    {        
        gameObject.SetActive(true);

        bool possibleEvo = AnimalMeshHelper.IsPossibleEvo(template._animalType);
        bool shownEvo = animalInfos[0]._isEvo;
        
        for (int i = 0; i < animalInfos.Length; i++)
        {
            Animals[i]._info = animalInfos[i];
            Animals[i]._info._isEvo &= possibleEvo;
            Animals[i].SetRight(animalInfos[i]._isRight);
        }

        string textColor = shownEvo && possibleEvo ? "[FF00FF]" : "[FFFFFF]"; 
        NameLabel.text = string.Format("{0}name: {1}[-]", textColor, template._type);
    }
    public void Hide()
    {
        DestroyImmediate(gameObject);
    }
    #endregion
}
#endif