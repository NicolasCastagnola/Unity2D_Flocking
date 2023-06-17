using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Flock/Filter/Physics Layer")]
public class PhysicsLayerFilter : ContextFilter
{
    public LayerMask mask;

    //---------------IA2-P1------------------

    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        /*List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            if (mask == (mask | (1 << item.gameObject.layer)) || item.gameObject.layer == 8)
            {
                filtered.Add(item);
            }
        }*/

        List<Transform> filtered = original
            .Where(item => 
            (mask == (mask | (1 << item.gameObject.layer))) || item.gameObject.layer == 8).
            ToList();

        return filtered;
    }
}
