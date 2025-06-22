using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSim_HexPathfind : MonoBehaviour
{
    public Vector3Int goal;
    public Tilemap tilemap;
    public int maxPathLength = 50;
    public bool goalReached = true; //for checking when a candidate path reaches the goal
    //public bool goalEntered = true; //for checking if we are ON a goal
    public TileBase signTile;
    public float moveSpeed = 5f;

    public List<Vector3Int> myPath = new List<Vector3Int>();
    private int currentStepOnPath = 0;
    int nextStep = 0;

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

    //helper function for converting hex coordinates
    public static Vector2Int OffsetToAxial(Vector3Int offset)
    {
        int x = offset.x;
        int y = offset.y - (offset.x / 2);  // even-q offset for flat-top hex
        return new Vector2Int(x, y);
    }
    public void CalculateNewPath()
    {
        myPath = GetBestPath(1000); //run GetBestPath with 1000 paths
    }
    public void TakeNextStepOnPath()
    {
        if (myPath == null || myPath.Count == 0)
        {
            Debug.LogWarning("No path calculated.");
            return;
        }
        if(nextStep >= myPath.Count)
        {
            Debug.Log("Reached end of path.");
            //goalEntered = true;
            myPath.Clear();
            return;
        }
        Vector3Int nextStepHex = myPath[nextStep];
        print("next step: " + nextStepHex);
        // Move visually
        Vector3 worldPosition = tilemap.GetCellCenterWorld(nextStepHex);
        transform.position = worldPosition;

        // Update logical position (convert offset to axial!)
        //Vector2Int axial = BSim_HexMove.OffsetToAxial(offset);
        GetComponent<BSim_HexMove>().currentHexPosition = nextStepHex;

        Vector3Int reconvert = GetComponent<BSim_HexMove>().currentHexPosition;
        if (reconvert != myPath[nextStep])
        {
            Debug.LogError($"DESYNC! Path says {myPath[nextStep]}, but position says {reconvert}");
        }
        currentStepOnPath = nextStep;
        nextStep++;
    }

    public void PrintBestPath()
    {
        myPath = GetBestPath(1000); //run GetBestPath with 1000 paths        

        Debug.Log("Best path steps:");
        for (int i = 0; i < myPath.Count; i++)
        {
            Vector3Int step = myPath[i];
            Debug.Log("Step " + i + ": (" + step.x + ", " + step.y + ", " + step.z + ")");
        }
        StartCoroutine(MoveAlongPath(myPath));
    }

    public IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3Int offset = path[i];

            // Move visually
            Vector3 worldPosition = tilemap.GetCellCenterWorld(offset);
            transform.position = worldPosition;

            // Update logical position (convert offset to axial!)
            //Vector2Int axial = BSim_HexMove.OffsetToAxial(offset);
            GetComponent<BSim_HexMove>().currentHexPosition = offset;

            Vector3Int reconvert = GetComponent<BSim_HexMove>().currentHexPosition;
            if (reconvert != path[i])
            {
                Debug.LogError($"DESYNC! Path says {path[i]}, but position says {reconvert}");
            }

            yield return new WaitForSeconds(moveSpeed);
        }
    }
    
    //Generate (pathsAmt) paths, give them a score & choose the cheapest one
    public List<Vector3Int> GetBestPath(int pathsAmt)
    {
        //goalEntered = false;

        List<List<Vector3Int>> generatedPaths = new List<List<Vector3Int>>();
        //make new modifier for containing the best path
        List<Vector3Int> bestPath = null;
        //set an astronomical sum to this int so we always find a shorter path
        int lowestCost = int.MaxValue;

        //generate (pathsAmt) paths
        for (int i = 0; i < pathsAmt; i++)
        {
            try
            {
                //use getRandomPath to get a new rand path
                List<Vector3Int> newPath = GetRandomPath();

                if (newPath == null || newPath.Count == 0)
                    continue;

                //add newly generated rand path to list of all new paths
                generatedPaths.Add(newPath);

                int pathCost = newPath.Count;
                if (pathCost < lowestCost && newPath[newPath.Count - 1] == goal)
                {
                    lowestCost = pathCost;
                    bestPath = newPath;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Path generation failed: " + ex.Message);
                // Keep calm and skip this iteration.
            }
        }
        if (bestPath == null)
        {
            Debug.LogError("No valid path found after " + pathsAmt + " attempts.");
            return new List<Vector3Int>(); // return empty path so caller doesn't crash
        }

        Debug.Log(gameObject.name + ": Best path length: " + bestPath.Count);
        return bestPath;
    }
    //returns a random-generated path to goal
    public List<Vector3Int> GetRandomPath()
    {
        goalReached = false;
        List<Vector3Int> path = new List<Vector3Int>();        

        //count steps to prevent paths longer than max length
        int steps = 0;

        //convert current 2D tile pos to 3D
        //Vector2Int axial = GetComponent<BSim_HexMove>().currentHexPosition;
        //Vector3Int tilePos3D = BSim_HexMove.AxialToOffset(axial);
        Vector3Int tilePos3D = GetComponent<BSim_HexMove>().currentHexPosition;

        //add start tile to path
        path.Add(tilePos3D);

        while (!goalReached && steps < maxPathLength)
        {
            //find my neighbors
            List<Vector3Int> neighbors = GetValidNeighbors(tilePos3D);

            if (neighbors.Count == 0)
            {
                Debug.LogWarning("No neighbors found, breaking out of loop.");
                break; // avoid infinite loop if stuck
            }

            //narrow down to best 3 neighbors
            List<Vector3Int> best3Neighbors = GetBestNeighbours(neighbors);

            // randomly choose one of 3 best options - closest tile gets 60% chance, 2nd 30%, 3rd 10%
            // IF one of these is the goal, it will return it
            Vector3Int nextTile = GetWeightedRandomTile(best3Neighbors);

            path.Add(nextTile);

            tilePos3D = nextTile;

            steps++;
        }
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
                // found goal!
                goalReached = true;
                validNeighbors.Add(neighborPos);
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

    // Call this to narrow a list of neighbour hexes down to 3 best (closest)
    public List<Vector3Int> GetBestNeighbours(List<Vector3Int> neighborList)
    {
        //calculate hex dist to goal for each neighbor, then sort neighborList so that closest hex is at index 0, next closest 1, etc.
        neighborList.Sort(CompareNeighbors);

        //how many neighbours to consider?
        int howMany = Mathf.Min(5, neighborList.Count); 
        //REMEMBER TO EDIT 244 AS WELL IF CHANGING THIS NUMBER!!!!

        //return the three closest hexes by trimming the list to the first n neighbors (howMany) (or fewer if there aren't howMany).
        return neighborList.GetRange(0, howMany);

    }

    // Call this to quasi-randomly pick one of the 3 best tiles
    public Vector3Int GetWeightedRandomTile(List<Vector3Int> tilesToChooseFrom)
    {        
        // roll a number between 0 and 99
        int roll = UnityEngine.Random.Range(0, 100); // 0 to 99

        Vector3Int selectedTile;

        // fallback: less tiles than required, pick randomly from what's available
        if (tilesToChooseFrom.Count < 5)
        {            
            int rInt = Random.Range(0, tilesToChooseFrom.Count);
            selectedTile = tilesToChooseFrom[rInt];
            //check if we found the goal
            if (selectedTile == goal)
            {
                goalReached = true;
            }
        }

        if (roll < 40)
        {
            // start% chance
            selectedTile = tilesToChooseFrom[0];
        }
        else if (roll < 70)
        {
            // less% chance
            selectedTile = tilesToChooseFrom[1];
        }
        else if (roll < 85)
        {
            // less% chance
            selectedTile = tilesToChooseFrom[2];
        }
        else if (roll < 93)
        {
            // less% chance
            selectedTile = tilesToChooseFrom[3];
        }
        else
        {
            // even less% chance
            selectedTile = tilesToChooseFrom[4];
        }
        //check if we found the goal
        if (selectedTile == goal) 
        {
            goalReached = true;
        }

        return selectedTile;
    }

    // Calculate hex distance between 2 hexes
    public int GetHexDist(Vector3Int a, Vector3Int b)
    {
        // Convert flat-top axial (q, r) to cube (x, y, z)
        int x1 = a.x;
        int z1 = a.y;
        int y1 = -x1 - z1;

        int x2 = b.x;
        int z2 = b.y;
        int y2 = -x2 - z2;

        int dx = Mathf.Abs(x1 - x2);
        int dy = Mathf.Abs(y1 - y2);
        int dz = Mathf.Abs(z1 - z2);

        return Mathf.Max(dx, dy, dz);
    }

    // Helper function for GetBestNeighbours, used in sorting neighborList according
    private int CompareNeighbors(Vector3Int n1, Vector3Int n2)
    {
        return GetHexDist(n1, goal).CompareTo(GetHexDist(n2, goal));
    }
}
