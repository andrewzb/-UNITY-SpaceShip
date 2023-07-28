using SpaceShip.UI.Controllers;
using Unity.Netcode;

namespace SpaceShip.Managers
{
    public class LobbyPlayer : NetworkBehaviour
    {
        private ShipSelectionController _selectionController;
        private ulong _playerId;
        private bool _isInit;

        public void Init(ulong playerId)
        {
            if (!_isInit)
            {

                var localId = NetworkManager.Singleton.LocalClientId;
                _playerId = playerId;
                _selectionController = LobbySelectionManager.Instance.GetSelectionController(playerId == localId);
                gameObject.name = $"player{playerId + 1}";
                _selectionController.ShipChangedAction += OnShipIndexActionHandler;
                _selectionController.StatusChangedAction += OnStatusChangedHandler;
                _selectionController.gameObject.SetActive(true);
                _selectionController.Init();
                _isInit = true;
            }
        }
        
        public void UpdateView(int index, bool isReady)
        {
            if (_selectionController != null)
            {
                _selectionController.UpdateView(index, gameObject.name, isReady);
            }
        }

        private void OnShipIndexActionHandler(int shipIndex)
        {   
            SetPlayerSelectedShipServerRpc(_playerId, shipIndex);
        }

        private void OnStatusChangedHandler(bool isReady)
        {
            SetPlayerStatusServerRpc(_playerId, isReady);
        }
        
        [ServerRpc]
        private void SetPlayerStatusServerRpc(ulong clientId, bool isReady)
        {
            LobbySelectionManager.Instance.SetPlayerState(clientId, isReady);
        }

        [ServerRpc]
        private void SetPlayerSelectedShipServerRpc(ulong clientId, int shipIndex)
        {
            LobbySelectionManager.Instance.SetPlayerSelectedShip(clientId, shipIndex);
        }

        private void OnDisable()
        {
            _selectionController.gameObject.SetActive(false);
            _selectionController.ShipChangedAction -= OnShipIndexActionHandler;
            _selectionController.StatusChangedAction -= OnStatusChangedHandler;
            _selectionController = null;
            _isInit = false;
        }
        
    }

}