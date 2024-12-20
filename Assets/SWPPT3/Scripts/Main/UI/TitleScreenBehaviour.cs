using System;
using SWPPT3.Main.AudioLogic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SWPPT3.Main.UI
{
    public enum ButtonClickType
    {
        Tutorial1 = 0,
        Tutorial2,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Option,
        Howto,
    }

    public class TitleScreenBehaviour : MonoBehaviour
    {
        [Header("UI Events")]
        [SerializeField] private UnityEvent<bool> _onTryingExitStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingOptionStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingHowtoStatusChanged;

        [Header("UI Screens")]
        [SerializeField] private GameObject _optionScene;
        [SerializeField] private GameObject _howtoScreen;

        [Header("Sliders and Text")]
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _cameraSensitivitySlider;
        [SerializeField] private Slider _rotationSensitivitySlider;

        [SerializeField] private TextMeshProUGUI _bgmValue;
        [SerializeField] private TextMeshProUGUI _sfxValue;
        [SerializeField] private TextMeshProUGUI _cameraSensitivity;
        [SerializeField] private TextMeshProUGUI _rotationSensitivity;

        private void Start()
        {
            InitializeUI();
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void InitializeUI()
        {
            _onTryingExitStatusChanged.Invoke(false);
            _onTryingOptionStatusChanged.Invoke(false);
            GameManager.Instance.GameState = GameState.BeforeStart;

            _bgmSlider.value = BgmManager.Instance.BGMVolume;
            _sfxSlider.value = BgmManager.Instance.SFXVolume;
            _cameraSensitivitySlider.value = InputManager.Instance.CameraCoffeicient;
            _rotationSensitivitySlider.value = InputManager.Instance.RotationCoefficient;

            UpdateSliderTextValues();
        }

        private void SubscribeEvents()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
            }
            else
            {
                Debug.LogError("InputManager is null");
            }
        }

        private void UnsubscribeEvents()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc -= HandleEsc;
            }
        }

        public void Update()
        {
            UpdateGameSettings();
            UpdateSliderTextValues();
        }

        private void UpdateGameSettings()
        {
            InputManager.Instance.CameraCoffeicient = _cameraSensitivitySlider.value;
            InputManager.Instance.RotationCoefficient = _rotationSensitivitySlider.value;
            BgmManager.Instance.BGMVolume = _bgmSlider.value;
            BgmManager.Instance.SFXVolume = _sfxSlider.value;
        }

        private void UpdateSliderTextValues()
        {
            _bgmValue.text = $"{(int)(_bgmSlider.value * 100)}";
            _sfxValue.text = $"{(int)(_sfxSlider.value * 100)}";
            _cameraSensitivity.text = $"{(int)(_cameraSensitivitySlider.value * 100)}";
            _rotationSensitivity.text = $"{(int)(_rotationSensitivitySlider.value * 100)}";
        }

        public void OnButtonClick(int type)
        {
            OnButtonClick((ButtonClickType)type);
        }

        public void OnButtonClick(ButtonClickType type)
        {
            if (GameManager.Instance.GameState != GameState.BeforeStart) return;

            switch (type)
            {
                case ButtonClickType.Tutorial1:
                case ButtonClickType.Tutorial2:
                case ButtonClickType.Stage1:
                case ButtonClickType.Stage2:
                case ButtonClickType.Stage3:
                case ButtonClickType.Stage4:
                case ButtonClickType.Stage5:
                    GameManager.Instance.StageSelect((int)type);
                    break;

                case ButtonClickType.Option:
                    ClickOption();
                    break;

                case ButtonClickType.Howto:
                    ClickHowto();
                    break;

                default:
                    Debug.LogError($"Unhandled ButtonClickType: {type}");
                    break;
            }
        }

        public void ClickHowto()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                _howtoScreen.SetActive(true);
                GameManager.Instance.GameState = GameState.OnHowto;
            }
        }

        public void ClickOption()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                _optionScene.SetActive(true);
                GameManager.Instance.GameState = GameState.OnOption;
            }
        }

        public void ClickMainMenu()
        {
            GameManager.Instance.GameState = GameState.BeforeStart;
            _optionScene.SetActive(false);
            _howtoScreen.SetActive(false);
            _onTryingExitStatusChanged.Invoke(false);
        }

        public void ClickExitGame()
        {
            Application.Quit();
        }

        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                GameManager.Instance.GameState = GameState.Exit;
                _onTryingExitStatusChanged.Invoke(true);
            }
        }
    }
}
