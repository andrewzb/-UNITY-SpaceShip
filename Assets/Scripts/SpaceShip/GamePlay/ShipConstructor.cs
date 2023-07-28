using Ship.Utils;
using SpaceShip.Models;
using SpaceShip.Utils;
using UnityEngine;

namespace SpaceShip.GamePlay
{
    public class ShipConstructor : SingletonNetwork<ShipConstructor>
    {
        [SerializeField] [Range(0.5f, 1)] private float _spawnZoneFactor;
        [SerializeField] private ShipConfigSO[] _shipCollection;

        private Vector2 GetRandomSpawnPosition()
        {
            var hf = (Camera.main.GetOrthographicCameraSize() * _spawnZoneFactor) / 2;
            return new Vector2(Random.Range(-hf.x, hf.x), Random.Range(-hf.y, hf.y));
        }

        public GameObject ConstructAndSpawn(GameObject prefab, ulong clientId, int shipIndex)
        {
            var spawnPosition = GetRandomSpawnPosition();
            var go = NetworkObjectSpawner.SpawnClientOwnershipNetworkObject(
                prefab, clientId, true, spawnPosition, Quaternion.identity);
            
            var playerController = go.GetComponent<PlayerController>();
            playerController.Init(_shipCollection[shipIndex]);
            return go;
        }
    }
}