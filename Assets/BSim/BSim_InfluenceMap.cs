using UnityEngine;
using UnityEngine.Tilemaps;

public class BSim_InfluenceMap : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile neutralTile;
    public Tile redTile;
    public Tile blueTile;

    public enum Owner { Neutral, North, South }

    public void SetTileOwner(Vector3Int cellPosition, Owner owner)
    {
        switch (owner)
        {
            case Owner.Neutral:
                tilemap.SetTile(cellPosition, neutralTile);
                break;
            case Owner.North:
                tilemap.SetTile(cellPosition, blueTile);
                break;
            case Owner.South:
                tilemap.SetTile(cellPosition, redTile);
                break;
        }
    }

    public Owner GetTileOwner(Vector3Int cellPosition)
    {
        Tile tile = tilemap.GetTile<Tile>(cellPosition);
        if (tile == blueTile) return Owner.South;
        if (tile == redTile) return Owner.North;
        return Owner.Neutral;
    }
}
