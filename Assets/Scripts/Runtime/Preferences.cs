using UnityEngine;

namespace MJ198
{
    [CreateAssetMenu(fileName = "DefaultPreferences", menuName = "Preferences")]
    public class Preferences : ScriptableObject
    {
        public Texture2D Cursor;
        public float DialogueSpeedMul;
    }
}
