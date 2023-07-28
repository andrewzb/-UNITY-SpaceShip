using System.Collections;
using Ship.Utils;
using SpaceShip.Managers;
using UnityEngine;

namespace SpaceShip.UI
{
    public class MenuController : MonoBehaviour
    {
        private void Start()
        {
            SceneLoader.Instance.SelectedShip.Clear();
            SceneLoader.Instance.Coins = 0;
            NetworkManagerSubscription.Instance.WaitUntilNetworkInit();
        }
        
        public void Host()
        {
            StartHost();
        }
        
        public void Join()
        {
            StartJoin();
        }
        
        public void Exit()
        {
            StartExit();
        }

        private void StartHost()
        {
            SceneLoader.Instance.LoadScene(SceneName.CreateLobby, false);
        }
        
        private void StartJoin()
        {
            SceneLoader.Instance.LoadScene(SceneName.SearchLobbies, false);
        }
        
        private void StartExit()
        {
            Application.Quit();
        }
    }
}