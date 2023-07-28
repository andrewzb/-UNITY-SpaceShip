using System;
using System.Collections;
using System.Collections.Generic;
using SpaceShip.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShip.Managers
{
    public enum SceneName : byte
    {
        Authorization,
        Menu,
        CreateLobby,
        SearchLobbies,
        Lobby,
        Game,
        Defeat,
        Victory
    }

    public class SceneLoader : SingletonPersistent<SceneLoader>
    {
        public SceneName ActiveScene => _activeScene;
        
        private SceneName _activeScene;
        
        public Dictionary<ulong, int> SelectedShip = new ();
        public int Coins;

        public void Init()  
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }
        
        public void LoadScene(SceneName sceneToLoad, bool isNetworkSessionActive = true)
        {
            StartCoroutine(Loading(sceneToLoad, isNetworkSessionActive));
        }
        
        private IEnumerator Loading(SceneName sceneToLoad, bool isNetworkSessionActive)
        {
            
            LoadingFadeEffect.Instance.RunFadeIn();
            yield return new WaitUntil(() => LoadingFadeEffect.isCanRun);
            if (isNetworkSessionActive)
            {
                if (NetworkManager.Singleton.IsServer)
                    LoadSceneNetwork(sceneToLoad);
            }
            else
            {
                LoadSceneLocal(sceneToLoad);
            }

            yield return new WaitForSeconds(1f);

            LoadingFadeEffect.Instance.RunFadeOut();
        }
        
        private void LoadSceneLocal(SceneName sceneToLoad)
        {
            SceneManager.LoadScene(sceneToLoad.ToString());
        }

        private void LoadSceneNetwork(SceneName sceneToLoad)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                sceneToLoad.ToString(),
                LoadSceneMode.Single);
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            Enum.TryParse(sceneName, out _activeScene);

            switch (_activeScene)
            {
                case SceneName.Lobby:
                    LobbySelectionManager.Instance.ServerSceneInit(clientId);
                    break;

            case SceneName.Game:
                GameManager.Instance.ServerSceneInit(clientId);
                break;
            }
        }
    }
}