using MJ198.Math;
using NUnit.Framework.Internal.Filters;
using PlazmaGames.Attribute;
using System.Collections;
using UnityEngine;

namespace MJ198.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        [Header("Decay Settings")]
        [SerializeField] private float _decayTime;

        [Header("Color Settings")]
        [SerializeField] private int _materialIdx;
        [SerializeField] private float _startColorPercent = 0.25f;
        [SerializeField] private Color _decayColor;
        [SerializeField, ReadOnly] private Color _baseColor;

        [Header("Fall Settings")]
        [SerializeField] private float _fallTime = 5f;
        [SerializeField] private float _fallDst = 100f;

        [SerializeField, ReadOnly] private bool _markedForFall;
        [SerializeField, ReadOnly] private bool _markedForDecay;
        [SerializeField, ReadOnly] private float _timer;

        [SerializeField, ReadOnly] private Vector3 _startPos;
        [SerializeField, ReadOnly] private Quaternion _startRot;

        private void Awake()
        {
            _baseColor = _renderer.materials[_materialIdx].color;

            _startRot = transform.rotation;
            _startPos = transform.position;
        }

        private void Update()
        {
            SetTileDecay();
            HandleFall();
        }

        public void MarkForDecay()
        {
            _markedForDecay = true;


        }

        private void HandleFall()
        {
            if (!_markedForFall) return;

            _timer += Time.deltaTime;
            transform.position = transform.position.SetY(_startPos.y - _fallDst * _timer / _fallTime);

            if (_timer  > _fallTime)
            {
                gameObject.SetActive(false);
            }
        }

        private void ResetTile()
        {
            _timer = 0;
            transform.position = _startPos;
            transform.rotation = _startRot;
            _renderer.materials[_materialIdx].color = _baseColor;
            _markedForDecay = false;
            _markedForFall = false;
        }

        private void SetTileDecay()
        {
            if (!_markedForDecay || _markedForFall) return;

            _timer += Time.deltaTime;

            _renderer.materials[_materialIdx].color = Color.Lerp(_baseColor, _decayColor, _startColorPercent + (_timer / ((1f / _startColorPercent) * _decayTime)));

            if (_timer > _decayTime)
            {
                _timer = 0f;
                _markedForFall = true;
            }
        }
    }
}
