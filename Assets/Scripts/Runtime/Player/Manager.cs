using MJ198.Grid;
using MJ198.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace MJ198
{

    public class Manager : MonoBehaviour
    {
        [SerializeField] private PlayerSettings _settings;
        [SerializeField] private Controller _controller;

        private RaycastHit[] _hits = new RaycastHit[1];

        private void Awake()
        {
            if (!_controller)  _controller = GetComponent<Controller>();
        }

        private void CheckTile()
        {
            int count = Physics.RaycastNonAlloc(
                transform.position,
                -transform.up,
                _hits,
                _settings.MaxDstToTileForTrigger,
                _settings.TileLayer
            );

            if (count > 0)
            {
                Debug.Log(_hits[0].transform.name);
                if(_hits[0].transform.TryGetComponent<Tile>(out Tile tile)) tile.MarkForDecay();
            }
        }

        private void FixedUpdate()
        {
            if (_controller.State == Controller.PlayerState.Grounded || _controller.State == Controller.PlayerState.Sliding) CheckTile();
        }
    }
}
