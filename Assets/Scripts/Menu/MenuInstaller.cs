using UnityEngine;

using Zenject;

public class MenuInstaller : MonoInstaller
{
    #region const
    private readonly string PREFAB_PATH = "Prefabs/MenuPopup/MenuContainer";
    #endregion
    
    #region public
    public Transform ParentTransform; 
    #endregion
    
    #region method
    public override void InstallBindings()
    {
        Container.Bind<MenuContainer>().FromResource(PREFAB_PATH).AsCached().NonLazy();
        Container.Bind<MenuFactory>().AsSingle().NonLazy();

        Container.Bind<Transform>().FromInstance(ParentTransform).When(context => context.ObjectType == typeof(MenuManager));
        Container.Bind<MenuManager>().AsSingle().NonLazy();
        
        Container.Bind<PopupStack>().AsSingle().NonLazy();
    }
    #endregion
}