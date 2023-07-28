using System;
using System.Threading;
using SpaceShip.Services;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.UI.Components
{
    public class LobbyPreview : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _playersText;
        [SerializeField] private Button _button;

        private const int REFRESH_INTERVAL = 5;
        private CancellationTokenSource _refreshSource;

        private Action<string> _callback;
        private string _lobbyId = string.Empty;
        private bool _isInit;

        private float _timer;
        
        public void Init(string id, Action<string> callBack)
        {
            _lobbyId = id;
            _callback = callBack;
            _isInit = true;
        }
        
        private void UpdateView(Lobby lobby)
        {
            _nameText.text = lobby.Name;
            _playersText.text = $"players: {lobby.Players.Count} / {lobby.MaxPlayers}";
        }

        private async void Update()
        {
            _timer -= Time.deltaTime;
            
            if (_isInit && _timer < 0)
            {
                _timer = REFRESH_INTERVAL;
                var lobby = await MatchmakingService.PullLobby(_lobbyId);
                if (lobby != null)
                {
                    UpdateView(lobby);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        public void OnClick()
        {
            _callback?.Invoke(_lobbyId);
        }

        private void OnDestroy()
        {
            _callback = null;
        }
    }
}