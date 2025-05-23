using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSim_HexPathfind : MonoBehaviour
{
    public Vector3Int goal;
    public Tilemap tilemap;
    private bool goalReached = false;
    // DUMBASS BOGO PATHFINDING!!!!
    // x is the amount of paths we generate
    // y is the cost of a path
    // we generate x paths and pick the one with lowest y (cost)
    // a path is a list of hexes in order of traverse
    // we take one step at a time, going through the hexes in the list in order.

    //path generation:
    /*
    1. From the current tile, get all valid neighbor tiles (non-empty, not blocked)
    2. Calculate the hex distance from each neighbor to the goal.
    3. Pick the 3 (or N) tiles that are closest to the goal.
    4. Randomly choose one of those 3 to move to.
    5. Repeat until:
        You reach the goal
        You run into a dead end
        Or exceed some safety max step count
    6. Do this process multiple times (e.g., 100 tries).
    7. Keep the path that reaches the goal and has the lowest cost or shortest length.
     */

    //returns a random-generated path to goal - path is a (dictionary?) that has a list of chosen hexes and total cost of path
    public List<Vector3Int> GetRandomPath()
    {
        goalReached = false;
        List<Vector3Int> path = new List<Vector3Int>();

        //convert 2D tile pos to 3D
        Vector2Int tilePos2D = GetComponent<BSim_HexMove>().currentHexPosition;
        Vector3Int tilePos3D = new Vector3Int(tilePos2D.x, tilePos2D.y, 0);

        //find my neighbors
        List<Vector3Int> neighbors = GetValidNeighbors(tilePos3D);

        //narrow down to best 3 neighbors
        List<Vector3Int> best3Neighbors = GetBestNeighbours(neighbors);

        // randomly choose one of 3 best options - closest tile gets 60% chance, 2nd 30%, 3rd 10%
        // IF one of these is the goal, it will return it
        Vector3Int nextTile = GetWeightedRandomTile(best3Neighbors);

        path.Add(nextTile);

        if (goalReached)
            return path;
    }

    // Call this to get all valid neighbors of a flat-top hex tile
    public List<Vector3Int> GetValidNeighbors(Vector3Int tilePos)
    {
        // This list will hold all neighbor positions that are not empty
        List<Vector3Int> validNeighbors = new List<Vector3Int>();

        // These are the 6 directions for flat-top hex grid (x, y) offsets
        // Even-q layout (even y rows are shifted right)
        Vector3Int[] directionsEven = new Vector3Int[]
        {
            new Vector3Int(+1,  0, 0), // Right
            new Vector3Int( 0, +1, 0), // Top-right
            new Vector3Int(-1, +1, 0), // Top-left
            new Vector3Int(-1,  0, 0), // Left
            new Vector3Int(-1, -1, 0), // Bottom-left
            new Vector3Int( 0, -1, 0)  // Bottom-right
        };

        // Odd-q layout (odd y rows are shifted left)
        Vector3Int[] directionsOdd = new Vector3Int[]
        {
            new Vector3Int(+1,  0, 0), // Right
            new Vector3Int(+1, +1, 0), // Top-right
            new Vector3Int( 0, +1, 0), // Top-left
            new Vector3Int(-1,  0, 0), // Left
            new Vector3Int( 0, -1, 0), // Bottom-left
            new Vector3Int(+1, -1, 0)  // Bottom-right
        };

        // Figure out if this row is even or odd
        bool isEvenRow = (tilePos.y % 2 == 0);

        // Pick the correct direction set
        Vector3Int[] directions;
        if (isEvenRow)
        {
            directions = directionsEven;
        }
        else
        {
            directions = directionsOdd;
        }

        // Go through all 6 possible neighbors
        for (int i = 0; i < directions.Length; i++)
        {
            // Calculate neighbor position
            Vector3Int neighborPos = tilePos + directions[i];

            // Check if that neighbor exists (not null)
            TileBase tileAtNeighbor = tilemap.GetTile(neighborPos);

            if (neighborPos == goal)
            {
                // stop doing this, other functions will see this is goal and choose it
                return null;
            }
            if (tileAtNeighbor != null)
            {
                // It's a valid neighbor, add to list
                validNeighbors.Add(neighborPos);
            }
        }

        // Return the list of valid neighbors
        return validNeighbors;
    }

    public List<Vector3Int> GetBestNeighbours(List<Vector3Int> neighborList)
    {
        //calculate hex dist to goal for each neighbor
            //hex dist calculation (might want another separate function for this)
        //pick 3 tiles with shortest dist to goal:
            // Sorts the list from smallest to biggest
            //numbers.Sort();

            // Get the first 3 (smallest) numbers
            //List<int> threeLowest = numbers.GetRange(0, 3);

            // Now 'threeLowest' contains the lowest three numbers
            // Sort them again to ensure correct order for random picking?
        //return threeLowest
    }

    public Vector3Int GetWeightedRandomTile(List<Vector3Int> tilesToChooseFrom)
    {
        // roll a number between 0 and 99
        int roll = UnityEngine.Random.Range(0, 100); // 0 to 99

        Vector3Int selectedTile;

        if (roll < 60)
        {
            // 60% chance
            selectedTile = tilesToChooseFrom[0];
            if (selectedTile == goal)
            {
                goalReached = true;
                return selectedTile; 
            }
        }
        else if (roll < 90)
        {
            // 30% chance
            selectedTile = tilesToChooseFrom[1];
            if (selectedTile == goal) 
            {
                goalReached = true;
                return selectedTile;                 
            }
        }
        else
        {
            // 10% chance
            selectedTile = tilesToChooseFrom[2];
            if (selectedTile == goal) 
            {
                goalReached = true;
                return selectedTile; 
            }
        }
        return selectedTile;
    }
}
