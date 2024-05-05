#if UNITY_EDITOR_WIN

using System.Collections.Generic;

namespace Trissiklikk.EditorTools
{
    public abstract class BaseSaveHandle
    {
        protected const string ERROR_STRING_KEY_MSG = "ERROR_STRING_KEY";
        protected const int ERROR_INT_KEY_MSG = int.MinValue;
        protected const float ERROR_FLOAT_KEY_MSG = float.NaN;

        public abstract void SaveString(string key, string value);
        public abstract void SaveInt(string key, int value);
        public abstract void SaveFloat(string key, float value);
        public abstract void Remove(string key);
        public abstract void GetValue(ref List<string> keys, ref List<PrefHolder> containList);
    }
}
#endif
