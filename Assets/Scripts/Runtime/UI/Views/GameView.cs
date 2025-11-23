using MJ198.Math;
using PlazmaGames.UI;
using TMPro;
using UnityEngine;

namespace MJ198
{
    public class GameView : View
    {
        [SerializeField] private GameObject _healthBar;
        [SerializeField] private TMP_Text _health;
        [SerializeField] private TMP_Text _score;

        public void SetHealth(float health, float maxHealth)
        {
            _health.text = $"{Mathf.RoundToInt(health)}";
            _healthBar.transform.localScale = _healthBar.transform.localScale.SetX(health/maxHealth);
        }

        public void SetScore(int score, int scoreMul)
        {
            _score.text = $"Score: {score}\n{scoreMul}X";
        }

        public override void Init()
        {

        }
    }
}
