using Unity.Mathematics;
using UnityEngine;

namespace SpaceShip.UI.Components
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] [Range(-180, 180)] private float _rotationSpeed = 5;

        private void Update()
        {
            transform.rotation *= quaternion.Euler(
                0, 0, _rotationSpeed * Time.deltaTime);
        }
    }
}