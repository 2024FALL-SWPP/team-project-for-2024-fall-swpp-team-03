using SWPPT3.Main.Utility.Singleton;
using System;
using SWPPT3.Main.StageDirector;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.Manager
{
    public enum GameState
    {
        BeforeStart,
        Playing,
        Paused,
        GameOver,
        StageCleared
    }

    public class GameManager : MonoSingleton<GameManager>
    {
        public event Action<GameState> OnGameStateChanged;

        private GameState gameState;
        public GameState GameState
        {
            get => gameState;
            private set
            {
                if (gameState != value)
                {
                    gameState = value;
                    HandleGameStateChanged(gameState);
                }
            }
        }
        private int stageNumber;

        public void Awake()
        {
            UIManager.Instance.Initialize(this);
            UIManager.Instance.ShowCanvas("BeforeStart");
        }


        private void HandleGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
            var stageManager = StageManager.Instance;

            if (stageManager == null)  { return; }
            switch (newState)
            {
                case GameState.BeforeStart:
                    break;
                case GameState.Playing:
                    stageManager.StartStage();
                    break;
                case GameState.Paused:
                    stageManager.PauseStage();
                    break;
                case GameState.GameOver:
                    stageManager.FailStage();
                    break;
                case GameState.StageCleared:
                    stageManager.ClearStage();
                    stageNumber++;
                    InitializeStage(stageNumber);
                    break;
            }
        }

        private void InitializeStage(int stageNumber)
        {
            StageManager stageManager = null;

            switch (stageNumber)
            {
                case 1:
                    stageManager = Stage1Director.Instance;
                    break;
                case 2:
                    stageManager = Stage2Director.Instance;
                    break;
                case 3:
                    stageManager = Stage3Director.Instance;
                    break;
                case 4:
                    stageManager = Stage4Director.Instance;
                    break;
                case 5:
                    stageManager = Stage5Director.Instance;
                    break;
                case 6:
                    stageManager = Tutorial1Director.Instance;
                    break;
                case 7:
                    stageManager = Tutorial2Director.Instance;
                    break;
                default:
                    break;
            }

            if (stageManager != null)
            {
                stageManager.InitializeStage();
            }

            SetGameState(GameState.BeforeStart);
        }


        public void SetGameState(GameState newState)
        {
            GameState = newState;
        }

        public void ProceedToNextStage()
        {
            stageNumber++;
        }

        public void ResetStage()
        {
            InitializeStage(stageNumber);
        }

        public void OnUIButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Pause":
                    SetGameState(GameState.Paused);
                    break;
                case "Resume":
                    SetGameState(GameState.Playing);
                    break;
                case "Reset":
                    ResetStage();
                    break;
                case "NextStage":
                    ProceedToNextStage();
                    break;
                case "Finish":
                    SetGameState(GameState.GameOver);
                    break;
                default:
                    break;
            }
        }
        public void OnUIButtonClicked(int stageNum)
        {
            //Debug.Log("GameManager button clicked");
            stageNumber = stageNum;
            string sceneName;
            if (stageNum < 6)
            {
                 sceneName = $"Stage{stageNum}test";
            }
            else
            {
                sceneName = $"Tutorial{stageNum - 5}test";
            }
            SceneManager.LoadScene(sceneName);
            SetGameState(GameState.Playing);
        }

        public void OnPlayerStateChanged(string state)
        {
            switch (state)
            {
                case "GameOver":
                    SetGameState(GameState.GameOver);
                    break;
                case "StageCleared":
                    SetGameState(GameState.StageCleared);
                    break;
                default:
                    break;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInitialized()
        {
            if (Instance == null)
            {
                var obj = new GameObject(nameof(GameManager));
                var manager = obj.AddComponent<GameManager>();

                var uiObj = new GameObject(nameof(UIManager));
                uiObj.AddComponent<UIManager>();

                var inputObj = new GameObject(nameof(InputManager));
                inputObj.AddComponent<InputManager>();
            }
        }
    }
}
