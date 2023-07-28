using Ship.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.GamePlay
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private float _speed;
        [SerializeField] private float _inactiveTimer;

        private void Update()
        {
            transform.position += transform.up * (_speed * Time.deltaTime);
            if (IsServer)
            {
                if (_inactiveTimer > 0)
                {
                    _inactiveTimer -= Time.deltaTime;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!IsServer)
            {
                return;
            }

            if (_inactiveTimer > 0)
            {
                return;
            }
            
            if (collider.TryGetComponent(out IDamagable damagable))
            {

                damagable.TakeDamage(_damage);
                NetworkObjectDeSpawner.DeSpawnNetworkObject(NetworkObject);

            }
        }
    }
}
