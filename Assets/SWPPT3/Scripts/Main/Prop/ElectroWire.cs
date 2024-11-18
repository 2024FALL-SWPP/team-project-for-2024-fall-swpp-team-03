using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ElectricWire : StatefulProp
    {
        public GameObject PositiveElectrode;
        public GameObject NegativeElectrode;

        private Dictionary<GameObject, List<GameObject>> connections = new Dictionary<GameObject, List<GameObject>>();

        private void OnCollisionEnter(Collision collision)
        {
            var otherObject = collision.gameObject;

            // MetalStateProp 객체인지 확인
            if (otherObject.TryGetComponent(out MetalStateProp metalStateProp))
            {
                // Dictionary에 MetalStateProp 추가
                if (!connections.ContainsKey(otherObject))
                {
                    connections[otherObject] = new List<GameObject>();
                }

                // 충돌한 객체의 연결을 추가
                foreach (var contact in collision.contacts)
                {
                    var connectedObject = contact.otherCollider.gameObject;
                    if (connectedObject.TryGetComponent(out MetalStateProp _))
                    {
                        if (!connections[otherObject].Contains(connectedObject))
                        {
                            connections[otherObject].Add(connectedObject);
                        }
                    }
                }

                if (IsPathFromPositiveToNegative())
                {
                    Debug.Log("Path found between Positive and Negative electrodes!");
                }
            }
        }

        private bool IsPathFromPositiveToNegative()
        {
            HashSet<GameObject> visited = new HashSet<GameObject>();
            return CheckPath(PositiveElectrode, NegativeElectrode, visited);
        }

        private bool CheckPath(GameObject current, GameObject target, HashSet<GameObject> visited)
        {
            if (visited.Contains(current)) return false;

            visited.Add(current);

            if (current == target) return true;

            if (connections.TryGetValue(current, out var connectedObjects))
            {
                foreach (var connectedObject in connectedObjects)
                {
                    if (CheckPath(connectedObject, target, visited))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
