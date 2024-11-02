using System.Collections.Generic;
using Unity;
using UnityEngine;
using SWPPT3.Main.Prop;

namespace SWPPT3.Main.Player
{
    public class Player : MonoBehaviour
    {
        private States _currentState = States.Slime;
        private PlayerState PlayerState => _playerStates[_currentState];

        private readonly Dictionary<States, PlayerState> _playerStates = new()
        {
            { States.Metal, new MetalState() },
            { States.Rubber, new RubberState() },
            { States.Slime, new SlimeState() },
        };

        public void PlayerMove()
        {

        }

        public void ChangeState(States state)
        {
            _currentState = state;
        }

        public void InteractWithObject(PropBase prop)
        {
            PlayerState.InteractWithProp(prop);
        }

        void OnCollisionEnter(Collision collision)
        {
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            InteractWithObject(obstacle);
        }


    }
}
