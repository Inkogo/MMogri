using MMogri.Scripting;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Quest : ScriptableObject
    {
        public int Index;
        public string name;
        public string description;

        public string onAcceptCallback;
        public string onCompleteCallback;
    }
}
