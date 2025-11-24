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
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;

        [Header("Decay Settings")]
        [SerializeField] private float _decayTime;

        [Header("Color Settings")]
        [SerializeField] private int _materialIdx;
        [SerializeField] private float _startColorPercent = 0.25f;
        [SerializeField, ColorUsage(true, true)] private Color _decayColor;
        [SerializeField, ColorUsage(true, true), ReadOnly] private Color _baseColor = Color.black;

        [Header("Fall Settings")]
        [SerializeField] private float _fallTime = 5f;
        [SerializeField] private float _downTime = 19f;
        [SerializeField] private float _returnTime = 1f;
        [SerializeField] private float _fallDst = 100f;

        [SerializeField, ReadOnly] private bool _markedForFall;
        [SerializeField, ReadOnly] private bool _markedForDecay;
        [SerializeField, ReadOnly] private bool _markedForWait;
        [SerializeField, ReadOnly] private bool _markedForReturn;
        [SerializeField, ReadOnly] private float _timer;

        [SerializeField, ReadOnly] private Vector3 _startPos;
        [SerializeField, ReadOnly] private Quaternion _startRot;

        private float _decayStartTime;
        private float _fallStartTime;
        private float _returnStartTIme;

        private void Awake()
        {
            if (!_collider) _collider = GetComponent<Collider>();
            if (!_rb) _rb = GetComponent<Rigidbody>();
            _baseColor = Color.black;
            _startRot = transform.rotation;
            _startPos = transform.position;

            _renderer.material.EnableKeyword("_EMISSION");
        }

        private void OnEnable()
        {
            if (_markedForDecay && !_markedForFall)
            {
                _timer = Time.time - _decayStartTime;
                UpdateDecay(_timer);
            }
            else if (_markedForFall && !_markedForReturn)
            {
                _timer = Time.time - _fallStartTime;
                UpdateFall(_timer);
            }
            else if (_markedForReturn)
            {
                _timer += Time.deltaTime;
                UpdateReturn(_timer);
            }
        }

        private void Update()
        {
            if (_markedForDecay && !_markedForFall)
            {
                _timer += Time.deltaTime;
                UpdateDecay(_timer);
            }
            else if (_markedForFall && !_markedForReturn)
            {
                _timer += Time.deltaTime;
                UpdateFall(_timer);
            }
            else if (_markedForReturn)
            {
                _timer += Time.deltaTime;
                UpdateReturn(_timer);
            }
        }

        public void SetRigidBodyState(bool state)
        {
            _rb.isKinematic = state;
        }

        public void MarkForDecay()
        {
            if (_markedForDecay) return;
            _markedForDecay = true;
            _decayStartTime = Time.time;
        }

        private void UpdateDecay(float timer)
        {
            _renderer.materials[_materialIdx].SetColor("_EmissionColor", Color.Lerp(
                _baseColor,
                _decayColor,
                _startColorPercent + (timer / ((1f / _startColorPercent) * _decayTime))
            ));

            if (timer >= _decayTime)
            {
                _timer = 0f;
                _markedForFall = true;
                _fallStartTime = Time.time;
            }
        }

        private void UpdateFall(float timer)
        {

            if (_collider.enabled) _collider.enabled = false;
            transform.position = transform.position.SetY(_startPos.y - _fallDst * timer / _fallTime);

            if (timer >= _fallTime)
            {
                _timer = 0;
                _markedForReturn = true;
                _renderer.enabled = false;
            }
        }

        private void UpdateReturn(float timer)
        {
            if (timer < _downTime) return;

            if (!_renderer.enabled) _renderer.enabled = true;
            transform.position = transform.position.SetY(_startPos.y - _fallDst + _fallDst * (timer - _downTime) / _returnTime);

            if (timer >= _returnTime + _downTime)
            {
                ResetTile();
            }
        }

        public void ResetTile()
        {
            _timer = 0;
            _collider.enabled = true;
            _renderer.enabled = true;
            transform.position = _startPos;
            transform.rotation = _startRot;
            _renderer.materials[_materialIdx].SetColor("_EmissionColor", _baseColor);
            _markedForDecay = false;
            _markedForFall = false;
            _markedForReturn = false;
            _decayStartTime = 0f;
            _fallStartTime = 0f;
            _returnStartTIme = 0f;
        }
    }
}
