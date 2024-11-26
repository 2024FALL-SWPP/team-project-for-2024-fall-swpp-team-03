using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using SWPPT3.Main.Manager;
using UnityEngine.Serialization;

namespace SWPPT3.Main.StageDirector
{
    public enum StagePlayingState
    {
        BeforeStart,
        Playing,
        Paused,
        PlayerFailed,
        Cleared,
    }

    public abstract class StageDirectorBase : MonoWeakSingleton<StageDirectorBase>
    {
        // private UIInputActions _inputActions;

        [SerializeField]
        private BgmManager bgmManager;

        public StagePlayingState StagePlayingState { get; protected set; } = StagePlayingState.BeforeStart;

        public event Action<StagePlayingState> OnStageStateChanged;

        protected void Awake()
        {
            // Input Actions 초기화 및 이벤트 등록
            // _inputActions = new UIInputActions();
            // _inputActions.UI.StagePause.performed += StagePause;
        }

        private void OnEnable()
        {
            // _inputActions.UI.Enable();
        }

        private void OnDisable()
        {
            // _inputActions.UI.Disable();
        }

        private void Start()
        {
            if (StagePlayingState == StagePlayingState.BeforeStart)
            {
                StartStage();
            }

            InitializeInput();
        }

        private void StagePause(InputAction.CallbackContext context)
        {
            if (StagePlayingState == StagePlayingState.Playing)
            {
                PauseStage();
            }
            else if (StagePlayingState == StagePlayingState.Paused)
            {
                ResumeStage();
            }
        }

        private void InitializeInput()
        {
            HandleInputState(StagePlayingState);
        }

        public void StartStage()
        {
            bgmManager.PlayBGM();
            ChangeStageState(StagePlayingState.Playing);
        }

        public void PauseStage()
        {
            bgmManager.ApplyLowPassFilter(true);
            ChangeStageState(StagePlayingState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeStage()
        {
            bgmManager.ApplyLowPassFilter(true);
            ChangeStageState(StagePlayingState.Playing);
            Time.timeScale = 1f;
        }

        public void FailStage()
        {
            bgmManager.PlayFailSound();
            ChangeStageState(StagePlayingState.PlayerFailed);
        }

        public void ClearStage()
        {
            bgmManager.PlaySuccessSound();
            ChangeStageState(StagePlayingState.Cleared);
        }

        protected void ChangeStageState(StagePlayingState newState)
        {
            StagePlayingState = newState;
            OnStageStateChanged?.Invoke(newState);
            HandleInputState(newState);
        }

        private void HandleInputState(StagePlayingState newState)
        {
            switch (newState)
            {
                case StagePlayingState.Playing:
                    EnableUIInput(false);
                    break;
                case StagePlayingState.Paused:
                    EnableUIInput(true);
                    break;
                case StagePlayingState.PlayerFailed:
                case StagePlayingState.Cleared:
                    EnableUIInput(true);
                    break;
                default:
                    EnableUIInput(false);
                    break;
            }
        }

        protected void EnableUIInput(bool enable)
        {
            if (enable)
            {
                // _inputActions.UI.Enable();
            }
            else
            {
                // _inputActions.UI.Disable();
            }
        }

        public abstract void InitializeStage();
    }
}
