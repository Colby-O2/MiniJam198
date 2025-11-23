using MJ198.Grid;
using MJ198.Player;
using PlazmaGames.Attribute;
using Unity.VisualScripting;
using UnityEngine;

namespace MJ198.Player
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private PlayerSettings _settings;
        [SerializeField] private Controller _controller;
        [SerializeField] private HealthTaker _healthTaker;

        [Header("Stats")]
        [SerializeField, Min(1)] private int _maxScoreMultiplier = 5;
        [SerializeField] private float _timeForScoreMultiplier = 15f;

        [Header("Current Stats")]
        [SerializeField, ReadOnly] int _scoreMultiplier = 1;
        [SerializeField, ReadOnly] private int _currentScore = 0;

        private Collider[] _hits = new Collider[10];

        private void Awake()
        {
            if (!_controller)  _controller = GetComponent<Controller>();
            if (!_healthTaker) _healthTaker = GetComponent<HealthTaker>();

            _healthTaker.OnDeath.AddListener(OnDeath);
            ResetScore();
        }

        private void OnDeath()
        {
            Debug.Log("You Died.");
        }

        private void Update()
        {
            _scoreMultiplier = Mathf.Clamp(1 + Mathf.FloorToInt(_controller.GetTimeOffGround() / _timeForScoreMultiplier), 1, _maxScoreMultiplier);
        }

        private void FixedUpdate()
        {
            if (_controller.State == Controller.PlayerState.Grounded || _controller.State == Controller.PlayerState.Sliding) CheckTile();
        }

        public void AddScore(int amount)
        {
            _currentScore += amount * _scoreMultiplier;
        }

        public void ResetScore()
        {
            _currentScore = 0;
        }

        private void CheckTile()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _settings.MaxDstToTileForTrigger,
                _hits,
                _settings.TileLayer
            );

            for (int i = 0; i < count; i++)
            {
                if(_hits[i].transform.TryGetComponent<Tile>(out Tile tile)) tile.MarkForDecay();
            }
        }
    }
}
