using SWPPT3.Main.Utility.Singleton;
using System;
using System.Collections.Generic;
using SWPPT3.Main.StageDirector;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.Manager
{
    public enum GameState
    {
        BeforeStart,
        Ready,
        Playing,
        Paused,
        OnChoice,
        GameOver,
        Exit,
        StageCleared,
        OnOption
    }

    public class GameManager : MonoSingleton<GameManager>
    {
        public event Action<GameState> OnGameStateChanged;

        private GameState gameState;
        public GameState GameState
        {
            get => gameState;
            set
            {
                if (gameState != value)
                {
                    Debug.Log($"{gameState} {value}");
                    gameState = value;
                    HandleGameStateChanged(gameState);
                }
            }
        }
        private int stageNumber;

        public int StageNumber { get => stageNumber; set => stageNumber = value; }

        private StageManager _stageManager;


        public void Awake()
        {

        }


        private void HandleGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.BeforeStart:
                    UIManager.Instance.ShowStartStage();
                    break;
                case GameState.Ready:
                    LoadScene();
                    break;
                case GameState.Playing:
                    _stageManager.ResumeStage();
                    break;
                case GameState.Paused:
                    _stageManager?.PauseStage();
                    break;
                case GameState.GameOver:
                    _stageManager?.FailStage();
                    UIManager.Instance.ShowFail();
                    break;
                case GameState.OnChoice:
                    _stageManager?.PauseStage();
                    break;
                case GameState.Exit:
                    break;
                case GameState.StageCleared:
                    _stageManager?.ClearStage();
                    ProceedToNextStage();
                    UIManager.Instance.ShowClear();
                    break;
                case GameState.OnOption:
                    break;
            }
        }

        private void InitializeStage()
        {
            //loadingscene을 켜고
            LoadScene();
            if (_stageManager != null)
            {
                _stageManager.InitializeStage();
            }
            //loadingscene 끄기
        }

        public void ProceedToNextStage()
        {
            stageNumber++;
        }

        public void ResetStage()
        {
            InitializeStage();
        }
        //
        // public void OnUIButtonClicked(string buttonName)
        // {
        //     switch (buttonName)
        //     {
        //         case "Pause":
        //             GameState = GameState.Paused;
        //             break;
        //         case "Resume":
        //             GameState = GameState.Playing;
        //             break;
        //         case "Restart":
        //             ResetStage();
        //             break;
        //         case "NextStage":
        //             ProceedToNextStage();
        //             break;
        //         case "StartMenu":
        //             break;
        //         case "GameFinish":
        //             Application.Quit();
        //             break;
        //         case "ReturnStart":
        //             UIManager.Instance.ReturnStart();
        //             break;
        //         default:
        //             break;
        //     }
        // }
        public void StageSelect(int stageNum)
        {
            stageNumber = stageNum;
            GameState = GameState.Ready;
        }

        public void LoadScene()
        {
            UIManager.Instance.ShowPlaying();
            string sceneName;
            // if (stageNum > 2)
            // {
            //      sceneName = $"Stage{stageNum-2}";
            // }
            // else
            // {
            //     sceneName = $"Tutorial{stageNum}";
            // }
            switch (stageNumber)
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
                    Debug.LogError($"Invalid stage number: {stageNumber}");
                    return;
            }
            SceneManager.LoadScene(sceneName);
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
            if (GameState == GameState.Ready)
            {
                _stageManager = StageManager.Instance;
                GameState = GameState.Playing;
                if (_stageManager != null)
                {
                    _stageManager.InitializeStage();
                    _stageManager.StartStage();
                    UIManager.Instance.IntializePlayer(_stageManager.Player);
                    UIManager.Instance.ShowPlayingScreen();
                }
                else
                {
                    Debug.LogError("Stage Manager is null after loading scene");
                }
            }
        }
    }
}
