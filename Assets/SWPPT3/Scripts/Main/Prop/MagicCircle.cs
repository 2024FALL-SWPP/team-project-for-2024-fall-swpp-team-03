using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StatefulProp
    {
        [SerializeField]
        private int stateChangeAmount;
        public override void StateChangeEvent()
        {
            if (this.State == Full)
            {

            }
            else if (this.State == Off)
            {

            }
        }

        public override void Activate()
        {
            this.State += stateChangeAmount;
        }

        public override void Deactivate()
        {
            this.State -= stateChangeAmount;
        }
    }
}
