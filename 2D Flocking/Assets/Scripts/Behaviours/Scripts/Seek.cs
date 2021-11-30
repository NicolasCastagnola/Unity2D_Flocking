using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/SeekFood")]

public class Seek : FilteredFlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
            return Vector2.zero;

        Vector2 seekMove = Vector2.zero;

        int n_Seek = 0;

        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            Food food = item.gameObject.GetComponent<Food>();
            
            if (food != null)
            {
                if (Vector2.SqrMagnitude(food.transform.position - agent.transform.position) <= flock.SquareSeekRadius  * 10)
                {
                    n_Seek++;
                    seekMove = food.transform.position - agent.transform.position;
                    seekMove.Normalize();

                    if (Vector2.Distance(food.transform.position, agent.transform.position) <= 0.2)
                    {
                        var d = food.GetComponent<IDestroyable>();

                        if (d != null)
                        {
                            d.Destroy();
                        }
                    }
                }
            }
        }

        return seekMove;
    }
}
