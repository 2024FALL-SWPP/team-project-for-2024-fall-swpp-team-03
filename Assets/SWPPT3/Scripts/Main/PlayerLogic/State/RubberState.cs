#region

using SWPPT3.Main.Prop;

#endregion

namespace SWPPT3.Main.PlayerLogic.State
{
    public class RubberState : PlayerState
    {
        public override void InteractWithProp(Player player, PropBase obstacle)
        {
            if (false)
            {
                // SomeSpecificProp에 대한 특별한 상호작용 처리
                // 여기에서 player 또는 obstacle에 대한 특정 작업 수행
            }
            else
            {
                base.InteractWithProp(player, obstacle);
            }
        }
    }
}
