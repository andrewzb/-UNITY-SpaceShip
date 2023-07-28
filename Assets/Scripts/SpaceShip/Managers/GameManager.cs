using System.Collections.Generic;
using System.Linq;
using SpaceShip.GamePlay;
using SpaceShip.Models;
using SpaceShip.Services;
using SpaceShip.UI.Components;
using SpaceShip.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.Managers
{
    public class GameManager : SingletonNetwork<GameManager>
    {
        [SerializeField] private PlayerView _mainPlayerView;
        [SerializeField] private PlayerView _secondPlayer;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private ShipConfigSO[] _shipCollection;
        [SerializeField] private Spawner[] _instantSpawnerCollection;
        
        private Dictionary<ulong, PlayerController> _players = new();

        private void Start()
        {
            if (IsServer)
            {
                foreach (var spawner in _instantSpawnerCollection)
                {
                    spawner.Spawn();
                }
            }
        }

        public void ServerSceneInit(ulong clientId)
        {
            var shipIndex = SceneLoader.Instance.SelectedShip[clientId];
            var go = ShipConstructor.Instance.ConstructAndSpawn(_playerPrefab, clientId, shipIndex);
            ClientConnectedClientRpc(clientId);
        }

        [ClientRpc]
        private void ClientConnectedClientRpc(ulong clientId)
        {
            var players = FindObjectsOfType<PlayerController>();
            _players.Clear();
            foreach (var playerController in players)
            {
                var shipIndex = SceneLoader.Instance.SelectedShip[playerController.OwnerClientId];
                _players.Add(playerController.OwnerClientId, playerController);
                playerController.InitClient(
                    GetPlayerPreview(playerController.OwnerClientId), _shipCollection[shipIndex]);
            }
        }

        private PlayerView GetPlayerPreview(ulong clientId)
        {
            if (clientId == 0)
            {
                return _mainPlayerView;
            }
            return _secondPlayer;
        }

        [ClientRpc]
        private void DefeatClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
        {
            SceneLoader.Instance.Coins = _players[clientId].GetCoins();
            Shutdown(SceneName.Defeat);
        }
        
        [ClientRpc]
        private void VictoryClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
        {
            SceneLoader.Instance.Coins = _players[clientId].GetCoins();
            Shutdown(SceneName.Victory);
        }

        private async void Shutdown(SceneName sceneName)
        {
            await MatchmakingService.LeaveLobby(false);
            SceneLoader.Instance.LoadScene(sceneName, false);
            NetworkManager.Singleton.Shutdown();
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerDestroyServerRpc(ulong clientId)
        {
            Debug.Log("PlayerDestroyServerRpc");
                
            ClientRpcParams clientRpcDefeatParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            };
            DefeatClientRpc(clientId, clientRpcDefeatParams);
            if ((_players.Count - 1) == 1)
            {
                var index = _players.Keys.ToList().FindIndex((id) => id != clientId);
                if (index > -1)
                {
                    var id = _players.Keys.ToList()[index];
                    ClientRpcParams clientRpcVictoryParams = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new[] { id }
                        }
                    };
                    VictoryClientRpc(id, clientRpcVictoryParams);
                }
            }
        }
        
    }
}