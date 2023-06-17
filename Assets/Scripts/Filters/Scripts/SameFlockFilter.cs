using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Flocking;
[CreateAssetMenu(menuName = "Flock/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{

    //---------------IA2-P1------------------

    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        /*List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock)
            {
                filtered.Add(item);
            }
        }*/

       List<Transform> filtered = original
            .Where(item =>
            {
                FlockAgent itemAgent = item.GetComponent<FlockAgent>();
                return itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock;
            })
            .ToList();


        return filtered;
    }
}

