#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System;

namespace CodePreset
{
    public static class CPEditorHelper
    {
        public static string TEMPLATE_FILES_PATH = "ScriptTemplates";
        
        #region 유니티 에디터용 아이콘
        public static Texture GetIconTextureInUnityEditor(string name)
        {
            var iconContent = EditorGUIUtility.IconContent(name);
            return iconContent.image;
        }
        #endregion
        
    }

    #region TextAreaTabKeyEvent
    [Serializable]
    public class TextAreaTabKeyEvent
    {
        public int lastKeyboardFocus;

        public TextAreaTabKeyEvent()
        {
            lastKeyboardFocus = -1;
        }

        public void CheckEvent(ref string contents, string controlName)
        {
            var current = Event.current;

            if (GUI.GetNameOfFocusedControl() == controlName && lastKeyboardFocus == GUIUtility.keyboardControl)
            {
                if (current.isKey && (current.keyCode == KeyCode.Tab || current.character == '\t'))
                {
                    if (current.type == EventType.KeyUp)
                    {
                        var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

                        if (!current.shift)
                        {
                            for (var i = 0; i < 4; i++)
                            {
                                te.Insert(' ');
                            }
                        }
                        else
                        {
                            var min = Math.Min(te.cursorIndex, te.selectIndex);
                            var index = min;
                            var temp = te.text;

                            for (var i = 1; i < 5; i++)
                            {
                                if ((min - i) < 0 || temp[min - i] != ' ')
                                {
                                    break;
                                }

                                index = min - i;
                            }

                            if (index < min)
                            {
                                te.selectIndex = index;
                                te.cursorIndex = min;
                                te.ReplaceSelection(string.Empty);
                            }
                        }
                        contents = te.text;
                    }
                    current.Use();
                }
            }
        }

        public void UpdateFocus()
        {
            var current = Event.current;

            bool otherControlID = lastKeyboardFocus != GUIUtility.keyboardControl;
            bool inputKeyboard = current.type == EventType.KeyDown || current.type == EventType.KeyUp; 
            
            if (otherControlID && inputKeyboard)
            {
                lastKeyboardFocus = GUIUtility.keyboardControl;
            }
        }
    }
    #endregion
}
#endif