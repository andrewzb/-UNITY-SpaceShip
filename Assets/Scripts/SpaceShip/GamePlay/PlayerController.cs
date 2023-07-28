using Ship.Utils;
using SpaceShip.Managers;
using SpaceShip.Models;
using SpaceShip.UI.Components;
using SpaceShip.Utils;
using Unity.Netcode;
using UnityEngine;

namespace SpaceShip.GamePlay
{
    public class PlayerController : NetworkBehaviour, IDamagable, ICoinable
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Camera _camera;
        [SerializeField] private ShipConfigSO _shipConfig;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private int _fireRate = 1;

        [SerializeField] private NetworkVariable<float> _hull = new NetworkVariable<float>(100);
        [SerializeField] private NetworkVariable<int> _coins = new NetworkVariable<int>(0);

        [SerializeField]private PlayerView _playerView;
        
        private PlayerInput _playerInput;
        private float _fireTimer = 0;

        // server
        public void Init(ShipConfigSO shipConfig)
        {
            _shipConfig = shipConfig;
            _renderer.sprite = _shipConfig.ShipSprite;
            _hull.Value = _shipConfig.HullPoints;
            _coins.Value = 0;
        }
        
        //client
        public void InitClient(PlayerView playerView, ShipConfigSO shipConfig)
        {
            ToggleSubscription(true);
            _shipConfig = shipConfig;
            name = $"player {NetworkObject.OwnerClientId + 1}";
            _playerView = playerView;
            _playerView.UpdateView(name, _hull.Value / _shipConfig.HullPoints, _coins.Value);
            _playerView.gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            _playerInput = new PlayerInput();
        }
        
        private void OnEnable()
        {
            _playerInput.Enable();
        }
        
        private void OnDisable()
        {
            ToggleSubscription(false);
            _playerInput.Disable();
            _playerView.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (IsClient)
            {
                if (!IsOwner || !Application.isFocused ) return;
                if (_camera == null)
                {
                    _camera = Camera.main;
                }
                Move();
                Rotate();
                Shoot();
            }
        }

        private void Shoot()
        {
            var isShoot = _playerInput.player.fire.ReadValue<float>();
            if (isShoot > 0)
            {
                _fireTimer -= Time.deltaTime;
                if (_fireTimer < 0)
                {
                    _fireTimer = 1f / _fireRate;
                    SpawnProjectileServerRpc();
                }
            }
        }
        
        private void Rotate()
        {
            var rotateValue = _playerInput.player.rotate.ReadValue<Vector2>();
            if (rotateValue != Vector2.zero)
            {
                var currAngle = ((Vector2)transform.up).GetAngle();
                var targetAngle = rotateValue.GetAngle();
                var diff = Mathf.DeltaAngle(currAngle, targetAngle);
                var diffTick = diff * Time.deltaTime * _shipConfig.RotationSpeed;
                transform.rotation *= Quaternion.AngleAxis(diffTick, Vector3.forward);
            }
        }

        private void Move()
        {
            var moveValue = _playerInput.player.move.ReadValue<Vector2>();
            if (moveValue != Vector2.zero)
            {
                var forwardBackwardSpeed = (moveValue.y > 0
                    ? _shipConfig.ForwardSpeed
                    : _shipConfig.BackwardSpeed) * moveValue.y;
                var verticalDiff = transform.up * forwardBackwardSpeed;
                var horizontalDiff = transform.right * (_shipConfig.SideSpeed * moveValue.x);
                var positionDiff = verticalDiff + horizontalDiff;
                var posDiffTick = positionDiff * Time.deltaTime;
                transform.position = ClampMovement(transform.position + posDiffTick);
            }
        }
        
        [ServerRpc]
        private void SpawnProjectileServerRpc()
        {
            NetworkObjectSpawner.SpawnNetworkObject(
                _projectilePrefab, true, transform.position, transform.rotation);
        }
        
        private Vector3 ClampMovement(Vector3 position)
        {
            var hs = _camera.GetOrthographicCameraSize() / 2;
            var x = Mathf.Clamp(position.x, -hs.x, hs.x);
            var y = Mathf.Clamp(position.y, -hs.y, hs.y);

            return new Vector3(x, y, 0);
        }
        
        private void ToggleSubscription(bool isSubscribe)
        {
            if (IsClient)
            {
                if (isSubscribe)
                {
                    _hull.OnValueChanged += OnHullChangedHandler;
                    _coins.OnValueChanged += OnCoinChangedHandler;
                }
                else
                {
                    _hull.OnValueChanged -= OnHullChangedHandler;
                    _coins.OnValueChanged -= OnCoinChangedHandler;
                }
            }
        }

        // client
        private void OnCoinChangedHandler(int previousValue, int newValue)
        {
            if (_playerView != null && _shipConfig != null)
            {
                _playerView.UpdateView(gameObject.name,
                    _hull.Value / _shipConfig.HullPoints, _coins.Value);
            }
        }

        // client
        private void OnHullChangedHandler(float previousValue, float newValue)
        {
            _playerView.UpdateView(gameObject.name,
                _hull.Value / _shipConfig.HullPoints, _coins.Value);

            if (newValue < 0)
            {
                GameManager.Instance.PlayerDestroyServerRpc(OwnerClientId);
                gameObject.SetActive(false);
            }
        }
        
        [ServerRpc]
        private void DestroyServerRpc()
        {
            NetworkObjectDeSpawner.DeSpawnNetworkObject(NetworkObject);
        }
        
        //server
        public void TakeDamage(float damage)
        {
            _hull.Value -= damage;
        }

        // server
        public void TakeCoins(int coins)
        {
            _coins.Value += coins; 
        }

        public int GetCoins()
        {
            return _coins.Value;
        }
        
    }
}