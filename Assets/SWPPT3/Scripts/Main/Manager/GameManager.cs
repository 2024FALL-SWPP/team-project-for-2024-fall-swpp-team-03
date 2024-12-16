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

        public int StageNumber
        {
            get => stageNumber;
            set { stageNumber = value; }
        }


        public void Awake()
        {
            //UIManager.Instance.Initialize(this);
            //UIManager.Instance.ShowCanvas("BeforeStart");
            //InitializeStage(1);
        }


        private void HandleGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
            StageManager stageManager = getStageManager();

            switch (newState)
            {
                case GameState.BeforeStart:
                    break;
                case GameState.Playing:
                    LoadScene(stageNumber);
                    break;
                case GameState.Paused:
                    stageManager?.PauseStage();
                    break;
                case GameState.GameOver:
                    stageManager?.FailStage();
                    break;
                case GameState.StageCleared:
                    stageManager?.ClearStage();
                    ProceedToNextStage();
                    SetGameState(GameState.Playing);
                    break;
            }
        }

        private void InitializeStage()
        {
            LoadScene(stageNumber);
            StageManager stageManager = getStageManager();
            if (stageManager != null)
            {
                stageManager.InitializeStage();
            }
            else
            {
                Debug.Log("Stage Manager is null");
            }
        }

        private StageManager getStageManager()
        {
            StageManager stageManager = null;

            switch (stageNumber)
            {
                case 1:
                    stageManager = Tutorial1Director.Instance;
                    break;
                case 2:
                    stageManager = Tutorial2Director.Instance;
                    break;
                case 3:
                    stageManager = Stage1Director.Instance;
                    break;
                case 4:
                    stageManager = Stage2Director.Instance;
                    break;
                case 5:
                    stageManager = Stage4Director.Instance;
                    break;
                case 6:
                    stageManager = Stage5Director.Instance;
                    break;
                case 7:
                    stageManager = Stage3Director.Instance;
                    break;
                default:
                    break;
            }

            return stageManager;
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
            InitializeStage();
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
            stageNumber = stageNum;
            SetGameState(GameState.Playing);
        }

        public void LoadScene(int stageNum)
        {
            string sceneName;
            // if (stageNum > 2)
            // {
            //      sceneName = $"Stage{stageNum-2}";
            // }
            // else
            // {
            //     sceneName = $"Tutorial{stageNum}";
            // }
            switch (stageNum)
            {
                case 1:
                    sceneName = "Tutorial1test";
                    break;
                case 2:
                    sceneName = "Tutorial2test";
                    break;
                case 3:
                    sceneName = "Stage1test";
                    break;
                case 4:
                    sceneName = "Stage2test";
                    break;
                case 5:
                    sceneName = "Stage4test";
                    break;
                case 6:
                    sceneName = "Stage5test";
                    break;
                case 7:
                    sceneName = "Stage3test";
                    break;
                default:
                    Debug.LogError($"Invalid stage number: {stageNum}");
                    return;
            }
            SceneManager.LoadScene(sceneName);
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
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (GameState == GameState.Playing)
            {
                StageManager stageManager = getStageManager();
                if (stageManager != null)
                {
                    stageManager.InitializeStage();
                    stageManager.StartStage();
                }
                else
                {
                    Debug.LogError("Stage Manager is null after loading scene");
                }
            }
        }
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // private static void EnsureInitialized()
        // {
        //     if (Instance == null)
        //     {
        //         var obj = new GameObject(nameof(GameManager));
        //         var manager = obj.AddComponent<GameManager>();
        //
        //         var uiObj = new GameObject(nameof(UIManager));
        //         uiObj.AddComponent<UIManager>();
        //
        //         var inputObj = new GameObject(nameof(InputManager));
        //         inputObj.AddComponent<InputManager>();
        //     }
        // }
    }
}
