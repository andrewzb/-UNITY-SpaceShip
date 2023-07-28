using UnityEngine;

namespace SpaceShip.Models
{
    [CreateAssetMenu(fileName = "ShipConfigSO", menuName = "ScriptableObjects/ShipConfigSO", order = 1)]
    public class ShipConfigSO : ScriptableObject
    {
        [SerializeField] private string _shipName;
        [SerializeField] private float _hullPoints;
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _backwardSpeed;
        [SerializeField] private float _sideSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Sprite _shipSprite;
        
        public string ShipName => _shipName;
        public float HullPoints => _hullPoints;
        public float ForwardSpeed => _forwardSpeed;
        public float BackwardSpeed => _backwardSpeed;
        public float SideSpeed => _sideSpeed;
        public float RotationSpeed => _rotationSpeed;
        public Sprite ShipSprite => _shipSprite;
    }
}