#if UNITY_EDITOR_WIN
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsElementUtility
    {
        private const float MAX_WIDTH_PRECENT = 40;
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

            using (new EditorGUILayout.HorizontalScope())
            {
                if (onRemoveAction != null)
                {
                    if (GUILayout.Button("x"))
                    {
                        onRemoveAction.Invoke(key);
                    }
                }

                float width = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.LabelField(key, lableStyle, GUILayout.Width((MAX_WIDTH_PRECENT / 100) * width));
                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup(type, enumOptions);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.TextArea(value, textAreaOptions);

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space(5);
        }
    }
}
#endif
