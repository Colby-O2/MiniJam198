using MJ198.Grid;
using MJ198.MonoSystems;
using MJ198.UI;
using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MJ198
{
    public class MJ198GameManager : GameManager
    {
        [SerializeField] GameObject _monoSystemHolder;

        [Header("MonoSystems")]
        [SerializeField] private UIMonoSystem _uiSystem;
        [SerializeField] private AnimationMonoSystem _animSystem;
        [SerializeField] private AudioMonoSystem _audioSystem;
        [SerializeField] private InputMonoSystem _inputSystem;
        [SerializeField] private GridMonoSystem _gridSystem;
        [SerializeField] private SpawnerMonoSystem _spawnerSystem;

        public static string Username;

        public static bool IsPaused = false;
        private static bool _lockMovement = false;

        public static Player.Manager Player;
        public static Preferences Preferences { get => (Instance as MJ198GameManager)._preferences; }
        [SerializeField] private Preferences _preferences;

        public static void UseCustomCursor()
        {
            if (Instance)
            {
                Cursor.SetCursor(Preferences.Cursor, Vector2.zero, CursorMode.Auto);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                UseCustomCursor();
            }
        }

        public static void HideCursor()
        {
            UseCustomCursor();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static void ShowCursor()
        {
            UseCustomCursor();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        private void AttachMonoSystems()
        {
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiSystem);
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animSystem);
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioSystem);
            AddMonoSystem<InputMonoSystem, IInputMonoSystem>(_inputSystem);
            AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_gridSystem);
            AddMonoSystem<SpawnerMonoSystem, ISpawnerMonoSystem>(_spawnerSystem);
        }

        public override string GetApplicationName()
        {
            return nameof(MJ198GameManager);
        }

        public override string GetApplicationVersion()
        {
            return "v0.0.1";
        }

        protected override void OnInitalized()
        {
            AttachMonoSystems();

            _monoSystemHolder.SetActive(true);
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {

        }

        private void OnSceneUnload(Scene scene)
        {
            RemoveAllEventListeners();
        }

        private void Awake()
        {
            Application.runInBackground = true;
        }

        private void Start()
        {
           Player = FindFirstObjectByType<Player.Manager>();
        }

        private static void UpdateScore()
        {
            if (!Player) return;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<RankingsView>().SetLeaderboardEntry(MJ198GameManager.Username, Player.GetScore());
        }

        public static void Restart()
        {
            UpdateScore();
            if (Player) Player.Restart();

            GameManager.GetMonoSystem<ISpawnerMonoSystem>().Restart();

            GameManager.GetMonoSystem<IGridMonoSystem>().Restart();

            Enemy.Manager[] enemies = FindObjectsByType<Enemy.Manager>(FindObjectsSortMode.None);
            Bullet[] bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None);

            foreach (Enemy.Manager enemy in enemies) Destroy(enemy.gameObject);
            foreach (Bullet bullet in bullets) Destroy(bullet.gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }

        public static void QuitGame()
        {
            UpdateScore();
            Application.Quit();
        }

        public static void StartGame()
        {
            Player.gameObject.SetActive(true);
            GameManager.GetMonoSystem<IGridMonoSystem>().SetRigidBodyState(true);
            MJ198GameManager.Restart();
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
        }
    }
}
