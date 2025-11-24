using TMPro;
using UnityEngine;

namespace MJ198.UI
{
    public class LeaderbaordEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _score;
        [SerializeField] private TMP_Text _ranking;

        public void SetName(string name)
        {
            _name.text = name;
        }

        public void SetScore(int score)
        {
            _score.text = $"{score}";
        }

        public void SetRanking(int ranking)
        {
            _ranking.text = $"{ranking}.";
        }

        public void SetRanking(string ranking)
        {
            _ranking.text = $"{ranking}.";
        }

        public void SetEmpty()
        {
            _name.text = string.Empty;  
            _score.text = string.Empty;
            _ranking.text = string.Empty;
        }
    }
}
