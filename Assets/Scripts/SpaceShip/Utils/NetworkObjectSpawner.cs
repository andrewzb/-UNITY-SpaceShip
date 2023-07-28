using Unity.Netcode;
using UnityEngine;

namespace Ship.Utils
{
    public class NetworkObjectSpawner
    {
        private static NetworkObject HandleSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogError("ERROR: Spawn not on server");
            }
            
            var go = Object.Instantiate(prefab, position, rotation);
            return go.GetComponent<NetworkObject>();
        }
        
        public static GameObject SpawnNetworkObject(
            GameObject prefab,
            bool destroyWithScene = true,
            Vector3 position = default,
            Quaternion rotation = default)
        {
            var networkObject = HandleSpawn(prefab, position, rotation);
            networkObject.Spawn(destroyWithScene);
            
            return networkObject.gameObject;
        }
        
        public static GameObject SpawnPlayerNetworkObject(
            GameObject prefab,
            ulong clientId,
            bool destroyWithScene = true,
            Vector3 position = default,
            Quaternion rotation = default)
        {
            var networkObject = HandleSpawn(prefab, position, rotation);
            networkObject.SpawnAsPlayerObject(clientId, destroyWithScene);
            
            return networkObject.gameObject;
        }
        
        public static GameObject SpawnClientOwnershipNetworkObject(
            GameObject prefab,
            ulong clientId,
            bool destroyWithScene = true,
            Vector3 position = default,
            Quaternion rotation = default)
        {
            var networkObject = HandleSpawn(prefab, position, rotation);
            networkObject.SpawnWithOwnership(clientId, destroyWithScene);
            return networkObject.gameObject;
        }
    }
}