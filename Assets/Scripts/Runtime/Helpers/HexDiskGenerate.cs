using UnityEditor;
using UnityEngine;

namespace MJ198.Helpers
{
    [ExecuteInEditMode]
    public class HexDiskGenerate : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float _tileSize = 1f;
        [SerializeField] private GameObject _hexTilePrefab;
        [SerializeField] private int _gridRadius = 10;

        [Header("Hole Settings")]
        [SerializeField] private int _innerRadius = 3;

        [Header("Editor Options")]
        [SerializeField] private bool _autoRegenerate;

        public Vector3 HexToWorld(int q, int r)
        {
            float x = _tileSize * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
            float z = _tileSize * (3f / 2f * r);
            return new Vector3(x, 0f, z);
        }

        public Vector2Int WorldToHex(Vector3 worldPos)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / _tileSize;
            float r = (2f / 3f * worldPos.z) / _tileSize;

            float x = q;
            float z = r;
            float y = -x - z;

            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Vector2Int(rx, rz);
        }

        [ContextMenu("Generate Grid")]
        public void GenerateGrid()
        {
            if (!_hexTilePrefab)
            {
                Debug.LogWarning("Hex Tile Prefab not assigned.");
                return;
            }

            ClearGrid();

            for (int q = -_gridRadius; q <= _gridRadius; q++)
            {
                int r1 = Mathf.Max(-_gridRadius, -q - _gridRadius);
                int r2 = Mathf.Min(_gridRadius, -q + _gridRadius);

                for (int r = r1; r <= r2; r++)
                {
                    int dist = (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(-q - r)) / 2;

                    if (dist < _innerRadius)
                        continue;

                    Vector3 pos = HexToWorld(q, r);

#if UNITY_EDITOR
                    GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(_hexTilePrefab, transform);
                    tile.transform.position = pos;
                    tile.transform.rotation = Quaternion.identity;
                    tile.name = $"Hex_{q}_{r}";
#else
                Instantiate(_hexTilePrefab, pos, Quaternion.identity, transform);
#endif
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        [ContextMenu("Clear Grid")]
        public void ClearGrid()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
            }
        }

        private void OnValidate()
        {
            if (!_autoRegenerate || Application.isPlaying)
                return;

            GenerateGrid();
        }
    }
}
