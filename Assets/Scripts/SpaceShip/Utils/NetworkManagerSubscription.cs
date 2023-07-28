using System.Collections;
using System.Collections.Generic;
using SpaceShip.Managers;
using SpaceShip.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Ship.Utils
{
    public class NetworkManagerSubscription: SingletonPersistent<NetworkManagerSubscription>
    {
        public void WaitUntilNetworkInit()
        {
            StartCoroutine(WaitUntilNetworkInitAndSubscribe());
        }
        
        private IEnumerator WaitUntilNetworkInitAndSubscribe()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton != null);
            yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);
            SceneLoader.Instance.Init();
        }
        
        
    }
}