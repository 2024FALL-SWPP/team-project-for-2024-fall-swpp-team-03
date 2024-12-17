using System;
using UnityEngine;
using UnityEngine.UI;
using SWPPT3.Main.Manager;
using SWPPT3.Main.Utility.Singleton;
using System.Collections;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Serialization.ObjectGraphVisitors;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {

        private GameManager _gameManager;

        [SerializeField] private GameObject mainCanvas;

        [Header("Button for start")]
        private GameObject _stage1Button;
        private GameObject _stage2Button;
        private GameObject _stage3Button;
        private GameObject _stage4Button;
        private GameObject _stage5Button;
        private GameObject _tutorial1Button;
        private GameObject _tutorial2Button;

        [Header("Image for start")]
        private GameObject _logo;
        private GameObject _exitGame;

        [Header("Button for stage")]
        private GameObject _pauseButton;

        [Header("Image for stage")]
        private GameObject _itemState;
        private GameObject _metalText;
        private GameObject _rubberText;
        private GameObject _metalNum;
        private GameObject _rubberNum;
        private GameObject _playTime;
        private GameObject _pauseScreen;
        private GameObject _failScreen;
        private GameObject _clearScreen;
        private GameObject _loadingScreen;
        private GameObject _startScreen;
        private GameObject _radialUI;

        [Header("stage Button")]
        private Button _resumeButton_PauseMenu;
        private Button _restartButton_PauseMenu;

        private Button _outButton_PauseMenu;

        // FailScreen Buttons
        private Button _restartButton_FailedMenu;

        private Button _outButton_FailedMenu;

        // ClearScreen Buttons
        private Button _nextButton_ClearedMenu;
        private Button _outButton_ClearedMenu;

        private Button _exitButton_StartMenu;
        private Button _returnButton_StartMenu;

        private TextMeshProUGUI _playTimeTmp;
        private TextMeshProUGUI _metalNumTmp;
        private TextMeshProUGUI _rubberNumTmp;



        private float introDuration = 1.0f;

        private void Awake()
        {
            _stage1Button = mainCanvas.transform.Find("stage1Button")?.gameObject;
            _stage2Button = mainCanvas.transform.Find("stage2Button")?.gameObject;
            _stage3Button = mainCanvas.transform.Find("stage3Button")?.gameObject;
            _stage4Button = mainCanvas.transform.Find("stage4Button")?.gameObject;
            _stage5Button = mainCanvas.transform.Find("stage5Button")?.gameObject;
            _tutorial1Button = mainCanvas.transform.Find("tutorial1Button")?.gameObject;
            _tutorial2Button = mainCanvas.transform.Find("tutorial2Button")?.gameObject;

            _logo           = mainCanvas.transform.Find("Logo")?.gameObject;
            _exitGame       = mainCanvas.transform.Find("ExitGame")?.gameObject;

            _pauseButton = mainCanvas.transform.Find("pauseButton")?.gameObject;

            _itemState      = mainCanvas.transform.Find("ItemState")?.gameObject;
            _metalText      = mainCanvas.transform.Find("MetalText")?.gameObject;
            _rubberText     = mainCanvas.transform.Find("RubberText")?.gameObject;
            _metalNum       = mainCanvas.transform.Find("MetalNum")?.gameObject;
            _rubberNum      = mainCanvas.transform.Find("RubberNum")?.gameObject;
            _playTime       = mainCanvas.transform.Find("PlayTime")?.gameObject;
            _pauseScreen    = mainCanvas.transform.Find("PauseScreen")?.gameObject;
            _failScreen     = mainCanvas.transform.Find("FailScreen")?.gameObject;
            _clearScreen    = mainCanvas.transform.Find("ClearScreen")?.gameObject;
            _loadingScreen  = mainCanvas.transform.Find("LoadingSreen")?.gameObject;
            _startScreen    = mainCanvas.transform.Find("StartScreen")?.gameObject;
            _radialUI       = mainCanvas.transform.Find("RadialUI")?.gameObject;

            _resumeButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/ResumeButton").GetComponent<Button>();
            _restartButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/RestartButton").GetComponent<Button>();
            _outButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/OutButton").GetComponent<Button>();
            // Fail Screen
            _restartButton_FailedMenu = _failScreen.transform.Find("FailedMenu/RestartButton").GetComponent<Button>();
            _outButton_FailedMenu = _failScreen.transform.Find("FailedMenu/OutButton").GetComponent<Button>();
            // Clear Screen
            _nextButton_ClearedMenu = _clearScreen.transform.Find("ClearedMenu/NextButton").GetComponent<Button>();
            _outButton_ClearedMenu = _clearScreen.transform.Find("ClearedMenu/OutButton").GetComponent<Button>();
            // Start Screen
            _exitButton_StartMenu = _exitGame.transform.Find("StartMenu/ExitButton").GetComponent<Button>();
            _returnButton_StartMenu = _exitGame.transform.Find("StartMenu/ReturnButton").GetComponent<Button>();

            _playTimeTmp = _playTime.transform.Find("PlayTimeText").GetComponent<TextMeshProUGUI>();
            _metalNumTmp = _metalNum.GetComponent<TextMeshProUGUI>();
            _rubberNumTmp = _rubberNum.transform.Find("PlayTimeText").GetComponent<TextMeshProUGUI>();

            InitializeButtons();
        }

        private void InitializeButtons()
        {

            _pauseButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClicked("Pause"));
            _tutorial1Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(1));
            _tutorial2Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(2));
            _stage1Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(3));
            _stage2Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(4));
            _stage3Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(7));
            _stage4Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(5));
            _stage5Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(6));
            _resumeButton_PauseMenu.onClick.AddListener(() => OnButtonClicked("Resume"));
            _restartButton_PauseMenu.onClick.AddListener(() => OnButtonClicked("Restart"));
            _outButton_PauseMenu.onClick.AddListener(() => OnButtonClicked("StartMenu"));
            _restartButton_FailedMenu.onClick.AddListener(() => OnButtonClicked("Restart"));
            _outButton_FailedMenu.onClick.AddListener(() => OnButtonClicked("StartMenu"));
            _nextButton_ClearedMenu.onClick.AddListener(() => OnButtonClicked("NextStage"));
            _outButton_ClearedMenu.onClick.AddListener(() => OnButtonClicked("StartMenu"));
            _exitButton_StartMenu.onClick.AddListener(()=> OnButtonClicked("GameFinish"));
            _returnButton_StartMenu.onClick.AddListener(() => OnButtonClicked("ReturnStart"));
        }

        private void HideAllUI()
        {
            HideScreen();
            HideExitGame();
            HideStartStage();
            HidePlayingScreen();
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
            _logo.SetActive(true);
        }

        public void HideStartStage()
        {
            _stage1Button.SetActive(false);
            _stage2Button.SetActive(false);
            _stage3Button.SetActive(false);
            _stage4Button.SetActive(false);
            _stage5Button.SetActive(false);
            _tutorial1Button.SetActive(false);
            _tutorial2Button.SetActive(false);
            _logo.SetActive(false);
        }

        public void ShowPlaying()
        {
            _startScreen.SetActive(true);
            _loadingScreen.SetActive(true);
            Invoke("ShowPlayingScreen", 2);
            _startScreen.SetActive(false);
            _loadingScreen.SetActive(false);
        }

        public void ShowPlayingScreen()
        {
            _pauseButton.SetActive(true);
            _itemState.SetActive(true);
            _metalText.SetActive(true);
            _rubberText.SetActive(true);
            _metalNum.SetActive(true);
            _rubberNum.SetActive(true);
            _playTime.SetActive(true);
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

        public void ShowPause()
        {
            HideScreen();
            _pauseScreen.SetActive(true);

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

        public void ShowExitGame()
        {
            _exitGame.SetActive(true);
        }

        public void HideExitGame()
        {
            _exitGame.SetActive(false);
        }

        public void ShowRadialUI()
        {
            _radialUI.SetActive(true);
        }

        public void HideRadialUI()
        {
            _radialUI.SetActive(false);
        }

        public void ReturnStart()
        {
            _exitGame.SetActive(false);
        }
        public void



        private void OnButtonClicked(string buttonName)
        {
        }

        public void StageSelect(int stageNum)
        {
            GameManager.Instance.StageSelect(stageNum);
        }
        public void PlayTimeUpdate(int time){
            int min = time/60;
            int sec = time%60;
            _playTimeTmp.text = min+":"+sec;
        }
        public void MetalNumUpdate(int num){
            _metalNumTmp.text = num.ToString();
        }
        public void RubberNumUpdate(int num){
            _rubberNumTmp.text = num.ToString();
        }
    }
}

