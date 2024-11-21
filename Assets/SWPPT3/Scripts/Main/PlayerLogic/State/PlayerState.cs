using SWPPT3.Main.Prop;
using SWPPT3.Main.PlayerLogic;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic.State
{
    public enum PlayerStates
    {
        Slime = 0,
        Metal,
        Rubber,
    }

    public abstract class PlayerState
    {
        public virtual void InteractWithProp(Player player, PropBase obstacle)
        {
            if (obstacle is ItemBox itemBox)
            {
                player.Item[itemBox.ItemState] += 1;
                Debug.Log(player.Item[itemBox.ItemState]);
                itemBox.InteractWithPlayer();
            }
            else if (obstacle is PoisonPool poisonPool)
            {
                player.GameOver();
            }
        }

        public abstract void ChangeRigidbody(Rigidbody rb);
        public abstract void ChangePhysics(Collider collider, PhysicMaterial physicMaterial);
    }

}
