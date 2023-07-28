using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceShip.Managers;
using SpaceShip.Models;
using SpaceShip.Services;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.UI
{
    public class CreateLobbyController : MonoBehaviour
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _submitBtn;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _messageField;

        private const string MESSAGE_DEFAULT = "Enter lobby name";
        private const string MESSAGE_ERROR = "Unvalid Name";
        private const string MESSAGE_CREATING = "Create lobby";
        
        private void Start()
        {
            _messageField.text = MESSAGE_DEFAULT;
        }

        public void OnClose()
        {
            StartClose();
        }

        public async void OnSubmit()
        {
            if (string.IsNullOrWhiteSpace(_inputField.text))
            {
                _messageField.text = MESSAGE_ERROR;
            }
            else
            {
                _messageField.text = MESSAGE_CREATING;
                var lobbyData = new LobbyData(_inputField.text);
                await MatchmakingService.CreateLobby(lobbyData);
                NetworkManager.Singleton.StartHost();
                SceneLoader.Instance.LoadScene(SceneName.Lobby, true);
            }
        }

        private void StartClose()
        {
            SceneLoader.Instance.LoadScene(SceneName.Menu, false);
        }

    }
}