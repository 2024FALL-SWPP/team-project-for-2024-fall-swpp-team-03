using SWPPT3.Main.Manager;
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
                GameManager.Instance.OnPlayerStateChanged("GameOver");
            }
            else if (obstacle is Gas gas)
            {
                player.TryChangeState(PlayerStates.Slime);
            }
            else if (obstacle is MagicCircle magicCircle)
            {
                GameManager.Instance.OnPlayerStateChanged("StageCleared");
            }
            else
            {
                obstacle.InteractWithPlayer(player.CurrentState);
            }
        }

        public virtual void StopInteractWithProp(Player player, PropBase obstacle)
        {
            obstacle.StopInteractWithPlayer(player.CurrentState);
        }

        public abstract void ChangeRigidbody(Rigidbody rb);
        public abstract void ChangePhysics(Collider collider, PhysicMaterial physicMaterial);
    }

}
