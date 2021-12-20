#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace PKWork
{
    public class PokoRepeatWorkEditor : EditorWindow
    {
        private static PokoRepeatWorkEditor _editor;
        
        private readonly PokokoroWork _pokokoroWork = new PokokoroWork();
        
        private readonly CollaboCheerUpWork _collaboCheerUpWork = new CollaboCheerUpWork();
        private readonly CollaboBlocksWork _collaboBlocksWork = new CollaboBlocksWork();

        private readonly ScrollPackageWork _scrollPackageWork = new ScrollPackageWork();
        private readonly OptionalPackageWork _optionalPackageWork = new OptionalPackageWork();
        
        [MenuItem("POKO/RepeatWork Editor")]
        static void OpenWindow()
        {
            _editor = (PokoRepeatWorkEditor)GetWindow(typeof(PokoRepeatWorkEditor), false, "Work EditorWindow");
        }

        private void OnGUI()
        {
            _pokokoroWork.DrawMenu();
            
            _collaboCheerUpWork.DrawMenu();
            _collaboBlocksWork.DrawMenu();
            
            _scrollPackageWork.DrawMenu();
            _optionalPackageWork.DrawMenu();
        }
    }


    public class PokoWorkEditor
    {
        #region const
        protected readonly string TEST_OUTPUT_FOLDER = "Assets/0. __WORK_EDITOR/Output/";
        #endregion

        #region 공통 메뉴
        protected void DrawTitle(string title)
        {
            GUI.contentColor = Color.yellow;
            {
                GUILayout.Label(title);
            }
            GUI.contentColor = Color.white;
        }
        
        protected void DrawFile(string path)
        {
            Object file = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorGUILayout.ObjectField(file, typeof(Object));
        }
        #endregion
    }
    
    public class PokokoroWork : PokoWorkEditor
    {
        #region const
        private readonly string TITLE = "포코코로";
        private readonly string FOLDER_PATH = "Assets/Editor Default Resources/AssetBundleContents/bonusdeco";
        private readonly string IMAGE_REF_ASSET = "PokokoroRef.asset"; 
        private readonly string HISTORY_ASSET = "PokokoroHistory.asset";
        #endregion

        #region private
        private readonly List<Texture> _changedAssetList = new List<Texture>();
        private Vector2 _scroll;
        #endregion
        
        #region properties
        public bool IsChangedAssets { get { return _changedAssetList.Count > 0; } }
        #endregion
            
        #region 메뉴
        public void DrawMenu()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal("box");
                {
                    DrawTitle(TITLE);
                    DrawFile(FOLDER_PATH);
                    DrawFile(TEST_OUTPUT_FOLDER + IMAGE_REF_ASSET);
                    DrawFile(TEST_OUTPUT_FOLDER + HISTORY_ASSET);
                    DrawButtons();
                }
                GUILayout.EndHorizontal();

                DrawBlackBoard();
            }
            GUILayout.EndVertical();
        }
        private void DrawButtons()
        {
            GUI.backgroundColor = Color.yellow;
            {
                if (GUILayout.Button("기록"))
                {
                    PokoResourceWork.SaveTextureContentsHash(FOLDER_PATH,
                        TEST_OUTPUT_FOLDER + HISTORY_ASSET);
                }
            }

            if (IsChangedAssets)
            {
                GUI.backgroundColor = Color.green;
                {
                    if (GUILayout.Button("실행"))
                    {
                        Apply();
                    }
                }
            }
            else
            {
                GUI.backgroundColor = Color.cyan;
                {
                    if (GUILayout.Button("확인"))
                    {
                        Check();
                    }
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawBlackBoard()
        {
            if (IsChangedAssets)
            {
                DrawChangedAssets();
            }
        }
        private void DrawChangedAssets()
        {
            GUI.contentColor = Color.green;
            {
                GUILayout.Label("ㄴ검증 목록");
            }
            GUI.contentColor = Color.white;
            
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(50));
            {
                foreach (var changedAsset in _changedAssetList)
                {
                    DrawChangedAsset(changedAsset);
                }
            }
            GUILayout.EndScrollView();
        }
        private void DrawChangedAsset(Object changedAsset)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.Label(changedAsset.name);
                EditorGUILayout.ObjectField(changedAsset, typeof(Object));
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 메서드
        private void Check()
        {
            List<Object> assets = PokoResourceWork.LoadAllAssetsAtPath(FOLDER_PATH);

            bool isExist = assets != null && assets.Count > 0;
            if (isExist)
            {
                AssetHistoryStorage historyStorage
                    = AssetDatabase.LoadAssetAtPath<AssetHistoryStorage>(TEST_OUTPUT_FOLDER + HISTORY_ASSET);

                bool existHistory = historyStorage != null 
                                    && historyStorage.AssetHistory != null
                                    && historyStorage.AssetHistory.Length > 0;
                if (existHistory)
                {
                    _changedAssetList.Clear();
                    
                    foreach (var asset in assets)
                    {
                        Texture texture = asset as Texture;

                        bool isChanged =
                            !historyStorage.Validate(texture.name, texture.imageContentsHash.GetHashCode()); 
                        if (isChanged)
                        {
                            _changedAssetList.Add(texture);
                        }
                    }
                }
            }
        }

        private void Apply()
        {
            PokoResourceWork.ApplyTextureSettings(_changedAssetList.ToArray(), TEST_OUTPUT_FOLDER + IMAGE_REF_ASSET);
            PokoResourceWork.SaveTextureContentsHash(FOLDER_PATH, TEST_OUTPUT_FOLDER + HISTORY_ASSET);
            
            _changedAssetList.Clear();
            _scroll = Vector2.zero;
        }
        #endregion
    }

    public class UpdateAtlasWork : PokoWorkEditor
    {
        #region protected
        protected readonly List<Texture> _changedAssetList = new List<Texture>();
        protected Vector2 _scroll;
        #endregion
        
        #region properties
        public virtual string TITLE { get { return "title"; } }
        public virtual string FOLDER_PATH { get { return "folder"; } }
        public virtual string IMAGES_PATH { get { return "images"; } }
        public virtual string ATLAS_ASSET { get { return "atlas.prefab"; } }
        public virtual string IMAGE_REF_ASSET { get { return "ref.asset"; } }

        public bool IsChangedAssets { get { return _changedAssetList.Count > 0; } }
        #endregion
        
        #region 메뉴
        public void DrawMenu()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal("box");
                {
                    DrawTitle(TITLE);
                    DrawFile(FOLDER_PATH + IMAGES_PATH);
                    DrawFile(TEST_OUTPUT_FOLDER + IMAGE_REF_ASSET);
                    DrawFile(FOLDER_PATH + ATLAS_ASSET);
                    DrawButtons();
                }
                GUILayout.EndHorizontal();

                DrawBlackBoard();
            }
            GUILayout.EndVertical();
        }
        private void DrawButtons()
        {
            if (IsChangedAssets)
            {
                GUI.backgroundColor = Color.green;
                {
                    if (GUILayout.Button("실행"))
                    {
                        Apply();
                    }
                }
            }
            else
            {
                GUI.backgroundColor = Color.cyan;
                {
                    if (GUILayout.Button("확인"))
                    {
                        Check();
                    }
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void DrawBlackBoard()
        {
            if (IsChangedAssets)
            {
                DrawChangedAsset();
            }
        }
        private void DrawChangedAsset()
        {
            GUI.contentColor = Color.green;
            {
                GUILayout.Label("ㄴ검증 목록");
            }
            GUI.contentColor = Color.white;

            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(200));
            {
                foreach (var changedAsset in _changedAssetList)
                {
                    DrawChangedAsset(changedAsset);
                }
            }
            GUILayout.EndScrollView();
        }
        private void DrawChangedAsset(Object changedAsset)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.Label(changedAsset.name);
                EditorGUILayout.ObjectField(changedAsset, typeof(Object));
            }
            GUILayout.EndHorizontal();
        }
        #endregion
        
        #region 메서드
        private void Check()
        {
            List<Object> assets = PokoResourceWork.LoadAllAssetsAtPath(FOLDER_PATH + IMAGES_PATH);

            bool isExist = assets != null && assets.Count > 0;
            if (isExist)
            {
                _changedAssetList.Clear();

                foreach (var asset in assets)
                {
                    _changedAssetList.Add(asset as Texture);
                }
            }
            
            PokoResourceWork.ApplyTextureSettings(_changedAssetList.ToArray(), TEST_OUTPUT_FOLDER + IMAGE_REF_ASSET);
        }
        
        private void Apply()
        {
            UIAtlas atlas = AssetDatabase.LoadAssetAtPath<UIAtlas>(FOLDER_PATH + ATLAS_ASSET);

            bool isExist = atlas != null;
            if (isExist)
            {
                NGUISettings.atlas = atlas;
                PokoResourceWork.UpdateAtlas(atlas, _changedAssetList, false);
                NGUISettings.atlas = null;
            }
            
            _changedAssetList.Clear();
        }
        #endregion
    }
    public class CollaboCheerUpWork : UpdateAtlasWork
    {
        #region properties
        public override string TITLE { get { return "콜라보-응원 이미지"; } }
        public override string FOLDER_PATH
        {
            get { return "Assets/Editor Default Resources/AssetBundleContents/collabo"; }
        }
        public override string IMAGES_PATH { get { return "/stand-images"; } }
        public override string ATLAS_ASSET { get { return "/stand/CollaboStandAtlas.prefab"; } }
        public override string IMAGE_REF_ASSET { get { return "CollaboStandRef.asset"; } }
        #endregion
    }
    public class CollaboBlocksWork : UpdateAtlasWork
    {
        #region properties
        public override string TITLE { get { return "콜라보-블록 이미지"; } }
        public override string FOLDER_PATH
        {
            get { return "Assets/Editor Default Resources/AssetBundleContents/collabo"; }
        }
        public override string IMAGES_PATH { get { return "/block-images"; } }
        public override string ATLAS_ASSET { get { return "/block/CollaboAtlas.prefab"; } }
        public override string IMAGE_REF_ASSET { get { return "CollaboBlockRef.asset"; } }
        #endregion
    }

    public class UpdateLocaleAtlasWork : PokoWorkEditor
    {
        #region const
        protected readonly string[] LOCALE = { "en", "ja", "th", "tw" };
        #endregion
        
        #region protected
        protected readonly Dictionary<Object, string> _resourceMap = new Dictionary<Object, string>();
        protected readonly List<Texture> _changedAssetList = new List<Texture>();
        protected Vector2 _scroll;
        #endregion

        #region properties
        public virtual string TITLE { get { return "title"; } }
        public virtual string FOLDER_PATH { get { return "folder"; } }
        public virtual string ATLAS_ASSET { get { return "atlas.prefab"; } }
        public virtual string HISTORY_ASSET { get { return "assetHistory.asset"; } }

        public bool IsChangedAssets { get { return _changedAssetList.Count > 0; } }
        #endregion
        
        #region 메뉴
        public void DrawMenu()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal("box");
                {
                    DrawTitle(TITLE);
                    DrawFile(FOLDER_PATH);
                    DrawFile(TEST_OUTPUT_FOLDER + HISTORY_ASSET);
                    DrawLocaleAtlas();
                    DrawButtons();
                }
                GUILayout.EndHorizontal();

                DrawBlackBoard();
            }
            GUILayout.EndVertical();
        }
        
        private void DrawLocaleAtlas()
        {
            GUILayout.BeginHorizontal();
            {
                foreach (var locale in LOCALE)
                {
                    DrawLocaleAtlas(locale);
                }
            }
            GUILayout.EndHorizontal();
        }
        private void DrawLocaleAtlas(string locale)
        {
            if (GUILayout.Button(locale))
            {
                string path = string.Format("{0}/{1}/{2}", FOLDER_PATH, locale, ATLAS_ASSET);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            }
        }
        
        private void DrawButtons()
        {
            GUI.backgroundColor = Color.yellow;
            {
                if (GUILayout.Button("기록"))
                {
                    Record();
                }
            }
            
            if (IsChangedAssets)
            {
                GUI.backgroundColor = Color.green;
                {
                    if (GUILayout.Button("실행"))
                    {
                        Apply();
                    }
                }
            }
            else
            {
                GUI.backgroundColor = Color.cyan;
                {
                    if (GUILayout.Button("확인"))
                    {
                        Check();
                    }
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawBlackBoard()
        {
            if (IsChangedAssets)
            {
                DrawChangedAsset();
            }
        }
        private void DrawChangedAsset()
        {
            GUI.contentColor = Color.green;
            {
                GUILayout.Label("ㄴ검증 목록");
            }
            GUI.contentColor = Color.white;

            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(150));
            {
                foreach (var changedAsset in _changedAssetList)
                {
                    DrawChangedAsset(changedAsset);
                }
            }
            GUILayout.EndScrollView();
        }
        private void DrawChangedAsset(Object changedAsset)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.Label(changedAsset.name);
                EditorGUILayout.ObjectField(changedAsset, typeof(Object));
            }
            GUILayout.EndHorizontal();
        }
        #endregion
        
        #region 메서드
        private void Check()
        {
            _changedAssetList.Clear();

            foreach (var locale in LOCALE)
            {
                Check(locale);
            }
        }
        private void Check(string locale)
        {
            string resourcePath = string.Format("{0}/{1}-images", FOLDER_PATH, locale);
            
            List<Object> assets = PokoResourceWork.LoadAllAssetsAtPath(resourcePath);

            bool isExist = assets != null && assets.Count > 0;
            if (isExist)
            {
                AssetHistoryStorage historyStorage
                    = AssetDatabase.LoadAssetAtPath<AssetHistoryStorage>(TEST_OUTPUT_FOLDER + HISTORY_ASSET);

                bool existHistory = historyStorage != null 
                                    && historyStorage.AssetHistory != null
                                    && historyStorage.AssetHistory.Length > 0;
                if (existHistory)
                {
                    foreach (var asset in assets)
                    {
                        Texture texture = asset as Texture;

                        string assetName = string.Format("{0}-images/{1}", locale, asset.name);
                        
                        bool isChanged =
                            !historyStorage.Validate(assetName, texture.imageContentsHash.GetHashCode()); 
                        if (isChanged)
                        {
                            _changedAssetList.Add(texture);
                        }
                    }
                }
            }
        }
        
        private void Record()
        {
            _resourceMap.Clear();
            
            foreach (var locale in LOCALE)
            {
                string localeFolder = string.Format("{0}-images", locale);
                string resourcePath = string.Format("{0}/{1}", FOLDER_PATH, localeFolder);

                List<Object> assets = PokoResourceWork.LoadAllAssetsAtPath(resourcePath);

                foreach (var asset in assets)
                {
                    string filePath = string.Format("{0}/{1}", localeFolder, asset.name);
                    
                    _resourceMap.Add(asset, filePath);
                }
            }
            
            PokoResourceWork.SaveTextureContentsHash(_resourceMap, TEST_OUTPUT_FOLDER + HISTORY_ASSET);
        }

        private void Apply()
        {
            foreach (var locale in LOCALE)
            {
                Apply(locale);
            }
            
            _changedAssetList.Clear();

            Record();
        }
        private void Apply(string locale)
        {
            string folder = string.Format("/{0}/", locale);

            UIAtlas atlas = AssetDatabase.LoadAssetAtPath<UIAtlas>(FOLDER_PATH + folder + ATLAS_ASSET);
            
            bool isExist = atlas != null;
            if (isExist)
            {
                List<Texture> textures = new List<Texture>();

                foreach (var changedAsset in _changedAssetList)
                {
                    string path = AssetDatabase.GetAssetPath(changedAsset);

                    folder = string.Format("/{0}-images/", locale);
                    
                    bool contains = path.Contains(folder);
                    if (contains)
                    {
                        textures.Add(changedAsset);
                    }
                }

                if (textures.Count > 0)
                {
                    NGUISettings.atlas = atlas;
                    PokoResourceWork.UpdateAtlas(atlas, textures, true);
                    NGUISettings.atlas = null;
                }
            }
        }
        #endregion
    }
    public class ScrollPackageWork : UpdateLocaleAtlasWork
    {
        #region properties
        public override string TITLE { get { return "스크롤 패키지"; } }
        public override string FOLDER_PATH
        {
            get { return "Assets/Editor Default Resources/AssetBundleContents/pack_scroll"; }
        }
        public override string ATLAS_ASSET { get { return "ScrollPackageAtlas.prefab"; } }
        public override string HISTORY_ASSET { get { return "ScrollPackageHistory.asset"; } }
        #endregion
    }
    public class OptionalPackageWork : UpdateLocaleAtlasWork
    {
        #region properties
        public override string TITLE { get { return "선택형 패키지"; } }
        public override string FOLDER_PATH
        {
            get { return "Assets/Editor Default Resources/AssetBundleContents/pack_option"; }
        }
        public override string ATLAS_ASSET { get { return "OptionPackAtlas.prefab"; } }
        public override string HISTORY_ASSET { get { return "OptionPackageHistory.asset"; } }
        #endregion
    }
}
#endif