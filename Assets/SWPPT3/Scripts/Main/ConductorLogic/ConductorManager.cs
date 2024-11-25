using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.ConductorLogic
{
    public class ConductorManager : MonoBehaviour
    {
        [SerializeField]
        private List<Conductor> _conductors;

        [SerializeField]
        private List<Emitter> _emitters;

        [SerializeField]
        private List<Receptor> _receptors;

        public static bool IsDirty = false;

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

            var queue = new Queue<Conductor>();
            var visited = new HashSet<Conductor>();

            foreach (var plusElectrode in _emitters)
            {
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
                        queue.Enqueue(conductor);
                        visited.Add(conductor);
                    }
                }
            }

            foreach (var minusElectrode in _receptors)
            {
                if (visited.Contains(minusElectrode))
                {
                    minusElectrode.UpdateState(true);
                }
                else
                {
                    minusElectrode.UpdateState(false);
                }
            }
        }
    }
}
