using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public interface IGridMonoSystem : IMonoSystem
    {
        public Vector3 HexToWorld(int q, int r);
        public Vector2Int WorldToHex(Vector3 worldPos);
    }
}

