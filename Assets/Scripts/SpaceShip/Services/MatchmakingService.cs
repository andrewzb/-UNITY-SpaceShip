using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpaceShip.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace SpaceShip.Services
{
    public class MatchmakingService : MonoBehaviour
    {
        private const int HEART_BEAT_INTERVAL = 15;
        private const int LOBBY_UPDATE_INVERVAL = 2;

        private static Lobby _currentLobby;
        private static CancellationTokenSource _heartbeatSource;
        private static CancellationTokenSource _updateLobbySource;

        private static UnityTransport _transport;

        private static UnityTransport Transport
        {
            get
            {
                if (_transport == null)
                {
                    _transport = FindObjectOfType<UnityTransport>();
                }

                return _transport;
            }

            set => _transport = value;
        }

        public static event Action<Lobby> CurrentLobbyUpdateAction;

        public static void ResetStatics()
        {
            if (Transport != null)
            {
                Transport.Shutdown();
                Transport = null;
            }

            _currentLobby = null;
        }

        public static async Task CreateLobby(LobbyData data)
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(data.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            var isPrivate = false;


            var options = new CreateLobbyOptions();
            var optionData = new Dictionary<string, DataObject>
            {
                { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
            };
            options.IsPrivate = isPrivate;
            options.Data = optionData;

            _currentLobby = await Lobbies.Instance.CreateLobbyAsync(data.Name, data.MaxPlayers, options);

            Transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
            PendingLobbyHeartBeat();
            PendingLobbyUpdate();
        }

        private static async void PendingLobbyHeartBeat()
        {
            _heartbeatSource = new CancellationTokenSource();
            while (!_heartbeatSource.IsCancellationRequested && _currentLobby != null)
            {
                await Lobbies.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                await Task.Delay(HEART_BEAT_INTERVAL * 1000);
            }
        }

        private static async void PendingLobbyUpdate()
        {
            _updateLobbySource = new CancellationTokenSource();
            await Task.Delay(LOBBY_UPDATE_INVERVAL);
            while (!_updateLobbySource.IsCancellationRequested && _currentLobby != null)
            {
                _currentLobby = await Lobbies.Instance.GetLobbyAsync(_currentLobby.Id);
                CurrentLobbyUpdateAction?.Invoke(_currentLobby);
                await Task.Delay(LOBBY_UPDATE_INVERVAL * 1000);
            }
        }

        public static async Task JoinLobby(string lobbyId)
        {
            _currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            var allocation = await RelayService.Instance.JoinAllocationAsync(_currentLobby.Data["joinCode"].Value);
            
            Transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            PendingLobbyUpdate();
        }

        public static async Task LeaveLobby(bool isDelete = false)
        {
            _heartbeatSource?.Cancel();
            _updateLobbySource?.Cancel();
            if (_currentLobby != null)
            {
                if (isDelete && _currentLobby.HostId == Authentication.PlayerId)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
                }
                else
                {
                    await Lobbies.Instance.RemovePlayerAsync(_currentLobby.Id, Authentication.PlayerId);
                }
            }
        }

        public static async Task RemovePlayerFromLobby(string playerId)
        {
            await Lobbies.Instance.RemovePlayerAsync(_currentLobby.Id, playerId);
        }

        public static async Task DeleteLobby()
        {
            _heartbeatSource?.Cancel();
            _updateLobbySource?.Cancel();
            if (_currentLobby != null)
            {
                await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
            }
        }

        public static List<Player> PullPlayerInLobby()
        {
            return _currentLobby?.Players ?? new List<Player>();
        }


        public static async Task<List<Lobby>> PullLobbies()
        {
            var options = new QueryLobbiesOptions();
            var optionFilters = new List<QueryFilter>
            {
                new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                new(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
            };

            options.Count = 3;
            options.Filters = optionFilters;

            var allLobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            return allLobbies.Results;
        }
        
        public static async Task<Lobby> PullLobby(string id)
        {
            return await Lobbies.Instance.GetLobbyAsync(id);
        }

        public static bool TryGetConnectedPlayer(bool isMain, out Player player)
        {
            player = null;

            //TODO
            // MOCK

            var playerList = _currentLobby?.Players ?? new List<Player>();
            var myID = Authentication.PlayerId;
            var myPlayerIndex = playerList.FindIndex((p) => p.Id == myID);
            var otherPlayerIndex = playerList.FindIndex((p) => p.Id != myID);

            if (isMain)
            {
                if (myPlayerIndex > 1)
                {
                    player = playerList[myPlayerIndex];
                    return true;
                }
            }
            else if (otherPlayerIndex > 0)
            {
                player = playerList[otherPlayerIndex];
                return true;
            }

            player = null;
            return false;
        }

        public static bool TryGetConnectedPlayer(string playerId, out Player player)
        {
            var playerList = _currentLobby.Players ?? new List<Player>();
            var index = playerList.FindIndex((el) => el.Id == playerId);
            if (index > -1)
            {
                player = playerList[index];
                return true;
            }

            player = null;
            return false;
        }

        private static void EmitUpdateAction()
        {
            CurrentLobbyUpdateAction?.Invoke(_currentLobby);
        }
        
    }
}