using System;
using System.Collections.Generic;
using SpaceShip.Models;
using SpaceShip.UI.Components;
using SpaceShip.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.UI.Controllers
{
    public class ShipSelectionController : MonoBehaviour
    {
        [SerializeField] private List<ShipConfigSO> _shipCollection;
        [SerializeField] private bool _isMain;
        [SerializeField] private TextMeshProUGUI _palyerName;
        [SerializeField] private Image _statusImage;
        [SerializeField] private Sprite _statusReady;
        [SerializeField] private Sprite _statusNotReady;
        [SerializeField] private Image _shipPreview;
        [SerializeField] private TextMeshProUGUI _shipName;
        [SerializeField] private RectTransform _shipPreviewLoader;
        [Header("ship Stats")]        
        [SerializeField] private Bar _hullPointsBar;
        [SerializeField] private Bar _forwardSpeedBar;
        [SerializeField] private Bar _backwardSpeedBar;
        [SerializeField] private Bar _sideSpeedBar;
        [SerializeField] private Bar _rotationSpeedBar;
        [Space(5)]
        [SerializeField] private TextMeshProUGUI _statusToggleName;
        [SerializeField] private Color _dangerColor;
        [SerializeField] private Color _successColor;
        
        private int _shipIndex;
        private bool _status;

        public bool IsMane => _isMain;
        public Action<int> ShipChangedAction;
        public Action<bool> StatusChangedAction;
        
        public void Init()
        {
            _shipIndex = 0;
        }
        
        public void Reset()
        {
            _shipIndex = -1;
            UpdateView(0, String.Empty, false, true);
        }

        public void UpdateView(int index, string playerName, bool isReady, bool isDefault = false)
        {
            UpdatePlayer(playerName, isReady, isDefault);
            UpdateShip(index, isDefault);
            _status = isReady;
            _shipIndex = index;
        }

        private void UpdateShip(int index, bool isDefault)
        {
            if (isDefault)
            {
                _shipPreviewLoader.gameObject.SetActive(true);
                _shipPreview.gameObject.SetActive(false);
                _shipName.text = "Ship Name";
                _shipPreview.sprite = null;
                if (_isMain)
                {   
                    _hullPointsBar.Set(0);
                    _forwardSpeedBar.Set(0);
                    _backwardSpeedBar.Set(0);
                    _sideSpeedBar.Set(0);
                    _rotationSpeedBar.Set(0);
                }
            }
            else
            {
                var shipConfig = _shipCollection[index];
                _shipPreviewLoader.gameObject.SetActive(false);
                _shipPreview.gameObject.SetActive(true);
                _shipName.text = shipConfig.ShipName;
                _shipPreview.sprite = shipConfig.ShipSprite;
                if (_isMain)
                {   
                    _hullPointsBar.Set(shipConfig.HullPoints / 300.0f);
                    _forwardSpeedBar.Set(shipConfig.ForwardSpeed / 30.0f);
                    _backwardSpeedBar.Set(shipConfig.BackwardSpeed / 20.0f);
                    _sideSpeedBar.Set(shipConfig.SideSpeed / 20.0f);
                    _rotationSpeedBar.Set(shipConfig.RotationSpeed / 60.0f);
                }
            }
        }

        private void UpdatePlayer(string playerName, bool isReady, bool isDefault)
        {
            if (isDefault)
            {
                _palyerName.text = "NOT_INIT";
                _statusImage.sprite = _statusNotReady;
                _statusToggleName.text = "Ready";
            }
            else
            {
                _palyerName.text = playerName;
                _statusImage.sprite = isReady ? _statusReady : _statusNotReady;
                if (_isMain)
                {
                    _statusToggleName.color = isReady
                        ? _dangerColor
                        : _successColor;
                    _statusToggleName.text = isReady
                        ? "Cancel"
                        : "Ready";
                }
            }
        }

        public void NextShip() => ChangeSelectedShip(_shipIndex + 1);
        
        public void PrevShip() => ChangeSelectedShip(_shipIndex - 1);
        
        public void ToggleStatus()
        {
            StatusChangedAction?.Invoke(!_status);
        }
        
        private void ChangeSelectedShip(int indexDiff)
        {
            ShipChangedAction?.Invoke(_shipCollection.ClampListIndex(indexDiff));
        }
        
    }
}