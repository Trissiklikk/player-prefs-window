#if UNITY_EDITOR_WIN
using System;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsElementUtility
    {
        /// <summary>
        /// This for create a data field for key and value.
        /// </summary>
        /// <param name="key">String of key name</param>
        /// <param name="type">The type of player prefs</param>
        /// <param name="value">Value of player prefs key</param>
        /// <param name="onRemoveAction">The action of call when remove action</param>
        public void KeyDataField(string key, PlayerPrefType type, string value, Action<string> onRemoveAction = null)
        {
            value = value.Replace("\n", string.Empty);

            GUILayoutOption[] textAreaOptions = new GUILayoutOption[]
            {
               GUILayout.Width(300),
               GUILayout.Height(20),
               GUILayout.ExpandWidth(true),
            };

            GUILayoutOption[] enumOptions = new GUILayoutOption[]
            {
               GUILayout.Width(75),
            };

            GUIStyle lableStyle = new GUIStyle(GUI.skin.label);
            lableStyle.fontSize = 12;
            lableStyle.fontStyle = FontStyle.Bold;
            lableStyle.normal.textColor = Color.white;

            GUIStyle lableStyled = new GUIStyle(GUI.skin.horizontalScrollbar);
            lableStyled.margin = new RectOffset(2, 2, 5, 5);

            EditorGUILayout.BeginHorizontal(lableStyled);
            EditorGUILayout.LabelField(key, lableStyle);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(type, enumOptions);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.TextArea(value, textAreaOptions);

            if(onRemoveAction != null)
            {
                if (GUILayout.Button("x"))
                {
                    onRemoveAction.Invoke(key);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }
}
#endif
