using MJ198.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace MJ198.MonoSystems
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [SerializeField] private float _tileSize;
        [SerializeField] private GameObject hexTilePrefab;
        [SerializeField] private int gridRadius = 10;

        private void Start()
        {
            //GenerateGrid();
        }

        public Vector3 HexToWorld(int q, int r)
        {
            float x = _tileSize * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
            float z = _tileSize * (3f / 2f * r);
            return new Vector3(x, 0f, z);
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