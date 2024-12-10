using UnityEngine;
using SWPPT3.Main.Manager;

namespace SWPPT3.Main.StageDirector
{
    public class Stage2Director : StageManager
    {
        public override void InitializeStage()
        {
            StartStage();
            player.SetItemCounts(0,0,1);

        }
    }
}
