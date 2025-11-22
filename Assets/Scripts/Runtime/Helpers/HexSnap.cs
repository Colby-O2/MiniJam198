using UnityEngine;
using UnityEditor;
using MJ198.Math;

namespace MJ198.Helpers
{
    [ExecuteInEditMode]
    public class HexSnap : MonoBehaviour
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private float _tileSize = 1f;

        private void Update()
        {
            if (_isEnabled && !Application.isPlaying)
            {
                Vector3 pos = transform.position;
                Vector3 snapPos = HexRound(WorldToHex(pos));
                transform.position = HexToWorld(snapPos).SetY(transform.position.y);
            }
        }

        private Vector3 HexRound(Vector3 hex)
        {
            float x = Mathf.Round(hex.x);
            float y = Mathf.Round(hex.y);
            float z = Mathf.Round(hex.z);

            float dx = Mathf.Abs(x - hex.x);
            float dy = Mathf.Abs(y - hex.y);
            float dz = Mathf.Abs(z - hex.z);

            if (dx > dy && dx > dz) x = -y - z;
            else if (dy > dz) y = -x - z;
            else z = -x - y;

            return new Vector3(x, y, z);
        }

        private Vector3 HexToWorld(Vector3 hex)
        {
            float x = _tileSize * Mathf.Sqrt(3f) * (hex.x + hex.y * 0.5f);
            float z = _tileSize * 1.5f * hex.y;
            return new Vector3(x, 0f, z);
        }

        private Vector3 WorldToHex(Vector3 pos)
        {
            float q = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.z) / _tileSize;
            float r = (2f / 3f * pos.z) / _tileSize;
            return new Vector3(q, r, -q - r);
        }
    }
}
