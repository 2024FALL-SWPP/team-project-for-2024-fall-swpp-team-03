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

        private TextMeshProUGUI _playTimeTmp;
        private TextMeshProUGUI _metalNumTmp;
        private TextMeshProUGUI _rubberNumTmp;

        private Player _playerScript;

        private void Awake()
        {
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

        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.BeforeStart)
            {
                GameManager.Instance.GameState = GameState.Exit;
                ShowExitGame();
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
        }

        public void ShowPlayingScreen()
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
            NumUpdate();
        }

        public void IntializePlayer(Player player)
        {
            _playerScript = player;
            _playerScript.OnItemChanged += NumUpdate;
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

        public void ClickPause()
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

        public void ClickNext()
        {
            HideScreen();
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
            HideScreen();
            GameManager.Instance.LoadScene();
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void ClickStartScene()
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
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(true);
            }

            if (_playerScript.Item[PlayerStates.Metal] == 0)
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
                // HideStartStage();
                GameManager.Instance.StageSelect(stageNum);
            }
        }

        public void ChangeSlime()
        {
            _playerScript.TryChangeState(PlayerStates.Slime);
        }

        public void ChangeMetal()
        {
            _playerScript.TryChangeState(PlayerStates.Metal);
        }

        public void ChangeRubber()
        {
            _playerScript.TryChangeState(PlayerStates.Rubber);
        }

        public void PlayTimeUpdate(int time){
            int min = time/60;
            int sec = time%60;
            _playTimeTmp.text = $"{min:D2}:{sec:D2}";
        }

        private void NumUpdate()
        {
            _metalNumTmp.text = $"{_playerScript.Item[PlayerStates.Metal]}";
            _rubberNumTmp.text = $"{_playerScript.Item[PlayerStates.Rubber]}";
        }

    }
}

