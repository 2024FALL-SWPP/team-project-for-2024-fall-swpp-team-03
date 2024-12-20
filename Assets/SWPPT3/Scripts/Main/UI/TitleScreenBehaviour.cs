#region

using System;
using SWPPT3.Main.AudioLogic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

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
    }

    public class TitleScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> _onTryingExitStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingOptionStatusChanged;

        [SerializeField] private PlayerScript _playerScript;
        [SerializeField] private CameraScript _cameraScript;
        [SerializeField] private BGMScript _bgmScript;


        [SerializeField] private GameObject _optionScene;

        private Slider _bgmSlider;
        private Slider _sfxSlider;
        private Slider _cameraSensitivitySlider;
        private Slider _rotationSensitivitySlider;

        private TextMeshProUGUI _bgmText;
        private TextMeshProUGUI _soundText;
        private TextMeshProUGUI _cameraSensivityText;
        private TextMeshProUGUI _rotationSensivityText;


        public void OnButtonClick(int type)
        {
            OnButtonClick((ButtonClickType) type);
        }

        public void OnButtonClick(ButtonClickType type)
        {
            switch (type)
            {
                case ButtonClickType.Tutorial1:
                case ButtonClickType.Tutorial2:
                case ButtonClickType.Stage1:
                case ButtonClickType.Stage2:
                case ButtonClickType.Stage3:
                case ButtonClickType.Stage4:
                case ButtonClickType.Stage5:
                    if (GameManager.Instance.GameState == GameState.BeforeStart)
                    {
                        GameManager.Instance.StageSelect((int)type);
                    }
                    break;
                case ButtonClickType.Option:
                    ClickOption();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
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
            _onTryingExitStatusChanged.Invoke(false);
        }

        public void ClickExitGame()
        {
            Application.Quit();
        }

        private void Start()
        {
            _onTryingExitStatusChanged.Invoke(false);
            _onTryingOptionStatusChanged.Invoke(false);
            GameManager.Instance.GameState = GameState.BeforeStart;
            // _exitGame = transform.Find("ExitGame");
            // _optionScene = GameObject.Find("OptionScene");

            var parentSlider = _optionScene.transform.Find("VerticalAlign");
            _bgmSlider = parentSlider.Find("BGMSlider").GetComponent<Slider>();
            _sfxSlider = parentSlider.Find("SoundEffectSlider").GetComponent<Slider>();
            _cameraSensitivitySlider = parentSlider.Find("CameraSensitivitySlider").GetComponent<Slider>();
            _rotationSensitivitySlider = parentSlider.Find("RotationSensitivitySlider").GetComponent<Slider>();

            _bgmText = parentSlider.Find("BGMSlider").Find("Value").GetComponent<TextMeshProUGUI>();
            _soundText = parentSlider.Find("SoundEffectSlider").Find("Value").GetComponent<TextMeshProUGUI>();
            _cameraSensivityText = parentSlider.Find("CameraSensitivitySlider").Find("Value").GetComponent<TextMeshProUGUI>();
            _rotationSensivityText = parentSlider.Find("RotationSensitivitySlider").Find("Value").GetComponent<TextMeshProUGUI>();

            _cameraSensitivitySlider.value = _cameraScript.MouseSensitivity;
            _rotationSensitivitySlider.value = _playerScript.RotationSpeed;
            _bgmSlider.value = _bgmScript.BgmVolume;
            _sfxSlider.value = _bgmScript.SfxVolume;

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
            }
            else
            {
                Debug.LogError("InputManager is null");
            }
        }

        public void Update()
        {
            if (GameManager.Instance.GameState == GameState.OnOption)
            {
                _cameraScript.MouseSensitivity = _cameraSensitivitySlider.value;
                _playerScript.RotationSpeed = _rotationSensitivitySlider.value;
                _bgmScript.BgmVolume = _bgmSlider.value;
                _bgmScript.SfxVolume = _sfxSlider.value;

                BgmManager.Instance.BGMVolume = _bgmSlider.value;
                BgmManager.Instance.SFXVolume = _sfxSlider.value;

                _bgmText.text = $"{Mathf.RoundToInt(_bgmSlider.value * 100)}";
                _soundText.text = $"{Mathf.RoundToInt(_sfxSlider.value * 100)}";
                _cameraSensivityText.text = $"{Mathf.RoundToInt(_cameraSensitivitySlider.value * 100)}";
                _rotationSensivityText.text = $"{Mathf.RoundToInt(_rotationSensitivitySlider.value * 100)}";
            }
        }

        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                GameManager.Instance.GameState = GameState.Exit;
                _onTryingExitStatusChanged.Invoke(true);
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc -= HandleEsc;
            }
        }
    }
}
