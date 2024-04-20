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
        private const string ERROR_STRING_KEY_MSG = "ERROR_STRING_KEY";
        private const int ERROR_INT_KEY_MSG = int.MinValue;
        private const float ERROR_FLOAT_KEY_MSG = float.NaN;

        private List<string> m_projectKeys = new List<string>();
        private List<string> m_unityKeys = new List<string>();
        private List<PlayerPrefHolder> m_projectPlayerPrefsHolder = new List<PlayerPrefHolder>();
        private List<PlayerPrefHolder> m_unityPlayerPrefsHolder = new List<PlayerPrefHolder>();
        private bool m_foldoutProjectData = true;
        private bool m_foldoutUnityData = false;
        private PlayerPrefsSaveHandle m_playerPrefsSaveHandle;
        private PlayerPrefsElementUtility m_elementUtility;

        [MenuItem("Window/Trissiklikk Editor Tools/PlayerPrefs Window %#F1")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PlayerPrefsWindow));
        }

        private void OnEnable()
        {
            m_elementUtility = new PlayerPrefsElementUtility();
            m_playerPrefsSaveHandle = new PlayerPrefsSaveHandle();

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
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add"))
            {
                PlayerPrefsAddPopup.ShowWindow();
            }

            if(GUILayout.Button("Refresh"))
            {
                RefreshPlayerPrefs();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginScrollView(Vector2.zero, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));

            m_foldoutUnityData = EditorGUILayout.Foldout(m_foldoutUnityData, "Unity PlayerPrefs", EditorStyles.foldout);

            if (m_foldoutUnityData)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < m_unityPlayerPrefsHolder.Count; i++)
                {
                    PlayerPrefHolder data = m_unityPlayerPrefsHolder[i];

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

            EditorGUILayout.Space();

            m_foldoutProjectData = EditorGUILayout.Foldout(m_foldoutProjectData, "Project PlayerPrefs", EditorStyles.foldout);

            if (m_foldoutProjectData)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < m_projectPlayerPrefsHolder.Count; i++)
                {
                    PlayerPrefHolder data = m_projectPlayerPrefsHolder[i];

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

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Method for getting value from player prefs key.
        /// </summary>
        private void GetKeyValue(ref List<string> keys, ref List<PlayerPrefHolder> containList)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (!PlayerPrefs.HasKey(keys[i]))
                {
                    continue;
                }

                string stringValue = PlayerPrefs.GetString(keys[i], ERROR_STRING_KEY_MSG);

                if (stringValue != ERROR_STRING_KEY_MSG)
                {
                    PlayerPrefHolder playerPrefHolder = new PlayerPrefHolder(keys[i]).WithStringValue(stringValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                int intValue = PlayerPrefs.GetInt(keys[i], ERROR_INT_KEY_MSG);

                if (intValue != ERROR_INT_KEY_MSG)
                {
                    PlayerPrefHolder playerPrefHolder = new PlayerPrefHolder(keys[i]).WithIntValue(intValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                float floatValue = PlayerPrefs.GetFloat(keys[i], ERROR_FLOAT_KEY_MSG);

                if (floatValue != ERROR_FLOAT_KEY_MSG)
                {
                    PlayerPrefHolder playerPrefHolder = new PlayerPrefHolder(keys[i]).WithFloatValue(floatValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }
            }
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
        /// Refresh player prefs data.
        /// </summary>
        private void RefreshPlayerPrefs()
        {
            m_projectKeys = new List<string>();
            m_unityKeys = new List<string>();

            m_projectPlayerPrefsHolder = new List<PlayerPrefHolder>();
            m_unityPlayerPrefsHolder = new List<PlayerPrefHolder>();

            GetAllPlayerPrefKeys();
            GetKeyValue(ref m_projectKeys, ref m_projectPlayerPrefsHolder);
            GetKeyValue(ref m_unityKeys, ref m_unityPlayerPrefsHolder);
        }

        /// <summary>
        /// Method for removing player prefs key.
        /// </summary>
        /// <param name="key"></param>
        private void RemovePlayerPrefs(string key)
        {
            if(EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this key?", "Yes", "No"))
            {
                m_playerPrefsSaveHandle.RemovePlayerPrefs(key);
                RefreshPlayerPrefs();
            }
        }
    }
}
#endif
