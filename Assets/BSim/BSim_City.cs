using UnityEngine;
using UnityEngine.Tilemaps;

public class BSim_City : MonoBehaviour
{
    public Vector3Int position;
    public Owner owner;
    public float strength;

    private Tilemap tilemap;

    public TileBase northCityTile;
    public TileBase southCityTile;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();  // Finds the Tilemap in the scene
        SetTileVisuals();
    }

    public void SetOwner(Owner newOwner, float newStrength)
    {
        owner = newOwner;
        strength = newStrength;
        SetTileVisuals();
    }

    private void SetTileVisuals()
    {
        TileBase tileToSet = owner == Owner.North ? northCityTile : southCityTile;
        tilemap.SetTile(position, tileToSet);  // Update visual tile on map
    }

    public void SimulateTurn()
    {
        // Add city-specific logic here (e.g., defense, resource generation)
    }
}
