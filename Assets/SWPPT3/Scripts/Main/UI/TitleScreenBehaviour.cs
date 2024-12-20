#region

using System;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.Utility;
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
        Howto,
    }

    public class TitleScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> _onTryingExitStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingOptionStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingHowtoStatusChanged;

        [SerializeField] private PlayerScript _playerScript;
        [SerializeField] private CameraScript _cameraScript;


        [SerializeField] private GameObject _optionScene;
        [SerializeField] private GameObject _howtoScreen;

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
                case ButtonClickType.Howto:
                    ClickHowto();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
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
            _cameraScript.MouseSensitivity = _cameraSensitivitySlider.value * 5;
            _playerScript.RotationSpeed = _rotationSensitivitySlider.value * 10;
            BgmManager.Instance.BGMVolume = _bgmSlider.value;
            BgmManager.Instance.SFXVolume = _sfxSlider.value;
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
