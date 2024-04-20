﻿#if UNITY_EDITOR_WIN
using System;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsAddPopup : EditorWindow
    {
        public static event Action OnCompleteAddData;

        private string m_keyValue = string.Empty;
        private string m_stringValue = string.Empty;
        private int m_intValue = 0;
        private float m_floatValue = 0.0f;
        private PlayerPrefType m_currentPlayerPrefType = PlayerPrefType.String;
        private PlayerPrefsSaveHandle m_playerPrefsSaveHandle;

        public static void ShowWindow()
        {
            GetWindow(typeof(PlayerPrefsAddPopup));
        }

        private PlayerPrefsAddPopup()
        {
            m_playerPrefsSaveHandle = new PlayerPrefsSaveHandle();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField("Player Prefs Type");
                GUILayout.FlexibleSpace();
                m_currentPlayerPrefType = (PlayerPrefType)EditorGUILayout.EnumPopup(m_currentPlayerPrefType);
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField("Key : ");
                m_keyValue = EditorGUILayout.TextField(m_keyValue);
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(2.5f);

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField("Value : ");
                switch (m_currentPlayerPrefType)
                {
                    case PlayerPrefType.String:
                        m_stringValue = EditorGUILayout.TextField(m_stringValue);
                        break;

                    case PlayerPrefType.Int:
                        m_intValue = EditorGUILayout.IntField(m_intValue);
                        break;

                    case PlayerPrefType.Float:
                        m_floatValue = EditorGUILayout.FloatField(m_floatValue);
                        break;
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    if (string.IsNullOrEmpty(m_keyValue))
                    {
                        EditorUtility.DisplayDialog("Error", "Key cannot be empty", "OK");
                        return;
                    }

                    SaveData();
                    ResetData();
                }

                if (GUILayout.Button("Clear", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    GUI.FocusControl(null);
                    ResetData();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        private void SaveData()
        {
            try
            {
                switch (m_currentPlayerPrefType)
                {
                    case PlayerPrefType.String:
                        m_playerPrefsSaveHandle.SavePlayerPrefs(m_keyValue, m_stringValue);
                        break;

                    case PlayerPrefType.Int:
                        m_playerPrefsSaveHandle.SavePlayerPrefs(m_keyValue, m_intValue);
                        break;

                    case PlayerPrefType.Float:
                        m_playerPrefsSaveHandle.SavePlayerPrefs(m_keyValue, m_floatValue);
                        break;
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "OK");
            }
            finally
            {
                OnCompleteAddData?.Invoke();
            }
        }

        private void ResetData()
        {
            m_keyValue = string.Empty;
            m_stringValue = string.Empty;
            m_intValue = 0;
            m_floatValue = 0.0f;
            m_intValue = 0;

            Repaint();
        }
    }
}
#endif
