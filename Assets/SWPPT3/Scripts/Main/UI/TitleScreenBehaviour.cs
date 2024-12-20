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

        [SerializeField] private GameObject _optionScene;

        private TextMeshProUGUI _bgmValue;
        private TextMeshProUGUI _sfxValue;
        private TextMeshProUGUI _cameraSensitivity;
        private TextMeshProUGUI _rotationSensitivity;


        private Slider _bgmSlider;
        private Slider _sfxSlider;
        private Slider _cameraSensitivitySlider;
        private Slider _rotationSensitivitySlider;

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

            _bgmValue = parentSlider.Find("BGMSlider/Value").GetComponent<TextMeshProUGUI>();
            _sfxValue = parentSlider.Find("SoundEffectSlider/Value").GetComponent<TextMeshProUGUI>();
            _cameraSensitivity = parentSlider.Find("CameraSensitivitySlider/Value").GetComponent<TextMeshProUGUI>();
            _rotationSensitivity = parentSlider.Find("RotationSensitivitySlider/Value").GetComponent<TextMeshProUGUI>();

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
            }
            else
            {
                Debug.LogError("InputManager is null");
            }

            _bgmSlider.value = BgmManager.Instance.BGMVolume;
            _sfxSlider.value = BgmManager.Instance.SFXVolume;
            _cameraSensitivitySlider.value = InputManager.Instance.CameraCoffeicient;
            _rotationSensitivitySlider.value = InputManager.Instance.RotationCoefficient;
        }

        public void Update()
        {
            InputManager.Instance.CameraCoffeicient =  _cameraSensitivitySlider.value;
            InputManager.Instance.RotationCoefficient = _rotationSensitivitySlider.value;
            BgmManager.Instance.BGMVolume = _bgmSlider.value;
            BgmManager.Instance.SFXVolume = _sfxSlider.value;

            _bgmValue.text = $"{(int)(_bgmSlider.value*100)}";
            _sfxValue.text = $"{(int)(_sfxSlider.value*100)}";
            _cameraSensitivity.text = $"{(int)(_cameraSensitivitySlider.value*100)}";
            _rotationSensitivity.text = $"{(int)(_rotationSensitivitySlider.value*100)}";
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

