using PlazmaGames.Core.MonoSystem;
using UnityEngine;
using UnityEngine.Events;

namespace MJ198.MonoSystems
{
    public interface IInputMonoSystem : IMonoSystem
    {
        public UnityEvent JumpAction { get; }
        public UnityEvent SprintAction { get; }
        public UnityEvent SlideAction { get; }

        public bool SprintHeld { get; set; }
        public Vector2 RawMovement { get; }
        public Vector2 RawLook { get; }
    }
}
