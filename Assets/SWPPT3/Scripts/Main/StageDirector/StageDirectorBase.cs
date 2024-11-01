using SWPPT3.Main.Utility.Singleton;
using UnityEditor.SceneManagement;

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
        public StagePlayingState StagePlayingState { get; protected set; }
    }
}
