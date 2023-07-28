using Ship.Utils;
using SpaceShip.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceShip.GamePlay
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _count;
        [SerializeField] [Range(0.5f, 1)] private float _spawnZoneFactor = 1;
        private Camera _camera;

        public void Spawn()
        {
            _camera = Camera.main;
            for (int i = 0; i < _count; i++)
            {
                var randPosition = GetRandomPosition();
                NetworkObjectSpawner.SpawnNetworkObject(_prefab, true,
                    randPosition, Quaternion.identity);
            }
        }

        private Vector3 GetRandomPosition()
        {
            var hs = (_camera.GetOrthographicCameraSize() / 2) * _spawnZoneFactor;
            return new Vector3(Random.Range(-hs.x, hs.x), Random.Range(-hs.y, hs.y), 0);
        }
    }
}