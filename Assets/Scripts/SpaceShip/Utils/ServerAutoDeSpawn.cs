using Ship.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.Utils
{
    public class ServerAutoDeSpawn : NetworkBehaviour
    {
        [SerializeField] [Range(0, 10)] private float _deSpawnTimer;
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            _deSpawnTimer -= Time.deltaTime;
            if (_deSpawnTimer < 0)
            {
                NetworkObjectDeSpawner.DeSpawnNetworkObject(NetworkObject);
            }
        }
    }
}