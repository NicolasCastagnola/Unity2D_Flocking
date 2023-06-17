using System.Collections;
using System.Collections.Generic;
using Flocking;
using UnityEngine;

public abstract class ContextFilter : ScriptableObject
{
    public abstract List<Transform> Filter(FlockAgent agent, List<Transform> original);
}
