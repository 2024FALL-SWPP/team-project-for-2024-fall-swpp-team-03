using SWPPT3.Main.Utility.Singleton;
using System;
using System.Collections.Generic;
using SWPPT3.Main.StageDirector;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
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

        public StageManager _stageManager;
        [SerializeField] private UnityEvent<bool> _onTryingLoadStatusChanged;


        public void Awake()
        {
            _onTryingLoadStatusChanged.Invoke(false);
        }


        private void HandleGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.BeforeStart:
                    // UIManager.Instance.ShowStartStage();
                    break;
                case GameState.Ready:
                    _onTryingLoadStatusChanged.Invoke(true);
                    LoadScene();
                    break;
                case GameState.Playing:
                    _onTryingLoadStatusChanged.Invoke(false);
                    _stageManager.ResumeStage();
                    break;
                case GameState.Paused:
                    _stageManager?.PauseStage();
                    break;
                case GameState.GameOver:
                    _stageManager?.FailStage();
                    // UIManager.Instance.ShowFail();
                    break;
                case GameState.OnChoice:
                    _stageManager?.PauseStage();
                    break;
                case GameState.Exit:
                    break;
                case GameState.StageCleared:
                    _stageManager?.ClearStage();
                    // UIManager.Instance.ShowClear();
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

        public void ResetStage()
        {
            InitializeStage();
        }

        public void StageSelect(int stageNum)
        {
            stageNumber = stageNum;
            GameState = GameState.Ready;
        }

        public void LoadScene()
        {
            string sceneName;

            switch (stageNumber)
            {
                case 0:
                    sceneName = "Tutorial 1";
                    break;
                case 1:
                    sceneName = "Tutorial 2";
                    break;
                case 2:
                    sceneName = "Stage 1";
                    break;
                case 3:
                    sceneName = "Stage 2";
                    break;
                case 4:
                    sceneName = "Stage 4";
                    break;
                case 5:
                    sceneName = "Stage 5";
                    break;
                case 6:
                    sceneName = "Stage 3";
                    break;
                default:
                    Debug.LogError($"Invalid stage number: {stageNumber}");
                    return;
            }
            SceneManager.LoadScene(sceneName);
        }


    }
}
