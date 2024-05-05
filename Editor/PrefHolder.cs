#if UNITY_EDITOR_WIN

namespace Trissiklikk.EditorTools
{
    public sealed class PrefHolder
    {
        public string Key { get; private set; }
        public PlayerPrefType Type { get; private set; }
        public string StringValue { get; private set; }
        public int IntValue { get; private set; }
        public float FloatValue { get; private set; }

        public PrefHolder(string key)
        {
            Key = key;
        }

        public PrefHolder WithStringValue(string value)
        {
            Type = PlayerPrefType.String;
            StringValue = value;
            return this;
        }

        public PrefHolder WithIntValue(int value)
        {
            Type = PlayerPrefType.Int;
            IntValue = value;
            return this;
        }

        public PrefHolder WithFloatValue(float value)
        {
            Type = PlayerPrefType.Float;
            FloatValue = value;
            return this;
        }
    }
}
#endif
