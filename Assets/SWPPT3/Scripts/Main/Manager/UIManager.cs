using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Utility;
using UnityEngine;
using UnityEngine.UI;
using SWPPT3.Main.Utility.Singleton;
using TMPro;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {

        // [SerializeField] private GameObject mainCanvas;

        [Header("Button for start")]
        public GameObject _stage1Button;
        public GameObject _stage2Button;
        public GameObject _stage3Button;
        public GameObject _stage4Button;
        public GameObject _stage5Button;
        public GameObject _tutorial1Button;
        public GameObject _tutorial2Button;
        public GameObject _optionButton;

        [Header("Image for start")]
        public GameObject _logo;
        public GameObject _exitGame;
        public GameObject _optionScene;

        [Header("Button for stage")]
        public GameObject _pauseButton;

        [Header("Image for stage")]
        public GameObject _itemState;
        public GameObject _metalText;
        public GameObject _rubberText;
        public GameObject _metalNum;
        public GameObject _rubberNum;
        public GameObject _playTime;
        public GameObject _pauseScreen;
        public GameObject _failScreen;
        public GameObject _clearScreen;
        public GameObject _loadingScreen;
        public GameObject _startScreen;
        public GameObject _radialUI;

        private TextMeshProUGUI _playTimeTmp;
        private TextMeshProUGUI _metalNumTmp;
        private TextMeshProUGUI _rubberNumTmp;

        private Player _player;

        private Slider _bgmSlider;
        private Slider _sfxSlider;
        private Slider _cameraSensitivitySlider;
        private Slider _rotationSensitivitySlider;

        [SerializeField] CameraScript _cameraScript;
        [SerializeField] PlayerScript _playerScript;

        private bool setOption;

        private void Awake()
        {
            setOption = false;
            var parentSlider = _optionScene.transform.Find("VerticalAlign");
            _bgmSlider = parentSlider.Find("BGMSlider").GetComponent<Slider>();
            _sfxSlider = parentSlider.Find("SoundEffectSlider").GetComponent<Slider>();
            _cameraSensitivitySlider = parentSlider.Find("CameraSensitivitySlider").GetComponent<Slider>();
            _rotationSensitivitySlider = parentSlider.Find("RotationSensitivitySlider").GetComponent<Slider>();

            _playTimeTmp = _playTime.transform.Find("PlaytimeText").GetComponent<TextMeshProUGUI>();
            _metalNumTmp = _metalNum.GetComponent<TextMeshProUGUI>();
            _rubberNumTmp = _rubberNum.GetComponent<TextMeshProUGUI>();

            HideAllUI();
            ShowStartStage();
            Debug.Log("initinalize ");

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
                InputManager.Instance.OnStartTransform += HandleTransform;
            }
            else
            {
                Debug.LogError("InputManager is null");
            }

            // InitializeButtons();
        }

        public void Update()
        {
            // Debug.Log("update");
            if (setOption)
            {
                _cameraScript.MouseSensitivity = _cameraSensitivitySlider.value;
                _playerScript.RotationSpeed = _rotationSensitivitySlider.value;
                // Debug.Log($"{_playerScript.RotationSpeed} {_rotationSensitivitySlider.value}");
            }
        }

        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                GameManager.Instance.GameState = GameState.Exit;
                ShowExitGame();
            }
            else if (GameManager.Instance.GameState == GameState.Playing)
            {
                GameManager.Instance.GameState = GameState.Paused;
                UIManager.Instance._pauseScreen.SetActive(true);
                Debug.Log($"{_pauseScreen == null}");
            }
        }

        private void HandleTransform(bool isClick)
        {
            if (GameManager.Instance.GameState == GameState.Playing && isClick)
            {
                GameManager.Instance.GameState = GameState.OnChoice;
                ShowRadialUI();
            }
            else if (GameManager.Instance.GameState == GameState.OnChoice && !isClick)
            {
                GameManager.Instance.GameState = GameState.Playing;
                HideRadialUI();
            }
        }


        private void HideAllUI()
        {
            HideScreen();
            HideExitGame();
            HideStartStage();
            HidePlayingScreen();
            HideOption();
            HideRadialUI();
            HidePlaying();
        }

        public void ShowStartStage()
        {
            _stage1Button.SetActive(true);
            _stage2Button.SetActive(true);
            _stage3Button.SetActive(true);
            _stage4Button.SetActive(true);
            _stage5Button.SetActive(true);
            _tutorial1Button.SetActive(true);
            _tutorial2Button.SetActive(true);
            _optionButton.SetActive(true);
            _logo.SetActive(true);
        }

        private void HideStartStage()
        {
            _stage1Button.SetActive(false);
            _stage2Button.SetActive(false);
            _stage3Button.SetActive(false);
            _stage4Button.SetActive(false);
            _stage5Button.SetActive(false);
            _tutorial1Button.SetActive(false);
            _tutorial2Button.SetActive(false);
            _optionButton.SetActive(false);
            _logo.SetActive(false);
        }

        public void ShowPlaying()
        {
            _startScreen.SetActive(true);
            _loadingScreen.SetActive(true);
        }

        public void HidePlaying()
        {
            _startScreen.SetActive(false);
            _loadingScreen.SetActive(false);
        }

        public void ShowPlayingScreen()
        {
            HidePlaying();
            _pauseButton.SetActive(true);
            _itemState.SetActive(true);
            _metalText.SetActive(true);
            _rubberText.SetActive(true);
            _metalNum.SetActive(true);
            _rubberNum.SetActive(true);
            _playTime.SetActive(true);
            NumUpdate();
        }

        public void IntializePlayer(Player player)
        {
            _player = player;
            _player.OnItemChanged += NumUpdate;
        }

        public void HidePlayingScreen()
        {
            _itemState.SetActive(false);
            _metalText.SetActive(false);
            _rubberText.SetActive(false);
            _metalNum.SetActive(false);
            _rubberNum.SetActive(false);
            _pauseButton.SetActive(false);
            _playTime .SetActive(false);
        }

        public void ShowClear()
        {
            HideScreen();
            _clearScreen.SetActive(true);
        }

        public void ShowFail()
        {
            HideScreen();
            _failScreen.SetActive(true);
        }

        public void HideScreen()
        {
            _pauseScreen.SetActive(false);
            _failScreen.SetActive(false);
            _clearScreen.SetActive(false);
        }

        public void ClickPause()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                _pauseScreen.SetActive(true);
                GameManager.Instance.GameState = GameState.Paused;
            }
        }

        public void ClickNext()
        {
            HideAllUI();
            GameManager.Instance.StageNumber++;
            GameManager.Instance.GameState = GameState.Playing;
            GameManager.Instance.LoadScene();
        }

        public void ClickResume()
        {
            HideScreen();
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void ClickReStart()
        {
            HideAllUI();
            GameManager.Instance.LoadScene();
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void ClickStartScene()
        {
            HideAllUI();
            GameManager.Instance.GameState = GameState.BeforeStart;
            GameManager.Instance.StageNumber = 0;
            ShowStartStage();
            SceneManager.LoadScene("Start");
        }

        public void ClickOption()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                setOption = true;
                _optionScene.SetActive(true);
                GameManager.Instance.GameState = GameState.OnOption;
            }
        }

        //input 에 의해 바뀌는 method
        public void ShowExitGame()
        {
            _exitGame.SetActive(true);
        }

        public void HideExitGame()
        {
            _exitGame.SetActive(false);
        }

        public void HideOption()
        {
            _optionScene.SetActive(false);
        }

        public void ShowRadialUI()
        {
            _radialUI.SetActive(true);
            if (_player.Item[PlayerStates.Rubber] == 0)
            {
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(true);
            }

            if (_player.Item[PlayerStates.Metal] == 0)
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(true);
            }
        }

        public void HideRadialUI()
        {
            _radialUI.SetActive(false);
        }

        public void ReturnStart()
        {
            GameManager.Instance.GameState = GameState.BeforeStart;
            setOption = false;
            _optionScene.SetActive(false);
            _exitGame.SetActive(false);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void StageSelect(int stageNum)
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                HideStartStage();
                GameManager.Instance.StageSelect(stageNum);
            }
        }

        public void ChangeSlime()
        {
            _player.TryChangeState(PlayerStates.Slime);
        }

        public void ChangeMetal()
        {
            _player.TryChangeState(PlayerStates.Metal);
        }

        public void ChangeRubber()
        {
            _player.TryChangeState(PlayerStates.Rubber);
        }

        public void PlayTimeUpdate(int time){
            int min = time/60;
            int sec = time%60;
            _playTimeTmp.text = $"{min:D2}:{sec:D2}";
        }

        private void NumUpdate()
        {
            _metalNumTmp.text = $"{_player.Item[PlayerStates.Metal]}";
            _rubberNumTmp.text = $"{_player.Item[PlayerStates.Rubber]}";
        }

    }
}

