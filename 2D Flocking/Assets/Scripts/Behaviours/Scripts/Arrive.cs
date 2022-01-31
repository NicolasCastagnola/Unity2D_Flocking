using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Arrive")]
public class Arrive : FilteredFlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
            return Vector2.zero;

        Vector2 arrivalMove = Vector2.zero;

        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            Food food = item.gameObject.GetComponent<Food>();

            if (food != null)
            {
                if (Vector2.SqrMagnitude(food.transform.position - agent.transform.position) <= flock.SquareSeekRadius * 10)
                {
                    //arrivalMove = arrivalMove.Normalize()  - agent.transform.position;
                    arrivalMove.Normalize();

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

        return default;
    }
    /*
        desired_velocity = target - position
    distance = length(desired_velocity)
 
    // Check the distance to detect whether the character
    // is inside the slowing area
    if (distance<slowingRadius) {
        // Inside the slowing area
        desired_velocity = normalize(desired_velocity) * max_velocity* (distance / slowingRadius)
    } else
    {
        // Outside the slowing area.
        desired_velocity = normalize(desired_velocity) * max_velocity
    }
    */
}
