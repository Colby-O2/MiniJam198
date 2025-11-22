using System.Collections.Generic;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [SerializeField] private float _tileSize;

        [SerializeField] private GameObject hexTilePrefab;
        [SerializeField] private int gridRadius = 10;
        [SerializeField] private float hexSize = 1f;

        private void Start()
        {
            GenerateGrid();
        }

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

        private void GenerateGrid()
        {
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                int r2 = Mathf.Min(gridRadius, -q + gridRadius);

                for (int r = r1; r <= r2; r++)
                {
                    Vector3 pos = HexToWorld(q, r);
                    GameObject tile = Instantiate(hexTilePrefab, pos, Quaternion.identity, transform);

                    float dist = new Vector2(q, r).magnitude;
                    float t = dist / gridRadius;
                }
            }
        }
    }
}