
#region

using System;
using SWPPT3.Main.StageDirector;
using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#endregion

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
        OnOption,
        OnHowto
    }

    public class GameManager : MonoSingleton<GameManager>
    {
        public event Action<GameState, GameState> OnGameStateChanged;

        private GameState _gameState;
        public GameState GameState
        {
            get => _gameState;
            set
            {
                if (_gameState != value)
                {
                    HandleGameStateChanged(_gameState, value);
                }
            }
        }
        private int _stageNumber;

        public int StageNumber { get => _stageNumber; set => _stageNumber = value; }

        public StageManager StageManager;
        [SerializeField] private UnityEvent<bool> _onTryingLoadStatusChanged;


        public void Awake()
        {
            _onTryingLoadStatusChanged.Invoke(false);
            Debug.Log("Game Manager Awake");

            SceneManager.LoadScene("Start");
        }

        public void OnDestroy()
        {
            Debug.Log("Game Manager OnDestroy");
        }


        private void HandleGameStateChanged(GameState oldState, GameState newState)
        {
            _gameState = newState;
            OnGameStateChanged?.Invoke(oldState,newState);

            switch (newState)
            {
                case GameState.BeforeStart:
                    break;
                case GameState.Ready:
                    _onTryingLoadStatusChanged.Invoke(true);
                    LoadScene();
                    break;
                case GameState.Playing:
                    _onTryingLoadStatusChanged.Invoke(false);
                    StageManager.ResumeStage();
                    break;
                case GameState.Paused:
                    StageManager?.PauseStage();
                    break;
                case GameState.GameOver:
                    StageManager?.FailStage();
                    break;
                case GameState.OnChoice:
                    StageManager?.PauseStage();
                    break;
                case GameState.Exit:
                    break;
                case GameState.StageCleared:
                    StageManager?.ClearStage();
                    break;
                case GameState.OnOption:
                    break;
                case GameState.OnHowto:
                    break;
            }
        }

        public void StageSelect(int stageNum)
        {
            _stageNumber = stageNum;
            GameState = GameState.Ready;
        }

        public void LoadScene()
        {
            string sceneName;

            switch (_stageNumber)
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
                    Debug.LogError($"Invalid stage number: {_stageNumber}");
                    return;
            }
            SceneManager.LoadScene(sceneName);
        }


    }
}

