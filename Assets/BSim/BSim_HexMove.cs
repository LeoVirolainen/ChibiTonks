using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexDirections
{
    // These are the axial directions for flat-top hexes.
    public static readonly Vector2Int[] axialDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 1),  // Top-left
        new Vector2Int(1, 0),  // Top-right
        new Vector2Int(-1, 0),  // Bottom-left
        new Vector2Int(1, -1)    // Bottom-right
    };
}

public class BSim_HexMove : MonoBehaviour
{
    public Vector3Int currentHexPosition; // Offset coords
    public Grid grid;

    public Tilemap tilemap;

    public TileBase northTile;
    public TileBase southTile;
    private void Update()
    {
        /*Vector2Int axial = OffsetToAxial(currentHexPosition);
        Vector3Int offset = currentHexPosition;
        Vector2Int roundTrip = OffsetToAxial(offset);

        Debug.Log($"Axial: {axial}, Offset: {offset}, RoundTrip: {roundTrip}");*/
    }
    public void MoveInDirection(int directionIndex)
    {
        // Clamp directionIndex between 0-5
        directionIndex = Mathf.Clamp(directionIndex, 0, 5);

        Vector2Int direction = HexDirections.axialDirections[directionIndex];
        Vector2Int currentAxial = OffsetToAxial(currentHexPosition);
        currentAxial += direction;

        Vector3 worldPos = HexToWorld(currentAxial, .5f);
        transform.position = worldPos;

        //update current pos
        currentHexPosition = AxialToOffset(currentAxial);
    }
    public static Vector3Int[] GetNeighborOffsets()
    {
        // These are axial directions for a hex grid (pointy-topped)
        Vector2Int[] axialDirections = new Vector2Int[]
        {
        new Vector2Int(1, 0),   // East
        new Vector2Int(1, -1),  // Northeast
        new Vector2Int(0, -1),  // Northwest
        new Vector2Int(-1, 0),  // West
        new Vector2Int(-1, 1),  // Southwest
        new Vector2Int(0, 1),   // Southeast
        };

        Vector3Int[] offsets = new Vector3Int[axialDirections.Length];

        for (int i = 0; i < axialDirections.Length; i++)
        {
            offsets[i] = AxialToOffset(axialDirections[i]);
        }

        return offsets;
    }
    public static Vector3 HexToWorld(Vector2Int axial, float hexSize)
    {
        float x = hexSize * (3f / 2f) * axial.x;
        float y = 0f;
        float z = hexSize * Mathf.Sqrt(3f) * (axial.y + axial.x / 2f);
        return new Vector3(x, y, z);
    }
    public static Vector2Int OffsetToAxial(Vector3Int offset)
    {
        int col = offset.x;
        int row = offset.y;
        int q = col;
        int r = row - (col / 2);
        return new Vector2Int(q, r);
    }
    public static Vector3Int AxialToOffset(Vector2Int axial)
    {
        int col = axial.x;
        int row = axial.y + (col / 2);
        return new Vector3Int(col, row, 0);
    }
    public void ScanTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        TileBase tile = tilemap.GetTile(cellPosition);

        Debug.Log($"Tile under me: {tile}");
    }

    public void ConvertTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        TileBase tile = tilemap.GetTile(cellPosition);
        if (tile == southTile)
            SwapTile(cellPosition, Owner.South);
        else
            SwapTile(cellPosition, Owner.North);
    }

    public void SwapTile(Vector3Int position, Owner owner)
    {
        switch (owner)
        {
            case Owner.North:
                tilemap.SetTile(position, southTile);
                break;
            case Owner.South:
                tilemap.SetTile(position, northTile);
                break;
        }
    }
}

