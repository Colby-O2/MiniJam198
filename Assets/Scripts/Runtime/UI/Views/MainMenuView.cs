using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace MJ198.UI
{
    public class MainMenuView : View
    {
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject _player;

        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _ranksing;
        [SerializeField] private EventButton _quit;

        private bool _hasStarted = false;


        private void Play()
        {
            _player.SetActive(true);
            MJ198GameManager.Restart();
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
        }

        private void Rankings()
        {

        }

        private void Quit()
        {
            Application.Quit();
        }

        public override void Init()
        {
            _play.onPointerDown.AddListener(Play);
            _ranksing.onPointerDown.AddListener(Rankings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            _cam.SetActive(true);
            if (_hasStarted) _player.SetActive(false);
            MJ198GameManager.ShowCursor();
        }

        public override void Hide()
        {
            base.Hide();
            _cam.SetActive(false);
            MJ198GameManager.HideCursor();
        }

        private void Update()
        {
            if (!_hasStarted)
            {
                _hasStarted = true;
                _player.SetActive(false);
            }
        }
    }
}
