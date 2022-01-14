#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CodePreset
{
    public class CPEditorWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "CodePresetEditor";
        private const float WINDOW_MAX_WIDTH = 1350f;
        private const float WINDOW_MAX_HEIGHT = 800f;

        private static CPEditorWindow editorWindow;
       
        [SerializeField] private CPEditorWriteMenu writeMenu;

        [MenuItem("Window/CodePreset/OpenEditor #_w")]
        static void OpenWindow()
        {
            editorWindow = (CPEditorWindow)GetWindow(typeof(CPEditorWindow), false, WINDOW_TITLE);
            editorWindow.maxSize = new Vector2(WINDOW_MAX_WIDTH, WINDOW_MAX_HEIGHT);
            editorWindow.minSize = editorWindow.maxSize;
        }

        private void Awake()
        {
            writeMenu = new CPEditorWriteMenu();
            writeMenu.Initialize();
        }

        private void OnEnable()
        {
            editorWindow = this;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);
            writeMenu?.DrawMenu();
        }
    }

    public abstract class CPEditorMenu
    {
        public abstract void Initialize();
        public abstract void DrawMenu();
    }
}
#endif