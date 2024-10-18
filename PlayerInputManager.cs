using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpeedSeeker.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        private static PlayerInputManager _Instance;
        public static PlayerInputManager Instance => _Instance;

        private PlayerInput _playerInput;

        public PlayerInput playerInput => _playerInput;
        private GameObject player;

        private void Awake()
        {
            if (_Instance != null && _Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _Instance = this;
            GetPlayerInput();
        }

        static PlayerInputManager()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("PlayerInputManager").AddComponent<PlayerInputManager>();
                DontDestroyOnLoad(_Instance.gameObject);
            }
        }

        public InputAction FindAction(string actionName)
        {
            if (_playerInput == null)
            {
                Debug.LogError("Player Input is null! Attempting to retrieve it...");
                GetPlayerInput();

                // if still null complain again
                if (_playerInput == null) { Debug.LogError("Failed to retrieve the Player Input component!"); return null; }
                else Debug.Log("Retrieval of Player Input component Success!");

            }
            return _playerInput.actions.FindAction(actionName);
        }

        void GetPlayerInput()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            _playerInput = player.GetComponent<PlayerInput>();
        }
    }
}
