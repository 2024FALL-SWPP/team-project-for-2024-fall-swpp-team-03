using UnityEngine;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;

namespace SWPPT3.Main.StageDirector
{
    public class Stage1Director : StageManager
    {
        public override void InitializeStage()
        {
            StartStage();
            player.SetItemCounts(0,0,0);
        }

    }
}
