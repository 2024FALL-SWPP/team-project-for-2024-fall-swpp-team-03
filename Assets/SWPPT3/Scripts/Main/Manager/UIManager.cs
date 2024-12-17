using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
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

        [Header("Image for start")]
        public GameObject _logo;
        public GameObject _exitGame;

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

        // [Header("stage Button")]
        // private Button _resumeButton_PauseMenu;
        // private Button _restartButton_PauseMenu;
        //
        // private Button _outButton_PauseMenu;
        //
        // // FailScreen Buttons
        // private Button _restartButton_FailedMenu;
        //
        // private Button _outButton_FailedMenu;
        //
        // // ClearScreen Buttons
        // private Button _nextButton_ClearedMenu;
        // private Button _outButton_ClearedMenu;
        //
        // private Button _exitButton_StartMenu;
        // private Button _returnButton_StartMenu;

        private TextMeshProUGUI _playTimeTmp;
        private TextMeshProUGUI _metalNumTmp;
        private TextMeshProUGUI _rubberNumTmp;

        private Player _playerScript;

        private void Awake()
        {
            // Debug.Log("awake");
            // _stage1Button = mainCanvas.transform.Find("Stage1Button")?.gameObject;
            // _stage2Button = mainCanvas.transform.Find("Stage2Button")?.gameObject;
            // _stage3Button = mainCanvas.transform.Find("Stage3Button")?.gameObject;
            // _stage4Button = mainCanvas.transform.Find("Stage4Button")?.gameObject;
            // _stage5Button = mainCanvas.transform.Find("Stage5Button")?.gameObject;
            // _tutorial1Button = mainCanvas.transform.Find("Tutorial1Button")?.gameObject;
            // _tutorial2Button = mainCanvas.transform.Find("Tutorial2Button")?.gameObject;
            //
            // _logo           = mainCanvas.transform.Find("Logo")?.gameObject;
            // _exitGame       = mainCanvas.transform.Find("ExitGame")?.gameObject;
            //
            // _pauseButton = mainCanvas.transform.Find("PauseButton")?.gameObject;
            //
            // _itemState      = mainCanvas.transform.Find("ItemState")?.gameObject;
            // _metalText      = mainCanvas.transform.Find("MetalText")?.gameObject;
            // _rubberText     = mainCanvas.transform.Find("RubberText")?.gameObject;
            // _metalNum       = mainCanvas.transform.Find("MetalNum")?.gameObject;
            // _rubberNum      = mainCanvas.transform.Find("RubberNum")?.gameObject;
            // _playTime       = mainCanvas.transform.Find("Playtime")?.gameObject;
            // _pauseScreen    = mainCanvas.transform.Find("PauseScreen")?.gameObject;
            // _failScreen     = mainCanvas.transform.Find("FailScreen")?.gameObject;
            // _clearScreen    = mainCanvas.transform.Find("ClearScreen")?.gameObject;
            // _loadingScreen  = mainCanvas.transform.Find("LoadingScreen")?.gameObject;
            // _startScreen    = mainCanvas.transform.Find("StartScreen")?.gameObject;
            // _radialUI       = mainCanvas.transform.Find("RadialUI")?.gameObject;

            // _resumeButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/ResumeButton").GetComponent<Button>();
            // _restartButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/RestartButton").GetComponent<Button>();
            // _outButton_PauseMenu = _pauseScreen.transform.Find("PausedMenu/OutButton").GetComponent<Button>();
            // // Fail Screen
            // _restartButton_FailedMenu = _failScreen.transform.Find("FailedMenu/RestartButton").GetComponent<Button>();
            // _outButton_FailedMenu = _failScreen.transform.Find("FailedMenu/OutButton").GetComponent<Button>();
            // // Clear Screen
            // _nextButton_ClearedMenu = _clearScreen.transform.Find("ClearedMenu/NextButton").GetComponent<Button>();
            // _outButton_ClearedMenu = _clearScreen.transform.Find("ClearedMenu/OutButton").GetComponent<Button>();
            // // Start Screen
            // _exitButton_StartMenu = _exitGame.transform.Find("StartMenu/ExitButton").GetComponent<Button>();
            // _returnButton_StartMenu = _exitGame.transform.Find("StartMenu/ReturnButton").GetComponent<Button>();

            _playTimeTmp = _playTime.transform.Find("PlaytimeText").GetComponent<TextMeshProUGUI>();
            _metalNumTmp = _metalNum.GetComponent<TextMeshProUGUI>();
            _rubberNumTmp = _rubberNum.GetComponent<TextMeshProUGUI>();

            // _playerScript = FindObjectOfType<Player>();
            HideAllUI();
            ShowStartStage();
            Debug.Log("initinalize ");

            // InitializeButtons();
        }

        // private void InitializeButtons()
        // {
        //     _pauseButton.GetComponent<Button>().onClick.AddListener(clickPause);
        //     _tutorial1Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(1));
        //     _tutorial2Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(2));
        //     _stage1Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(3));
        //     _stage2Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(4));
        //     _stage3Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(7));
        //     _stage4Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(5));
        //     _stage5Button.GetComponent<Button>().onClick.AddListener(() => StageSelect(6));
        //     _resumeButton_PauseMenu.onClick.AddListener(clickResume);
        //     _restartButton_PauseMenu.onClick.AddListener(clickReStart);
        //     _outButton_PauseMenu.onClick.AddListener(clickStartScene);
        //     _restartButton_FailedMenu.onClick.AddListener(clickReStart);
        //     _outButton_FailedMenu.onClick.AddListener(clickStartScene);
        //     _nextButton_ClearedMenu.onClick.AddListener(clickNext);
        //     _outButton_ClearedMenu.onClick.AddListener(clickStartScene);
        //     _exitButton_StartMenu.onClick.AddListener(exitGame);
        //     _returnButton_StartMenu.onClick.AddListener(returnStart);
        // }

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
            // Invoke("showPlayingScreen", 2);
        }

        public void showPlayingScreen()
        {
            _startScreen.SetActive(false);
            _loadingScreen.SetActive(false);
            _pauseButton.SetActive(true);
            _itemState.SetActive(true);
            _metalText.SetActive(true);
            _rubberText.SetActive(true);
            _metalNum.SetActive(true);
            _rubberNum.SetActive(true);
            _playTime.SetActive(true);
            MetalNumUpdate(_playerScript.Item[PlayerStates.Metal]);
            RubberNumUpdate(_playerScript.Item[PlayerStates.Rubber]);
        }

        public void IntializePlayer(Player player)
        {
            _playerScript = player;
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

        public void clickPause()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                _pauseScreen.SetActive(true);
                GameManager.Instance.GameState = GameState.Paused;
            }
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

        public void clickNext()
        {
            HideScreen();
            GameManager.Instance.StageNumber++;
            GameManager.Instance.GameState = GameState.Playing;
            GameManager.Instance.LoadScene();
        }

        public void clickResume()
        {
            HideScreen();
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void clickReStart()
        {
            HideScreen();
            GameManager.Instance.LoadScene();
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void clickStartScene()
        {
            HideScreen();
            GameManager.Instance.GameState = GameState.BeforeStart;
            GameManager.Instance.StageNumber = 0;
            SceneManager.LoadScene("Start");
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

        public void ShowRadialUI()
        {
            _radialUI.SetActive(true);
            if (_playerScript.Item[PlayerStates.Rubber] == 0)
            {
                _radialUI.transform.Find("RightButton").gameObject.SetActive(false);
            }

            if (_playerScript.Item[PlayerStates.Metal] == 0)
            {
                _radialUI.transform.Find("LeftButton").gameObject.SetActive(false);
            }
        }

        public void HideRadialUI()
        {
            _radialUI.SetActive(false);
        }

        public void returnStart()
        {
            GameManager.Instance.GameState = GameState.BeforeStart;
            _exitGame.SetActive(false);
        }

        public void exitGame()
        {
            Application.Quit();
        }

        public void StageSelect(int stageNum)
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                // HideStartStage();
                GameManager.Instance.StageSelect(stageNum);
            }
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

