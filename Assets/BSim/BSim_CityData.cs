using UnityEngine;

[CreateAssetMenu(fileName = "NewCity", menuName = "BSim/City")]
public class BSim_CityData : ScriptableObject
{
    public string cityName; // Optional, for debugging or display
    public Vector3Int position;
    public Owner owner;
    public float baseStrength;
}
