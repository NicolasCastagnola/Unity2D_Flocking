using UnityEngine;

namespace Imported.FSM_CLASE
{
    public static class Utility
    {

        public static T Logging<T>(T param, string message = "")
        {
            Debug.Log(message + param.ToString());
            return param;
        }
    }
}