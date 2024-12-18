namespace SWPPT3.Main.StageDirector
{
    public class Tutorial1Director : StageManager
    {
        public override void InitializeStage()
        {
            // StartStage();
            player.SetItemCounts(0,0,0);
            //Debug.Log("Tutorial1Director Initialized");
        }
    }
}
