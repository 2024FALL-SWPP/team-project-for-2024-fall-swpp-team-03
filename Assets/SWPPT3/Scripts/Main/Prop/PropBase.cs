using UnityEngine;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.Prop
{
    public abstract class PropBase : MonoBehaviour
    {
        public virtual void InteractWithPlayer()
        {

        }
        public virtual void InteractWithPlayer(PlayerStates state)
        {

        }
    }
}
