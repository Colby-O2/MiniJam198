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

        private float _decayStartTime;
        private float _fallStartTime;

        private void Awake()
        {
            _baseColor = _renderer.materials[_materialIdx].color;
            _startRot = transform.rotation;
            _startPos = transform.position;
        }

        private void OnEnable()
        {
            if (_markedForDecay && !_markedForFall)
            {
                _timer = Time.time - _decayStartTime;
                UpdateDecay(_timer);
            }
            else if (_markedForFall)
            {
                _timer = Time.time - _fallStartTime;
                UpdateFall(_timer);
            }
        }

        private void Update()
        {
            if (_markedForDecay && !_markedForFall)
            {
                _timer += Time.deltaTime;
                UpdateDecay(_timer);
            }

            if (_markedForFall)
            {
                _timer += Time.deltaTime;
                UpdateFall(_timer);
            }
        }

        public void MarkForDecay()
        {
            if (_markedForDecay) return;
            _markedForDecay = true;
            _decayStartTime = Time.time;
        }

        private void UpdateDecay(float timer)
        {
            _renderer.materials[_materialIdx].color = Color.Lerp(
                _baseColor,
                _decayColor,
                _startColorPercent + (timer / ((1f / _startColorPercent) * _decayTime))
            );

            if (timer >= _decayTime)
            {
                _timer = 0f;
                _markedForFall = true;
                _fallStartTime = Time.time;
            }
        }

        private void UpdateFall(float timer)
        {
            transform.position = transform.position.SetY(_startPos.y - _fallDst * timer / _fallTime);

            if (timer >= _fallTime)
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
    }
}
