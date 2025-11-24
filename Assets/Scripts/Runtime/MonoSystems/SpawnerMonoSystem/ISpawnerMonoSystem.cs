using PlazmaGames.Core.MonoSystem;

using UnityEngine;

namespace MJ198.MonoSystems
{
    public interface ISpawnerMonoSystem : IMonoSystem
    {
        public void SetPlayer(Player.Controller player);
        public void Restart();
    }
}

