using UnityEngine;

namespace SWPPT3.Main
{
    public class YouSpinMeRound : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation *= Quaternion.Euler(0, 100 * Time.deltaTime, 0);
        }
    }
}

