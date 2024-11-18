using System.Collections.Generic;
using SWPPT3.Main.Prop;
using UnityEngine;

public class MetalStateProp : StatefulProp
{
    // MetalStateProp은 별도의 상태 없이 연결만 담당
    private Dictionary<GameObject, List<GameObject>> localConnections = new Dictionary<GameObject, List<GameObject>>();

    private void OnCollisionEnter(Collision collision)
    {
        var otherObject = collision.gameObject;

        if (otherObject.TryGetComponent(out MetalStateProp _))
        {
            if (!localConnections.ContainsKey(this.gameObject))
            {
                localConnections[this.gameObject] = new List<GameObject>();
            }

            localConnections[this.gameObject].Add(otherObject);
        }
    }
}
