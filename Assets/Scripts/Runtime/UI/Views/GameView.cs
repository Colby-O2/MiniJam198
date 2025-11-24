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
            _score.text = $"{scoreMul}X\nScore: {score}";
        }

        public override void Init()
        {

        }

        public override void Show()
        {
            base.Show();
            MJ198GameManager.HideCursor();
        }
    }
}
