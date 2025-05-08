using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexDirections
{
    // These are the axial directions for flat-top hexes.
    public static readonly Vector2Int[] axialDirections = new Vector2Int[]
    {
        new Vector2Int(0, -1),  // Up
        new Vector2Int(0, 1),   // Down
        new Vector2Int(-1, 0),  // Top-left
        new Vector2Int(1, -1),  // Top-right
        new Vector2Int(-1, 1),  // Bottom-left
        new Vector2Int(1, 0)    // Bottom-right
    };
}

public class BSim_HexMove : MonoBehaviour
{
    public Vector2Int currentHexPosition; // Axial coords
    public Grid grid;

    public Tilemap tilemap;

    public void MoveInDirection(int directionIndex)
    {
        // Clamp directionIndex between 0-5
        directionIndex = Mathf.Clamp(directionIndex, 0, 5);

        Vector2Int direction = HexDirections.axialDirections[directionIndex];
        currentHexPosition += direction;

        Vector3 worldPos = HexToWorld(currentHexPosition, .5f);
        transform.position = worldPos;
    }

    public static Vector3 HexToWorld(Vector2Int axial, float hexSize)
    {
        float x = hexSize * (3f / 2f) * axial.x;
        float y = 0f;
        float z = hexSize * Mathf.Sqrt(3f) * (axial.y + axial.x / 2f);
        return new Vector3(x, y, z);
    }

    public void ScanTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        TileBase tile = tilemap.GetTile(cellPosition);

        Debug.Log($"Tile under me: {tile}");
    }

    //WIP Switch-Case to convert tiles:
    /*
    switch (owner)
    {
        case Owner.North:
            tilemap.SetTile(position, northTile);
            break;
        case Owner.South:
            tilemap.SetTile(position, southTile);
            break;
        case Owner.Neutral:
        default:
            tilemap.SetTile(position, neutralTile);
            break;
    }
     */
}

