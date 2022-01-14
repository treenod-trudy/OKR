#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System;

using Object = UnityEngine.Object;

namespace CodePreset
{
    [Serializable]
    public class CPEditorWriteMenu : CPEditorMenu
    {
        private readonly string TEXT_AREA_CONTROL_NAME = "writeBoard";
        private readonly string C_SHARP_SCRIPT_ICON_NAME = "cs Script Icon";
        private readonly string CREATE_NOTICE_MESSAGE = "(프리셋을 이용한 실제 스크립트 생성은 에디터를 재시작 후 가능합니다.)";

        private readonly string EXAMPLE_FILE_NAME = "82-PkPkScript__ManagerScript-NewManager.cs";
        private readonly string TEMPLATE_FILE_NAME_FORM = "{0}-{1}__{2}-{3}";
        private readonly string TAG_MENU_ORDER = "#MENU_ORDER#";
        private readonly string TAG_MENU_TYPE = "#MENU_TYPE#";
        private readonly string TAG_MENU_NAME = "#MENU_NAME#";
        private readonly string TAG_FILE_DEFAULT_NAME = "#FILE_DEFAULT_NAME#";

        public Object rootFolder;
        public Texture iconTexture;
        public string contents;

        public string templateFilesPath;
        public string[] saveFileName = new string[4];

        public TextAreaTabKeyEvent textAreaTabKeyEvent = new TextAreaTabKeyEvent();
        public List<string> templateFileList = new List<string>();

        public int selectedTemplateFile;
        public Vector2 textAreaScroll;

        public override void Initialize()
        {
            SetIconTexture();
            SetTemplateFiles();
            
            ClearSavedFileName();
        }
        private void SetIconTexture()
        {
            iconTexture = CPEditorHelper.GetIconTextureInUnityEditor(C_SHARP_SCRIPT_ICON_NAME);
        }
        private void SetTemplateFiles()
        {
            templateFilesPath = $"{Application.dataPath}/{CPEditorHelper.TEMPLATE_FILES_PATH}";
            templateFileList.Clear();

            rootFolder = AssetDatabase.LoadAssetAtPath<Object>($"Assets/{CPEditorHelper.TEMPLATE_FILES_PATH}");

            var directoryInfo = new DirectoryInfo(templateFilesPath);
            var files = directoryInfo.GetFiles();
            foreach (var fileInfo in files)
            {
                var isTemplateFile = fileInfo.Extension.Contains("txt");
                if (!isTemplateFile)
                    continue;

                templateFileList.Add(fileInfo.Name);
            }

            selectedTemplateFile = -1;
        }

        public override void DrawMenu()
        {
            DrawWriteBoardTitle();
            
            DrawFilePath();
            GUILayout.Space(5f);

            DrawOptionMenu();
            GUILayout.Space(5f);

            DrawWriteBoard();
            GUILayout.Space(5f);

            DrawWriteBoardMenu();
        }

