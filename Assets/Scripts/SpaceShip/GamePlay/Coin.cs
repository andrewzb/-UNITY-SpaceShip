using Ship.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.GamePlay
{
    public class Coin : NetworkBehaviour
    {
        [SerializeField] [Range(1, 50)] private int _value = 1;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!IsServer)
                return;

            if (collider.TryGetComponent(out ICoinable coinable))
            {

                coinable.TakeCoins(_value);
                NetworkObjectDeSpawner.DeSpawnNetworkObject(NetworkObject);

            }
        }
    }
}