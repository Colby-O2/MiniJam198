using MJ198.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [SerializeField] private List<Tile> _tiles =new List<Tile>();

        private void Start()
        {
            _tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None).ToList();
        }

        public void Restart()
        {
            foreach (Tile tile in _tiles) tile.ResetTile();

        }

        public void SetRigidBodyState(bool state)
        {
            foreach (Tile tile in _tiles)
            {
                tile.SetRigidBodyState(state);
            }
        }

    }
}