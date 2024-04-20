#if UNITY_EDITOR_WIN
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefsSaveHandle
    {
        public void SavePlayerPrefs(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void SavePlayerPrefs(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SavePlayerPrefs(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void RemovePlayerPrefs(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
#endif
