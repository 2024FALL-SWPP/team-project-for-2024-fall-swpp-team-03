using SWPPT3.Main.Utility.Singleton;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

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
        // public List<PropBase> PropList; //PR 후 추가.
        // private MagicCircleSystem _magicCircle;

        public StagePlayingState StagePlayingState { get; protected set; } = StagePlayingState.BeforeStart;

        private void Start()
        {
            // _magicCircle = ; // SystemObject인 MagicCircle와 연결될 수 있게
            if (StagePlayingState == StagePlayingState.BeforeStart)
            {
                SpawnObjects();
            }
        }
        private void Update()
        {
            // 특정 조건일때 pause하도록 일단은 keydown으로 설정 나중에 수정
            if (StagePlayingState == StagePlayingState.Playing && Input.GetKeyDown(KeyCode.P))
            {
                PauseStage();
            }
            // 특정 조건일때 resume하도록 일단은 keydown으로 설정 나중에 수정
            else if (StagePlayingState == StagePlayingState.Paused && Input.GetKeyDown(KeyCode.R))
            {
                ResumeStage();
            }


            // if (StagePlayingState == StagePlayingState.Playing && Player.life && _magicCircle.isClear())
            // {
            //     ClearStage();
            // }
            //
            // if (StagePlayingState == StagePlayingState.Playing && !Player.life)
            // {
            //     FailStage();
            // }
        }


        public void SpawnObjects()
        {
            // prop과 player에 각 stage에 맞는 spawn positoin, rotation 추가.
            // foreach (ProbBase prop in PropList)
            // {
            //     Instantiate((GameObject)prop, prom.position, prop.rotation);
            // }
        }

        public void DestroyObjects()
        {
            // foreach ()
            // {
            //     Destory()
            // }
        }

        public void SpawnPlayer()
        {

        }

        public void StartStage()
        {
            StagePlayingState = StagePlayingState.Playing;
            SpawnPlayer();
        }

        public void PauseStage()
        {
            StagePlayingState = StagePlayingState.Paused;
            // key input이 player에게 가지 않도록 설정
        }

        public void ResumeStage()
        {
            StagePlayingState = StagePlayingState.Playing;
            // key input이 player에게 가도록 설정
        }

        public void FailStage()
        {
            StagePlayingState = StagePlayingState.PlayerFailed;
        }

        public void ClearStage()
        {
            StagePlayingState = StagePlayingState.Cleared;
            LoadNextStage();
            DestroyObjects();
        }

        public abstract void LoadNextStage();

    }
}
