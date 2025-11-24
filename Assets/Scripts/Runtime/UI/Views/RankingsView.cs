using Dan.Main;
using Dan.Models;
using MJ198.MonoSystems;
using NUnit.Framework;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MJ198.UI
{
    public class RankingsView : View
    {
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject _player;

        private string publicKey = "80520e9c2f887cf4f04ed509f21f0f1f2e3c2b5c209443d524dd24a6cbd74001";

        [SerializeField] private int _numRankings = 0;
        [SerializeField] private LeaderbaordEntry _yourEntry;
        [SerializeField] private GameObject _entryParnet;
        [SerializeField] private LeaderbaordEntry _entryPrefab;
        [SerializeField, ReadOnly] private List<LeaderbaordEntry> _entries = new List<LeaderbaordEntry>();

        [SerializeField] private EventButton _menu;
        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _quit;

        [SerializeField] private TMP_Text _playLabel;

        [SerializeField] private int _bestScore;

        private bool _hasStarted = false;

        private void Play()
        {
            MJ198GameManager.StartGame();
        }

        private void Menu()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
        }

        private void Quit()
        {
            MJ198GameManager.QuitGame();
        }

        public override void Init()
        {
            foreach (Transform t in _entryParnet.transform) Destroy(t.gameObject);

            LeaderboardCreator.ResetPlayer();

            _menu.onPointerDown.AddListener(Menu);
            _play.onPointerDown.AddListener(Play);
            _quit.onPointerDown.AddListener(Quit);

            GetLeaderboard();
        }

        public override void Show()
        {
            _playLabel.text = (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<MainMenuView>()) ? "Play" : "Play Again";
            base.Show();
            GameManager.GetMonoSystem<IGridMonoSystem>().SetRigidBodyState(false);
            GetLeaderboard();
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

        public void GetLeaderboard()
        {
            LeaderboardCreator.GetLeaderboard(publicKey, false,  msg =>
            {
                foreach (LeaderbaordEntry e in _entries) if (e) Destroy(e.gameObject);

                for (int i = 0; i <_numRankings; i++)
                {
                    LeaderbaordEntry entry = Instantiate<LeaderbaordEntry>(_entryPrefab);
                    if (msg.Length <= i)
                    {
                        entry.transform.SetParent(_entryParnet.transform);
                        entry.SetEmpty();
                    }
                    else
                    {
                        entry.transform.SetParent(_entryParnet.transform);
                        entry.SetRanking(i + 1);
                        entry.SetName(msg[i].Username);
                        entry.SetScore(msg[i].Score);
                    }

                    _entries.Add(entry);
                }

                Entry you = msg.FirstOrDefault(e => e.Username == MJ198GameManager.Username);

                _yourEntry.SetRanking("You");
                _yourEntry.SetName(MJ198GameManager.Username);
                _yourEntry.SetScore(you.Score);
                _bestScore = you.Score;
            });
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            if (_bestScore >= score) return;
            LeaderboardCreator.UploadNewEntry(publicKey, username, score, msg =>
            {
                GetLeaderboard();
            }, e => {
                Debug.Log($"Error  : {e}");
            });
        }

        private void Update()
        {
            if (!_hasStarted)
            {
                _hasStarted = true;
            }
        }
    }
}