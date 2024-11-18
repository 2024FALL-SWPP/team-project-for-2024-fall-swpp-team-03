using System;
using System.Collections.Generic;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ElectroWire : StateSource
    {
        // +극, -극을 합쳐서 상위 object를 만든다.
        // 각각 전기가 통하는 물체들은 접촉하고 있는 전도체와 player를 관리한다.
        // +극에서 bfs를 해서 -극까지 연결되면 된다.
        // CheckConnection은 전도체들에서 ElectroWire를 attribute로 가지고
        // connections list들이 바뀔때마다 호출해서 Update에 비해 overhead를 줄임.
        public GameObject PositiveElectrode;
        public GameObject NegativeElectrode;

        private List<GameObject> _connectionsPos = new List<GameObject>();
        private List<GameObject> _connectionsNeg = new List<GameObject>();

        public void OnElectrodeCollisionEnter(GameObject electrode, GameObject otherObject)
        {
            if (electrode == PositiveElectrode)
            {
                _connectionsPos.Add(otherObject);
            }
            else if (electrode == NegativeElectrode)
            {
                _connectionsNeg.Add(otherObject);
            }
            CheckConnection();
        }

        public void OnElectrodeCollisionExit(GameObject electrode, GameObject otherObject)
        {
            if (electrode == PositiveElectrode)
            {
                _connectionsPos.Remove(otherObject);
            }
            else if (electrode == NegativeElectrode)
            {
                _connectionsNeg.Remove(otherObject);
            }
            CheckConnection();
        }

        public bool CheckConnection()
        {
            HashSet<GameObject> visited = new HashSet<GameObject>();
            Queue<GameObject> queue = new Queue<GameObject>();

            foreach (GameObject obj in _connectionsPos)
            {
                if (obj != null)
                {
                    queue.Enqueue(obj);
                    visited.Add(obj);
                }
            }

            while (queue.Count > 0)
            {
                GameObject current = queue.Dequeue();

                if (_connectionsNeg.Contains(current))
                {
                    State = On;
                    return true;
                }

                if (visited.Contains(current))
                {
                    continue;
                }
                visited.Add(current);

                Player player = current.GetComponent<Player>();
                if (player != null)
                {
                    if (player.CurrentState != PlayerStates.Metal)
                    {
                        continue;
                    }

                    foreach (GameObject neighbor in player.GetConnectedObjects())
                    {
                        if (neighbor != null && !visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                    continue;
                }

                ElectroBox prop = current.GetComponent<ElectroBox>();
                if (prop != null)
                {
                    foreach (GameObject neighbor in prop.GetConnectedObjects())
                    {
                        if (neighbor != null && !visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            State = Off;
            return false;
        }

    }
}
