using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.Utility.Singleton;
using UnityEngine;

namespace SWPPT3.Main.StageDirector
{
    public abstract class StageManager : MonoWeakSingleton<StageManager>
    {
        [SerializeField] protected Player player;

        public Player Player{ get => player;}
        protected int Time;

        public virtual void InitializeStage()
        {
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
            //Debug.Log("FailStage");
        }

        public void ClearStage()
        {
            UnityEngine.Time.timeScale = 0f;
            //Debug.Log("Cleared Stage");
        }

        public void UpdateTime()
        {
            Time++;
            UIManager.Instance.PlayTimeUpdate(Time);
        }

    }
}
