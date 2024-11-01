using Unity;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SWPPT3.Main.Player;
namespace SWPPT3.Main.Obstacle
{
    public class ItemBox : ObstacleBase
    {
        private States _state;

        public ItemBox(States state)
        {
            _state = state;
        }

        public override void InteractWithPlayer()
        {
            Destroy(gameObject);
        }
    }
}
