using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public static class Extensions
{
    /// <summary>
    /// Prints a collection to te console.
    /// </summary>
    /// <param name="IEnumerable">Collection to print.</param>
    public static IEnumerable<T> PrintCollection<T>(this IEnumerable<T> showList)
    {

        var text = new StringBuilder("[");
        var list = showList.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            text.Append(list[i]);

            if (i != list.Count - 1)
            {
                text.Append(",");
            }
        }

        text.Append("]");
        Debug.Log($"{text}");
        return showList;
     }
}
