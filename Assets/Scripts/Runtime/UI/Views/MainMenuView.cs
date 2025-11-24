using MJ198.MonoSystems;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MJ198.UI
{
    public class RandomUsername
    {
        private static string[] adjectives = { "Swift", "Silent", "Crazy", "Mighty", "Sneaky", "Happy", "Dark", "Wild" };
        private static string[] nouns = { "Tiger", "Shadow", "Ninja", "Eagle", "Wizard", "Samurai", "Wolf", "Dragon" };

        public static string GenerateUsername()
        {
            string adj = adjectives[Random.Range(0, adjectives.Length)];
            string noun = nouns[Random.Range(0, nouns.Length)];
            int number = Random.Range(0, 1000);
            return $"{adj}{noun}{number}";
        }
    }

    public class MainMenuView : View
    {
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject _player;

        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _ranksing;
        [SerializeField] private EventButton _quit;

        [SerializeField] private Player.MovementSettings _playerSettings;
        [SerializeField] private Slider _volume;
        [SerializeField] private Slider _sensitivity;

        [SerializeField] private TMP_InputField _username;

        private bool _hasStarted = false;

        private float GetSensitivityAdjustedValue(float input, float exp = 2f)
        {
            return Mathf.Pow(input, exp);
        }

        private void OnSensitivityChanged(float val)
        {
            float sens = Mathf.Lerp(0.01f, 1f, GetSensitivityAdjustedValue(val));
            PlayerPrefs.SetFloat("Sensitivity", sens);
            _playerSettings.Sensitivity = sens;
        }

        private void OnVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("Volume", val);
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetOverallVolume(val);
        }

        private void Play()
        {
            MJ198GameManager.StartGame();
        }

        private void Rankings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<RankingsView>();
        }

        private void Quit()
        {
            MJ198GameManager.QuitGame();
        }

        private void OnUsernameChanged(string name)
        {
            MJ198GameManager.Username = name;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<RankingsView>().GetLeaderboard();
            PlayerPrefs.SetString("Username", MJ198GameManager.Username);
            PlayerPrefs.Save();
        }

        public override void Init()
        {
            _play.onPointerDown.AddListener(Play);
            _ranksing.onPointerDown.AddListener(Rankings);
            _quit.onPointerDown.AddListener(Quit);

            _volume.value = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume(); ;

            float sens = (PlayerPrefs.HasKey("Sensitivity")) ? PlayerPrefs.GetFloat("Sensitivity") : 0.5f;
            _playerSettings.Sensitivity = sens;
            _sensitivity.value = sens;

            _volume.onValueChanged.AddListener(OnVolumeChanged);
            _sensitivity.onValueChanged.AddListener(OnSensitivityChanged);

            string savedUsername = PlayerPrefs.GetString("Username");
            string defaultName = (savedUsername == string.Empty) ? RandomUsername.GenerateUsername() : savedUsername;
            _username.text = defaultName;
            MJ198GameManager.Username = defaultName;

            _username.onValueChanged.AddListener(OnUsernameChanged);
        }

        public override void Show()
        {
            base.Show();
            _cam.SetActive(true);
            GameManager.GetMonoSystem<IGridMonoSystem>().SetRigidBodyState(false);
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
