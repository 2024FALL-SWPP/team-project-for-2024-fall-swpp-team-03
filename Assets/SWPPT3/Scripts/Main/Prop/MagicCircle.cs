using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField]
        private int stateChangeAmount;
        protected override void OnSourceStateChanged(bool state)
        {
            if (this.State == On)
            {

            }
            else if (this.State == Off)
            {

            }
        }
    }
}
