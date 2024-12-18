using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.UI;
using SWPPT3.Main.Utility.Singleton;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace SWPPT3.Main.StageDirector
{
    public abstract class StageManager : MonoWeakSingleton<StageManager>
    {
        [SerializeField] protected Player player;
        [SerializeField] protected InGameScreenBehaviour inGameScreen;

        public Player Player{ get => player;}
        public int Time;

        public abstract void InitializeStage();

        public void Awake()
        {
            InitializeStage();
            StartStage();
            GameManager.Instance._stageManager = this;
            GameManager.Instance.GameState = GameState.Playing;
        }

        public void StartStage()
        {
            UnityEngine.Time.timeScale = 1f;
            Time = 0;
            InvokeRepeating("UpdateTime", 1.0f, 1.0f);
            //Debug.Log("Starting Stage");
        }

        public void PauseStage()
        {
            UnityEngine.Time.timeScale = 0f;
        }

        public void ResumeStage()
        {
            UnityEngine.Time.timeScale = 1f;
        }

        public void FailStage()
        {
            UnityEngine.Time.timeScale = 0f;
            inGameScreen.ShowFail();
            //Debug.Log("FailStage");
        }

        public void ClearStage()
        {
            UnityEngine.Time.timeScale = 0f;
            inGameScreen.ShowSuccess();
            //Debug.Log("Cleared Stage");
        }

        public void UpdateTime()
        {
            Time++;
            inGameScreen.PlayTimeUpdate(Time);
        }

    }
}
