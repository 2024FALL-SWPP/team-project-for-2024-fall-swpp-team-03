using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.Utility.Singleton;
using UnityEngine;

namespace SWPPT3.Main.StageDirector
{
    public class StageManager : MonoWeakSingleton<StageManager>
    {
        [SerializeField] protected Player player;

        public virtual void InitializeStage() { Debug.Log("not stageNdirector");}

        public void StartStage()
        {
            Time.timeScale = 1f;
            Debug.Log("Starting Stage");
        }

        public void PauseStage()
        {
            Time.timeScale = 0f;
        }

        public void ResumeStage()
        {
            Time.timeScale = 1f;
        }

        public void FailStage()
        {
            Time.timeScale = 0f;
            Debug.Log("FailStage");
        }

        public void ClearStage()
        {
            Time.timeScale = 0f;
            Debug.Log("Cleared Stage");
        }
    }
}
