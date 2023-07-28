using System.Collections;
using System.Collections.Generic;
using SpaceShip.Managers;
using SpaceShip.Services;
using SpaceShip.UI.Components;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.UI.Controllers
{
    public class SearchLobbyController : MonoBehaviour
    {
        [SerializeField] private LobbyPreview _lobbyPreviewPrefab;
        [SerializeField] private RectTransform _lobbyListContainer;
        
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _refreshBtn;

        [SerializeField] private RectTransform _loader;

        private void Start()
        {
            Refresh();
        }

        public async void Refresh()
        {
            _refreshBtn.interactable = false;
            Clear();
            _loader.gameObject.SetActive(true);
            var lobbies = await MatchmakingService.PullLobbies();
            _loader.gameObject.SetActive(false);
            Spawn(lobbies);
            _refreshBtn.interactable = true;
        }

        public void Close()
        {
            SceneLoader.Instance.LoadScene(SceneName.Menu, false);
        }
        
        private void Spawn(List<Lobby> lobbyStructsList)
        {
            foreach (var lobby in lobbyStructsList)
            {
                var preview = Instantiate(_lobbyPreviewPrefab, Vector3.zero,
                    Quaternion.identity, _lobbyListContainer);
                preview.Init(lobby.Id, JoinLobby);
            }
        }
        
        private void Clear()
        {
            for (int i = 0; i < _lobbyListContainer.childCount; i++)
            {
                Destroy(_lobbyListContainer.GetChild(i).gameObject);
            }
        }

        private async void JoinLobby(string lobbyId)
        {
            await MatchmakingService.JoinLobby(lobbyId);
            StartCoroutine(RunJoin());
        }

        private IEnumerator RunJoin()
        {
            LoadingFadeEffect.Instance.RunFadeAll();
            yield return new WaitUntil(() => LoadingFadeEffect.isCanRun);
            NetworkManager.Singleton.StartClient();
        }
        
    }
}