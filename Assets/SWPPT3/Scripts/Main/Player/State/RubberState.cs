using SWPPT3.Main.Prop;

namespace SWPPT3.Main.Player.State
{
    public class RubberState : PlayerState
    {
        public override void InteractWithProp(PropBase obstacle)
        {
            // player의 변화
            obstacle.InteractWithPlayer();
        }
    }
}
