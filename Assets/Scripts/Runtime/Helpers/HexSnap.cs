#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MJ198.Math;

namespace MJ198.Helpers
{
    [ExecuteInEditMode]
    public class HexSnap : MonoBehaviour
    {
        public enum SnapPlane { XZ, XY, YZ }

        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private float _tileSize = 1f;
        [SerializeField] private SnapPlane _plane = SnapPlane.XZ;

        private void Update()
        {
            if (_isEnabled && !Application.isPlaying)
            {
                Vector3 pos = transform.position;
                Vector3 hex = WorldToHex(pos);
                Vector3 rounded = HexRound(hex);
                Vector3 snappedWorld = HexToWorld(rounded);

                switch (_plane)
                {
                    case SnapPlane.XZ: snappedWorld.y = pos.y; break;
                    case SnapPlane.XY: snappedWorld.z = pos.z; break;
                    case SnapPlane.YZ: snappedWorld.x = pos.x; break;
                }

                transform.position = snappedWorld;
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

        private Vector3 WorldToHex(Vector3 pos)
        {
            switch (_plane)
            {
                case SnapPlane.XY:
                    float qXY = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.y) / _tileSize;
                    float rXY = (2f / 3f * pos.y) / _tileSize;
                    return new Vector3(qXY, rXY, -qXY - rXY);

                case SnapPlane.YZ:
                    float qYZ = (Mathf.Sqrt(3f) / 3f * pos.y - 1f / 3f * pos.z) / _tileSize;
                    float rYZ = (2f / 3f * pos.z) / _tileSize;
                    return new Vector3(qYZ, rYZ, -qYZ - rYZ);

                default: 
                    float qXZ = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.z) / _tileSize;
                    float rXZ = (2f / 3f * pos.z) / _tileSize;
                    return new Vector3(qXZ, rXZ, -qXZ - rXZ);
            }
        }

        private Vector3 HexToWorld(Vector3 hex)
        {
            switch (_plane)
            {
                case SnapPlane.XY:
                    float xXY = _tileSize * Mathf.Sqrt(3f) * (hex.x + hex.y * 0.5f);
                    float yXY = _tileSize * 1.5f * hex.y;
                    return new Vector3(xXY, yXY, 0f);

                case SnapPlane.YZ:
                    float yYZ = _tileSize * Mathf.Sqrt(3f) * (hex.x + hex.y * 0.5f);
                    float zYZ = _tileSize * 1.5f * hex.y;
                    return new Vector3(0f, yYZ, zYZ);

                default: // XZ
                    float xXZ = _tileSize * Mathf.Sqrt(3f) * (hex.x + hex.y * 0.5f);
                    float zXZ = _tileSize * 1.5f * hex.y;
                    return new Vector3(xXZ, 0f, zXZ);
            }
        }
    }
}
#endif