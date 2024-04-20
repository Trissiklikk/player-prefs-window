#if UNITY_EDITOR_WIN

namespace Trissiklikk.EditorTools
{
    public sealed class PlayerPrefHolder
    {
        public string Key { get; private set; }
        public PlayerPrefType Type { get; private set; }
        public string StringValue { get; private set; }
        public int IntValue { get; private set; }
        public float FloatValue { get; private set; }

        public PlayerPrefHolder(string key)
        {
            Key = key;
        }

        public PlayerPrefHolder WithStringValue(string value)
        {
            Type = PlayerPrefType.String;
            StringValue = value;
            return this;
        }

        public PlayerPrefHolder WithIntValue(int value)
        {
            Type = PlayerPrefType.Int;
            IntValue = value;
            return this;
        }

        public PlayerPrefHolder WithFloatValue(float value)
        {
            Type = PlayerPrefType.Float;
            FloatValue = value;
            return this;
        }
    }
}
#endif
