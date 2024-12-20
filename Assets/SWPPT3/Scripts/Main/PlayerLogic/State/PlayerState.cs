#region

using System;
using SWPPT3.Main.Manager;
using SWPPT3.Main.Prop;
using SWPPT3.Main.PlayerLogic;
using UnityEngine;
using SWPPT3.Main.UI;

#endregion

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
        private bool _isInGas;

        public virtual void InteractWithProp(Player player, PropBase obstacle)
        {
            if (obstacle is ItemBox itemBox)
            {
                if (!itemBox.MarkedToBeDestroyed)
                {
                    itemBox.InteractWithPlayer();

                    player.PlusItemCounts(itemBox.ItemState);
                    player.ItemSound();
                }
            }
            else if (obstacle is PoisonPool)
            {
                // Debug.Log("collide with Poison pool");
                GameManager.Instance.GameState = GameState.GameOver;
            }
            else if (obstacle is Gas)
            {
                if (_isInGas == false)
                {
                    foreach(PlayerStates playerState in System.Enum.GetValues(typeof(PlayerStates)))
                    {
                        player.SetItemCounts(0,0,0);
                    }
                    player.TryChangeState(PlayerStates.Slime);
                    // player.GasSound();
                    _isInGas = true;
                }
            }
            else if (obstacle is MagicCircle)
            {
                GameManager.Instance.GameState = GameState.StageCleared;
            }
            else
            {
                obstacle.InteractWithPlayer(player.CurrentState);
            }
        }

        public virtual void StopInteractWithProp(Player player, PropBase obstacle)
        {
            obstacle.StopInteractWithPlayer(player.CurrentState);
            if (obstacle is Gas)
            {
                _isInGas = false;
            }

        }
    }
}
