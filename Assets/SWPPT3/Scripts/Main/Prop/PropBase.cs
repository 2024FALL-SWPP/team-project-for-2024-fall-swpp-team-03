using UnityEngine;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.Prop
{
    public abstract class PropBase : MonoBehaviour
    {
        public virtual void InteractWithPlayer()
            // Player에 의해 StateProb이 어떻게 변하는지
        {

        }
        public virtual void InteractWithPlayer(States state)
        // Player에 의해 StateProb이 어떻게 변하는지
        {

        }
    }
}
