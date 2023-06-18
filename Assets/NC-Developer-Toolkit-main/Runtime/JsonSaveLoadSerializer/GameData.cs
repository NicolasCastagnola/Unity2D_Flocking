using JsonSaveLoadSerializer.Serializable;
using UnityEngine;

namespace JsonSaveLoadSerializer
{
    [System.Serializable]
    public class GameData
    {
        //Default Variables
        public int playerHealth = 1;
        public Vector3 currentPosition = Vector3.zero;
        public SerializableDictionary<Vector3, bool> checkPoints = new();
        //DECLARE HERE VARIABLES TO SAVE!
        public GameData()
        {
            Debug.Log("Initializing Game data");
        }
    }
}
