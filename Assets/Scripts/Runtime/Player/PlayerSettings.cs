using UnityEngine;

namespace MJ198
{
    [CreateAssetMenu(fileName = "DefaultPlayerSettings", menuName = "Player/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public LayerMask TileLayer;
        public float MaxDstToTileForTrigger = 0.5f;
    }
}
