namespace SWPPT3.Main.StageDirector
{
    public class Tutorial2Director : StageManager
    {
        public override void InitializeStage()
        {
            // StartStage();
            player.SetItemCounts(0,0,1);
            //Debug.Log("Tutorial2Director Initialized");
        }
    }
}
