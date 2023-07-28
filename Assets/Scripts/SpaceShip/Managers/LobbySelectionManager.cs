using System.Collections.Generic;
using System.Linq;
using Ship.Utils;
using SpaceShip.Models;
using SpaceShip.Services;
using SpaceShip.UI.Controllers;
using SpaceShip.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.Managers
{
    public class LobbySelectionManager : SingletonNetwork<LobbySelectionManager>
    {
        [SerializeField] private ShipConfigSO[] _avalibleShipRange;
        [SerializeField] private ShipSelectionController _mainSelectionControllers; 
        [SerializeField] private ShipSelectionController _secondSelectionControllers; 
        [SerializeField] private GameObject _playerPrefab;

        private Dictionary<ulong, int> _bufferPlayerSelectedShip = new ();
        private Dictionary<ulong, int> _playerSelectedShip = new ();
        
        private Dictionary<ulong, bool> _bufferPlayerStatus = new();
        private Dictionary<ulong, bool> _playerStatus = new();
        private Dictionary<ulong, LobbyPlayer> _players = new();

        private bool IsAllReady => _players.Count > 1 && _playerStatus.Values.All((value) => value);
        private const int GAME_RUN_INTERVAL = 3;
        private float _timer = GAME_RUN_INTERVAL;
        
        private void Update()
        {
            if (IsServer)
            {
                if (IsAllReady)
                {
                    _timer -= Time.deltaTime;
                    if (_timer < 0)
                    {
                        StartGame();
                    }
                }
                else
                {
                    _timer = GAME_RUN_INTERVAL;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnectsHandler;
            }
        }

        void OnDisable()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnectsHandler;
            }
        }

        public void ServerSceneInit(ulong clientId)
        {
            // exec on server 
            var player = NetworkObjectSpawner
                .SpawnClientOwnershipNetworkObject(_playerPrefab,clientId,true);
            _playerSelectedShip[clientId] = 0;
            _playerStatus[clientId] = false;
            _players[clientId] = player.GetComponent<LobbyPlayer>();
            PropagateAll();
            NotifyConnectedPlayerClientRpc(clientId);
        }

        #region Change
        
        public void SetPlayerSelectedShip(ulong clientId, int index)
        {
            _playerSelectedShip[clientId] = index;
            PropagatePlayerShip();
        }
        
        public void SetPlayerState(ulong clientId, bool isReady)
        {
            _playerStatus[clientId] = isReady;
            PropagatePlayerStatus();
        }

        #endregion

        #region Propagate All
        
        private void PropagateAll()
        {
            PropagateAllStartClientRpc();
            foreach (var (key, value) in _playerSelectedShip)
            {
                UpdatePlayerShipClientRpc(key, value);
            }
            foreach (var (key, value) in _playerStatus)
            {
                UpdatePlayerStatusClientRpc(key, value);
            }

            PropagateAllFinishClientRpc();
            PlayerNotifyChangeClientRpc();
        }
        
        [ClientRpc]
        private void PropagateAllStartClientRpc()
        {
            _bufferPlayerSelectedShip.Clear();
            _bufferPlayerStatus.Clear();
        }

        [ClientRpc]
        private void PropagateAllFinishClientRpc()
        {
            _playerSelectedShip.Clear();
            foreach (var (key, value) in _bufferPlayerSelectedShip)
            {
                _playerSelectedShip.Add(key, value);
            }
            _bufferPlayerSelectedShip.Clear();
            
            _playerStatus.Clear();
            foreach (var (key, value) in _bufferPlayerStatus)
            {
                _playerStatus.Add(key, value);
            }
            _bufferPlayerStatus.Clear();
        }

        #endregion
        
        #region Propagate Selected Ship
        
        private void PropagatePlayerShip()
        {
            PropagatePlayerShipStartClientRpc();
            foreach (var (key, value) in _playerSelectedShip)
            {
                UpdatePlayerShipClientRpc(key, value);
            }

            PropagatePlayerShipFinishClientRpc();
            PlayerNotifyChangeClientRpc();
        }
        
        [ClientRpc]
        private void PropagatePlayerShipStartClientRpc()
        {
            _bufferPlayerSelectedShip.Clear();
        }
        
        [ClientRpc]
        private void UpdatePlayerShipClientRpc(ulong clientId, int shipIndex)
        {
            _bufferPlayerSelectedShip.Add(clientId, shipIndex);
        }
        
        [ClientRpc]
        private void PropagatePlayerShipFinishClientRpc()
        {
            _playerSelectedShip.Clear();
            foreach (var (key, value) in _bufferPlayerSelectedShip)
            {
                _playerSelectedShip.Add(key, value);
            }
            _bufferPlayerStatus.Clear();
        }
        
        #endregion
        
        #region Propagate Status
        
        private void PropagatePlayerStatus()
        {
            PropagatePlayerStatusStartClientRpc();
            foreach (var (key, value) in _playerStatus)
            {
                UpdatePlayerStatusClientRpc(key, value);
            }

            PropagatePlayerStatusFinishClientRpc();
            PlayerNotifyChangeClientRpc();
        }
        
        [ClientRpc]
        private void PropagatePlayerStatusStartClientRpc()
        {
            _bufferPlayerStatus.Clear();
        }
        
        [ClientRpc]
        private void UpdatePlayerStatusClientRpc(ulong clientId, bool isRedy)
        {
            _bufferPlayerStatus.Add(clientId, isRedy);
        }
        
        [ClientRpc]
        private void PropagatePlayerStatusFinishClientRpc()
        {
            _playerStatus.Clear();
            foreach (var (key, value) in _bufferPlayerStatus)
            {
                _playerStatus.Add(key, value);
            }
            _bufferPlayerStatus.Clear();
        }
        
        #endregion

        #region Disconent remove
        private void OnPlayerDisconnectsHandler(ulong clientId)
        {
            if (clientId != 0)
            {
                _players.Remove(clientId);
                _playerStatus.Remove(clientId);
                _playerSelectedShip.Remove(clientId);
                PropagateAll();
                NotifyConnectedPlayerClientRpc(clientId);
            }
            
            if (clientId == 0)
            {
                ShutDown();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RemoveClientServerRpc(ulong clientId)
        {
            if (clientId == 0)
            {
                var clientsId = _playerSelectedShip.Keys.Where((id) => id != clientId).ToArray();
                var allExceptHostClientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = clientsId
                    }
                };
                ShutdownClientRpc(allExceptHostClientRpcParams);
                
                var hostClientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { clientId }
                    }
                };
                ShutdownClientRpc(hostClientRpcParams);
            }
            else
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { clientId }
                    }
                };
                ShutdownClientRpc(clientRpcParams);
            }
                
        }
        
        [ClientRpc]
        private void ShutdownClientRpc(ClientRpcParams clientRpcParams = default)
        {
            ShutDown();
        }
        #endregion

        #region Notify

        [ClientRpc]
        private void NotifyConnectedPlayerClientRpc(ulong clientId)
        {
            if (_playerStatus.Count != _players.Count)
            {
                _players.Clear();
                var playerCollection = FindObjectsOfType<LobbyPlayer>();
                foreach (var lobbyPlayer in playerCollection)
                {
                    _players.Add(lobbyPlayer.OwnerClientId, lobbyPlayer);
                }
            }

            foreach (var (key, value) in _players)
            {
                value.Init(key);
                value.UpdateView(_playerSelectedShip[key], _playerStatus[key]);
                
            }
        }
        
        [ClientRpc]
        private void PlayerNotifyChangeClientRpc()
        {
            foreach (var (key, value) in _players)
            {
                value.UpdateView(_playerSelectedShip[key], _playerStatus[key]); 
            }
        }

        [ClientRpc]
        private void StartGameClientRpc()
        {
            SceneLoader.Instance.SelectedShip = _playerSelectedShip;
        }
        
        #endregion
        
        #region Utils

        private void StartGame()
        {
            StartGameClientRpc();
            SceneLoader.Instance.LoadScene(SceneName.Game, true);
        }

        public ShipSelectionController GetSelectionController(bool isMain)
        {
            return isMain
                ? _mainSelectionControllers
                : _secondSelectionControllers;
        }
        
        public void CloseLobby()
        {
            RemoveClientServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
        private async void ShutDown()
        {
            await MatchmakingService.LeaveLobby(false);
            SceneLoader.Instance.LoadScene(SceneName.Menu, false);
            NetworkManager.Singleton.Shutdown();
        }
        
        #endregion

    }
}