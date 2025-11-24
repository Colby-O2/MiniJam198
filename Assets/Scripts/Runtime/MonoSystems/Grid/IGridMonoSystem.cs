using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public interface IGridMonoSystem : IMonoSystem
    {
        public void SetRigidBodyState(bool state);

        public void Restart();
    }
}

