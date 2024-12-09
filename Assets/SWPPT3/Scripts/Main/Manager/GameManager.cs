using SWPPT3.Main.Utility.Singleton;
using System;
using SWPPT3.Main.StageDirector;
using UnityEditor.SceneManagement;
using UnityEngine;

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

        public void Awake()
        {
            InitializeStage(stageNumber);
        }

        [SerializeField] private int stageNumber;

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
                default:
                    break;
            }
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
    }
}
