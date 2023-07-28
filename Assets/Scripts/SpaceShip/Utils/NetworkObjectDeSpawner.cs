using Unity.Netcode;
using UnityEngine;

namespace Ship.Utils
{
    public static class NetworkObjectDeSpawner
    {
        public static void DeSpawnNetworkObject(NetworkObject networkObject)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogError("ERROR: Despawn not on server");
            }
            
            if(networkObject != null && networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}