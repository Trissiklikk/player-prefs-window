﻿#if UNITY_EDITOR_WIN
using System.Collections.Generic;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsSaveHandle : BaseSaveHandle
    {
        public override void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public override void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public override void SaveFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public override void Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
       
        /// <summary>
        /// Method for getting value from player prefs key.
        /// </summary>
        public override void GetValue(ref List<string> keys, ref List<PrefHolder> containList)
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
                    PrefHolder playerPrefHolder = new PrefHolder(keys[i]).WithStringValue(stringValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                int intValue = PlayerPrefs.GetInt(keys[i], ERROR_INT_KEY_MSG);

                if (intValue != ERROR_INT_KEY_MSG)
                {
                    PrefHolder playerPrefHolder = new PrefHolder(keys[i]).WithIntValue(intValue);
                    containList.Add(playerPrefHolder);

                    continue;
                }

                float floatValue = PlayerPrefs.GetFloat(keys[i], ERROR_FLOAT_KEY_MSG);

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
