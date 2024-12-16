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

        public int StageNumber { get => stageNumber; set => stageNumber = value; }


        public void Awake()
        {
            //UIManager.Instance.Initialize(this);
            //UIManager.Instance.ShowCanvas("BeforeStart");
            //InitializeStage(1);
        }


        private void HandleGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
            StageManager stageManager = StageManager.Instance;

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
                    GameState = GameState.Playing;
                    break;
            }
        }

        private void InitializeStage()
        {
            LoadScene(stageNumber);
            StageManager stageManager = StageManager.Instance;
            if (stageManager != null)
            {
                stageManager.InitializeStage();
            }
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
                    GameState = GameState.Paused;
                    break;
                case "Resume":
                    GameState = GameState.Playing;
                    break;
                case "Reset":
                    ResetStage();
                    break;
                case "NextStage":
                    ProceedToNextStage();
                    break;
                case "Finish":
                    GameState = GameState.GameOver;
                    break;
                default:
                    break;
            }
        }
        public void OnUIButtonClicked(int stageNum)
        {
            stageNumber = stageNum;
            GameState = GameState.Playing;
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
                    GameState = GameState.GameOver;
                    break;
                case "StageCleared":
                    GameState = GameState.StageCleared;
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
                StageManager stageManager = StageManager.Instance;
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
    }
}