        private void DrawWriteBoardTitle()
        {
            GUI.backgroundColor = Color.clear;
            {
                GUILayout.Button(iconTexture, GUILayout.Height(50f));
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void DrawFilePath()
        {
            GUILayout.BeginHorizontal("box");
            {
                GUI.contentColor = Color.yellow;
                {
                    GUILayout.Label("Template Files Folder");
                }
                GUI.contentColor = Color.white;

#pragma warning disable 618
                EditorGUILayout.ObjectField(rootFolder, typeof(Object));
#pragma warning restore 618
            }
            GUILayout.EndHorizontal();
        }

        private void DrawOptionMenu()
        {
            GUILayout.BeginHorizontal("box");
            {
                selectedTemplateFile =
                    EditorGUILayout.Popup("copy template", selectedTemplateFile, templateFileList.ToArray());

                bool isSelectedFile = selectedTemplateFile > -1;
                if (isSelectedFile)
                {
                    GUILayoutOption option = GUILayout.Width(200f);
                    GUI.backgroundColor = Color.yellow;
                    {
                        if (GUILayout.Button("복사", option))
                        {
                            CopyTemplateFile();
                        }
                    }
                    GUI.backgroundColor = Color.green;
                    {
                        if (GUILayout.Button("저장", option))
                        {
                            SaveTemplateFile();
                        }
                    }                    
                    GUI.backgroundColor = Color.red;
                    {
                        if (GUILayout.Button("제거", option))
                        {
                            RemoveTemplateFile();
                        }
                    }
                    GUI.backgroundColor = Color.white;           
                }
            }
            GUILayout.EndHorizontal();
        }
        private void CopyTemplateFile()
        {
            string filePath = $"{templateFilesPath}/{templateFileList[selectedTemplateFile]}";
            contents = File.ReadAllText(filePath);
        }
        private void SaveTemplateFile()
        {
            bool empty = string.IsNullOrEmpty(contents);
            if (empty)
                return;
            
            string filePath = $"{templateFilesPath}/{templateFileList[selectedTemplateFile]}";

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(contents);
            streamWriter.Flush();
            streamWriter.Close();
            
            EditorUtility.DisplayDialog("안내", $"{filePath}이 저장되었습니다.", "확인");
                    
            AssetDatabase.Refresh();
        }
        private void RemoveTemplateFile()
        {
            string fileName = templateFileList[selectedTemplateFile];
            string message = $"{fileName}을 정말로 제거하시겠습니까?";
                            
            if (EditorUtility.DisplayDialog("안내", message, "확인", "취소"))
            {
                string filePath = $"{templateFilesPath}/{templateFileList[selectedTemplateFile]}";
                File.Delete(filePath);
                                
                AssetDatabase.Refresh();
                                
                SetTemplateFiles();
            }
        }

        private void ClearSavedFileName()
        {
            saveFileName[0] = TAG_MENU_ORDER;
            saveFileName[1] = TAG_MENU_TYPE;
            saveFileName[2] = TAG_MENU_NAME;
            saveFileName[3] = TAG_FILE_DEFAULT_NAME;
        }

        private void DrawWriteBoard()
        {
            textAreaTabKeyEvent.CheckEvent(ref contents, TEXT_AREA_CONTROL_NAME);
            {
                textAreaScroll = GUILayout.BeginScrollView(textAreaScroll, GUILayout.Height(500f));
                {
                    GUI.SetNextControlName(TEXT_AREA_CONTROL_NAME);
                    {
                        contents = GUILayout.TextArea(contents, GUILayout.ExpandHeight(true));
                    }
                }
                GUILayout.EndScrollView();
            }
            textAreaTabKeyEvent.UpdateFocus();
        }

        private void DrawWriteBoardMenu()
        {
            DrawAssistantMenu();
            GUILayout.Space(10f);
            DrawSaveFileMenu();
        }

        private void DrawAssistantMenu()
        {
            bool empty = string.IsNullOrEmpty(contents);
            if (empty)
                return;
            
            GUILayout.BeginHorizontal("box");
            {
                GUILayoutOption option = GUILayout.Width(200f);
                
                GUI.backgroundColor = Color.cyan;
                {
                    if (GUILayout.Button("작성기록 추가", option))
                    {
                        contents = contents.Insert(0, WriteForm.SCRIPT_RECORD_INFO);
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSaveFileMenu()
        {
            GUILayout.BeginHorizontal("box");
            {
                GUI.contentColor = Color.green;
                {
                    GUILayout.Label("Save FilePath [input]", GUILayout.Width(200f));
                }
                GUI.contentColor = Color.white;
             
                DrawSaveFilePath();
            }
            GUILayout.EndHorizontal();

            DrawSaveFileName();
        }
        private void DrawSaveFilePath()
        {
            GUILayoutOption option = GUILayout.ExpandWidth(false);

            GUILayout.TextField($"{templateFilesPath}/", option);
            saveFileName[0] = GUILayout.TextField(saveFileName[0], option);
            GUILayout.Label("-", option);
            saveFileName[1] = GUILayout.TextField(saveFileName[1], option);
            GUILayout.Label("__", option);
            saveFileName[2] = GUILayout.TextField(saveFileName[2], option);
            GUILayout.Label("-", option);
            saveFileName[3] = GUILayout.TextField(saveFileName[3], option);
            GUILayout.Label(".cs.txt", option);
        }
        private void DrawSaveFileName()
        {
            EditorGUILayout.HelpBox($"FileName Example: {EXAMPLE_FILE_NAME}", MessageType.Warning);
            
            GUILayout.BeginHorizontal("box");
            {
                GUI.contentColor = Color.yellow;
                {
                    GUILayout.Label("Save FilePath [result]", GUILayout.Width(200f));
                }
                GUI.contentColor = Color.white;

                string filePath = string.Format(templateFilesPath + "/" + TEMPLATE_FILE_NAME_FORM + ".cs.txt",
                    saveFileName[0],
                    saveFileName[1],
                    saveFileName[2],
                    saveFileName[3]);
                GUILayout.TextField($"{filePath}");
                
                DrawSaveButton(filePath);
            }
            GUILayout.EndHorizontal();
        }
        private void DrawSaveButton(string filePath)
        {
            GUI.backgroundColor = Color.yellow;
            {
                if (GUILayout.Button("저장", GUILayout.Width(150f)))
                {
                    StreamWriter streamWriter = new StreamWriter(filePath);
                    streamWriter.Write(contents);
                    streamWriter.Flush();
                    streamWriter.Close();

                    SetTemplateFiles();

                    EditorUtility.DisplayDialog("안내", $"{filePath}이 생성되었습니다.{CREATE_NOTICE_MESSAGE}", "확인");
                    
                    AssetDatabase.Refresh();
                }
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
#endif