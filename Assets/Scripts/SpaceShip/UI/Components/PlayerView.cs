using TMPro;
using UnityEngine;

namespace SpaceShip.UI.Components
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private Bar _healthBar;
        [SerializeField] private TextMeshProUGUI _coins;
        private ulong _clientId;

        public void Init(ulong clientId)
        {
            _clientId = clientId;
        }


        public void UpdateView(string playerName, float health, int coins)
        {
            _playerName.text = playerName;
            _healthBar.Set(health);
            _coins.text = coins.ToString();
        }
    }
}