using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.ConductorLogic
{
    public class ConductorManager : MonoBehaviour
    {
        [SerializeField]
        private List<Conductor> _conductors;

        [SerializeField]
        private List<PlusElectrode> _plusElectrodes;

        [SerializeField]
        private List<MinusElectrode> _minusElectrodes;

        public bool IsDirty = false;

        private void Update()
        {
            if (IsDirty)
            {
                CheckConnection();
            }
        }

        private void CheckConnection()
        {
            IsDirty = false;

            // Reset all conductors' CurrentFlow to false
            foreach (var conductor in _conductors)
            {
                conductor.CurrentFlow = false;
            }

            // BFS starting from plus electrodes
            var queue = new Queue<Conductor>();
            var visited = new HashSet<Conductor>();

            foreach (var plusElectrode in _plusElectrodes)
            {
                plusElectrode.CurrentFlow = true;
                queue.Enqueue(plusElectrode);
                visited.Add(plusElectrode);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var connectionObj in current.Connections)
                {
                    var conductor = connectionObj.GetComponent<Conductor>();

                    if (conductor != null && !visited.Contains(conductor) && conductor.IsConductive())
                    {
                        conductor.CurrentFlow = true;
                        queue.Enqueue(conductor);
                        visited.Add(conductor);
                    }
                }
            }

            foreach (var minusElectrode in _minusElectrodes)
            {
                minusElectrode.UpdatePower(minusElectrode.CurrentFlow);
            }
        }
    }
}
