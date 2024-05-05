#if UNITY_EDITOR_WIN

using System.Collections.Generic;
using UnityEditor;

namespace Trissiklikk.EditorTools
{
    public sealed class EditorPrefsSaveHandle : BaseSaveHandle
    {
        public override void SaveString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public override void SaveInt(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        public override void SaveFloat(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        public override void Remove(string key)
        {
            EditorPrefs.DeleteKey(key);
        }

        public override void GetValue(ref List<string> keys, ref List<PrefHolder> containList)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (!EditorPrefs.HasKey(keys[i]))
                {
                    continue;
                }

                string stringValue = EditorPrefs.GetString(keys[i], ERROR_STRING_KEY_MSG);

                if (stringValue != ERROR_STRING_KEY_MSG)
                {
                    PrefHolder playerPrefHolder = new PrefHolder(keys[i]).WithStringValue(stringValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                int intValue = EditorPrefs.GetInt(keys[i], ERROR_INT_KEY_MSG);

                if (intValue != ERROR_INT_KEY_MSG)
                {
                    PrefHolder playerPrefHolder = new PrefHolder(keys[i]).WithIntValue(intValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                float floatValue = EditorPrefs.GetFloat(keys[i], ERROR_FLOAT_KEY_MSG);

                if (floatValue != ERROR_FLOAT_KEY_MSG)
                {
                    PrefHolder playerPrefHolder = new PrefHolder(keys[i]).WithFloatValue(floatValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }
            }
        }
    }
}
#endif
