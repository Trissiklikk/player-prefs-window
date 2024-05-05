#if UNITY_EDITOR_WIN
using Microsoft.Win32;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsWindow : EditorWindow
    {
        private const string REG_KEY_PATH_PATTERN = @"Software\Unity\UnityEditor\{0}\{1}";
        private const string REG_KEY_PATH_EDITOR_PATTERN = @"Software\Unity Technologies\Unity Editor 5.x"; // Should handle different versions of Unity Editor for ready Unity 6.x.

        private bool m_foldoutProjectData = true;
        private bool m_foldoutUnityData = false;
        private bool m_foldoutEditorData = false;
        private int m_currentTab = 0;
        private Vector2 m_scrollPosition = Vector2.zero;
        private PlayerPrefsSaveHandle m_playerPrefsSaveHandle;
        private EditorPrefsSaveHandle m_editorPrefsSaveHandle;
        private PlayerPrefsElementUtility m_elementUtility;
        private List<string> m_projectKeys = new List<string>();
        private List<string> m_unityKeys = new List<string>();
        private List<string> m_editorKeys = new List<string>();
        private List<PrefHolder> m_playerPrefsHolder = new List<PrefHolder>();
        private List<PrefHolder> m_unityPlayerPrefsHolder = new List<PrefHolder>();
        private List<PrefHolder> m_editorPrefsHolder = new List<PrefHolder>();

        [MenuItem("Window/Trissiklikk Editor Tools/PlayerPrefs Window %#F1")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PlayerPrefsWindow));
        }

        private void OnEnable()
        {
            m_elementUtility = new PlayerPrefsElementUtility();
            m_playerPrefsSaveHandle = new PlayerPrefsSaveHandle();
            m_editorPrefsSaveHandle = new EditorPrefsSaveHandle();

            PlayerPrefsAddPopup.OnCompleteAddData += RefreshPlayerPrefs;

            RefreshPlayerPrefs();
        }

        private void OnDisable()
        {
            PlayerPrefsAddPopup.OnCompleteAddData -= RefreshPlayerPrefs;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            int tab = GUILayout.Toolbar(m_currentTab, new string[] { "Player Prefs", "Editor Prefs" });

            GUILayout.Space(10);

            m_currentTab = tab;

            switch (m_currentTab)
            {
                case 0:
                    DrawPlayerPrefsContent();
                    break;
                case 1:
                    DrawEdiorPrefsContent();
                    break;
                default:
                    throw new System.ArgumentNullException();
            }

        }

        private void DrawPlayerPrefsContent()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add"))
                {
                    PlayerPrefsAddPopup.ShowWindow();
                }

                if (GUILayout.Button("Refresh"))
                {
                    RefreshPlayerPrefs();
                }
            }

            GUILayout.Space(10);

            using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPosition, true, true, GUILayout.Height(position.height), GUILayout.Width(position.width)))
            {
                m_scrollPosition = scrollView.scrollPosition;

                #region Unity PlayerPrefs

                m_foldoutUnityData = EditorGUILayout.Foldout(m_foldoutUnityData, "Unity PlayerPrefs", EditorStyles.foldout);

                if (m_foldoutUnityData)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < m_unityPlayerPrefsHolder.Count; i++)
                    {
                        PrefHolder data = m_unityPlayerPrefsHolder[i];

                        switch (data.Type)
                        {
                            case PlayerPrefType.String:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.StringValue);
                                continue;

                            case PlayerPrefType.Int:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.IntValue.ToString());
                                continue;

                            case PlayerPrefType.Float:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.FloatValue.ToString());
                                continue;
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                #endregion

                EditorGUILayout.Space();

                #region Project Prefs

                m_foldoutProjectData = EditorGUILayout.Foldout(m_foldoutProjectData, "Project PlayerPrefs", EditorStyles.foldout);

                if (m_foldoutProjectData)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < m_playerPrefsHolder.Count; i++)
                    {
                        PrefHolder data = m_playerPrefsHolder[i];

                        switch (data.Type)
                        {
                            case PlayerPrefType.String:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.StringValue, RemovePlayerPrefs);
                                continue;

                            case PlayerPrefType.Int:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.IntValue.ToString(), RemovePlayerPrefs);
                                continue;

                            case PlayerPrefType.Float:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.FloatValue.ToString(), RemovePlayerPrefs);
                                continue;
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                #endregion
            }
        }

        private void DrawEdiorPrefsContent()
        {

            m_foldoutEditorData = EditorGUILayout.Foldout(m_foldoutEditorData, "Editor Prefs", EditorStyles.foldout);

            using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPosition, true, true, GUILayout.Height(position.height), GUILayout.Width(position.width)))
            {
                m_scrollPosition = scrollView.scrollPosition;

                if (m_foldoutEditorData)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < m_editorPrefsHolder.Count; i++)
                    {
                        PrefHolder data = m_editorPrefsHolder[i];

                        switch (data.Type)
                        {
                            case PlayerPrefType.String:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.StringValue);
                                continue;

                            case PlayerPrefType.Int:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.IntValue.ToString());
                                continue;

                            case PlayerPrefType.Float:
                                m_elementUtility.KeyDataField(data.Key, data.Type, data.FloatValue.ToString());
                                continue;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Method for get all player pref keys in registry.
        /// </summary>
        private void GetAllPlayerPrefKeys()
        {
            string regKeyPath = string.Format(REG_KEY_PATH_PATTERN, PlayerSettings.companyName, PlayerSettings.productName);
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(regKeyPath);

            if (regKey == null)
            {
                return;
            }

            string[] keys = regKey.GetValueNames();

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];

                key = key.Substring(0, key.LastIndexOf("_h"));

                if (key.Contains("unity.") || key.Contains("UnityGraphicsQuality"))
                {
                    m_unityKeys.Add(key);
                }
                else
                {
                    m_projectKeys.Add(key);
                }
            }
        }

        /// <summary>
        /// Get all editor pref keys in registry.
        /// </summary>
        private void GetAllEditorPrefKeys()
        {
            string regKeyPath = string.Format(REG_KEY_PATH_EDITOR_PATTERN, PlayerSettings.companyName, PlayerSettings.productName);
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(regKeyPath);

            if (regKey == null)
            {
                return;
            }

            string[] keys = regKey.GetValueNames();

            m_editorKeys = new List<string>(keys);
        }

        /// <summary>
        /// Refresh player prefs data.
        /// </summary>
        private void RefreshPlayerPrefs()
        {
            m_projectKeys = new List<string>();
            m_unityKeys = new List<string>();
            m_editorKeys = new List<string>();

            m_playerPrefsHolder = new List<PrefHolder>();
            m_unityPlayerPrefsHolder = new List<PrefHolder>();
            m_editorPrefsHolder = new List<PrefHolder>();

            GetAllPlayerPrefKeys();
            GetAllEditorPrefKeys();
            m_playerPrefsSaveHandle.GetValue(ref m_projectKeys, ref m_playerPrefsHolder);
            m_playerPrefsSaveHandle.GetValue(ref m_unityKeys, ref m_unityPlayerPrefsHolder);
            m_editorPrefsSaveHandle.GetValue(ref m_editorKeys, ref m_editorPrefsHolder);
            GUI.FocusControl(null);
        }

        /// <summary>
        /// Method for removing player prefs key.
        /// </summary>
        /// <param name="key"></param>
        private void RemovePlayerPrefs(string key)
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this key?", "Yes", "No"))
            {
                m_playerPrefsSaveHandle.Remove(key);
                RefreshPlayerPrefs();
            }
        }
    }
}
#endif
