using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Owner { Neutral, North, South }

public class BSim_TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile neutralTile;
    public Tile northTile;
    public Tile southTile;
    public TileBase northCityTile;
    public TileBase southCityTile;
    public Dictionary<Vector3Int, (Owner owner, float strength)> cityPositions = new();

    [System.Serializable]
    public class TileData
    {
        public Vector3Int position;
        public Owner owner;
        public float strength; // Used if needed later
    }

    public Dictionary<Vector3Int, TileData> tileDict = new Dictionary<Vector3Int, TileData>();

    void Start()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            tileDict[pos] = new TileData
            {
                position = pos,
                owner = Owner.Neutral,
                strength = 0
            };
        }

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile == northCityTile)
            {
                cityPositions[pos] = (Owner.North, 50);
                if (tileDict.ContainsKey(pos))
                {
                    tileDict[pos].owner = Owner.North;
                    tileDict[pos].strength = 50f; // Or whatever you want as base city strength
                }
            }
            else if (tile == southCityTile)
            {
                cityPositions[pos] = (Owner.South, 5);
                if (tileDict.ContainsKey(pos))
                {
                    tileDict[pos].owner = Owner.South;
                    tileDict[pos].strength = 5f;
                }
            }
        }

        /*
        // Add city positions manually
        cityPositions.Add(new Vector3Int(4, 2, 0), Owner.North);
        cityPositions.Add(new Vector3Int(10, 3, 0), Owner.South);

        // Manually mark cities here, or in a separate setup method
        SetCityOwner(new Vector3Int(-4, 0, 0), Owner.North); //Birmingham
        SetCityOwner(new Vector3Int(-5, 0, 0), Owner.North);
        SetCityOwner(new Vector3Int(-5, -1, 0), Owner.North);
        
        SetCityOwner(new Vector3Int(5, -2, 0), Owner.North); //manchester
        SetCityOwner(new Vector3Int(8, 2, 0), Owner.North); //leeds
        SetCityOwner(new Vector3Int(4, -7, 0), Owner.North); //liverpool
        SetCityOwner(new Vector3Int(4, 2, 0), Owner.North); //sheffield
        SetCityOwner(new Vector3Int(-14, -9, 0), Owner.North); //cardiff
        SetCityOwner(new Vector3Int(-5, 2, 0), Owner.North); //coventry
        SetCityOwner(new Vector3Int(8, 0, 0), Owner.North); //bradford
        SetCityOwner(new Vector3Int(0, 4, 0), Owner.North); //nottingham
        SetCityOwner(new Vector3Int(18, 2, 0), Owner.North); //newcastle-upon-tyne
        
        SetCityOwner(new Vector3Int(-13, 10, 0), Owner.South); //London
        SetCityOwner(new Vector3Int(-14, 10, 0), Owner.South);
        SetCityOwner(new Vector3Int(-13, 11, 0), Owner.South);
        SetCityOwner(new Vector3Int(-14, 11, 0), Owner.South); //centre
        SetCityOwner(new Vector3Int(-15, 11, 0), Owner.South);
        SetCityOwner(new Vector3Int(-13, 12, 0), Owner.South);
        SetCityOwner(new Vector3Int(-14, 12, 0), Owner.South);

        SetCityOwner(new Vector3Int(-14, -5, 0), Owner.South); //bristol
        SetCityOwner(new Vector3Int(-4, 5, 0), Owner.South); //leicester
        SetCityOwner(new Vector3Int(-20, 11, 0), Owner.South); //brighton & hove
        SetCityOwner(new Vector3Int(-23, -16, 0), Owner.South); //plymouth
        SetCityOwner(new Vector3Int(-19, 3, 0), Owner.South); //southampton
        SetCityOwner(new Vector3Int(-7, 6, 0), Owner.South); //northampton
        SetCityOwner(new Vector3Int(-10, 9, 0), Owner.South); //luton
        SetCityOwner(new Vector3Int(-20, 5, 0), Owner.South); //portsmouth
        SetCityOwner(new Vector3Int(-14, 6, 0), Owner.South); //reading
        */
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulateTurn();
        }
    }

    public void SetCityOwner(Vector3Int position, Owner owner, float strength = 5f)
    {
        cityPositions[position] = (owner, strength);

        if (tileDict.ContainsKey(position))
        {
            tileDict[position].owner = owner;
            tileDict[position].strength = strength;
        }
    }


    public List<Vector3Int> GetHexNeighbors(Vector3Int pos)
    {
        // Even-q vertical layout (Unity default for hex grids)
        int x = pos.x;
        int y = pos.y;

        int[][] evenOffsets = new int[][]
        {
        new int[] {+1, 0}, new int[] {0, +1}, new int[] {-1, +1},
        new int[] {-1, 0}, new int[] {-1, -1}, new int[] {0, -1}
        };

        int[][] oddOffsets = new int[][]
        {
        new int[] {+1, 0}, new int[] {+1, +1}, new int[] {0, +1},
        new int[] {-1, 0}, new int[] {0, -1}, new int[] {+1, -1}
        };

        int[][] offsets = (y % 2 == 0) ? evenOffsets : oddOffsets;

        List<Vector3Int> neighbors = new List<Vector3Int>();

        foreach (var offset in offsets)
        {
            Vector3Int neighbor = new Vector3Int(x + offset[0], y + offset[1], 0);
            if (tileDict.ContainsKey(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }
    public void SimulateTurn()
    {
        List<(Vector3Int attacker, Vector3Int target)> conversions = new();

        foreach (var kvp in tileDict)
        {
            var tile = kvp.Value;

            if (tile.owner == Owner.Neutral) continue;

            foreach (var neighbor in GetHexNeighbors(tile.position))
            {
                var neighborTile = tileDict[neighbor];
                if (neighborTile.owner == Owner.Neutral || neighborTile.owner != tile.owner)
                {
                    float myPower = tile.strength + Random.Range(0f, 2f);
                    float enemyPower = neighborTile.strength + Random.Range(0f, 1f);

                    if (myPower > enemyPower)
                        conversions.Add((tile.position, neighbor));
                }
            }
        }

        foreach (var conversion in conversions)
        {
            var attackerOwner = tileDict[conversion.attacker].owner;
            var targetPos = conversion.target;

            tileDict[targetPos].owner = attackerOwner;
            tileDict[targetPos].strength = 1f;

            // Check if target was a city
            if (cityPositions.ContainsKey(targetPos))
            {
                cityPositions[targetPos] = (attackerOwner, 5f); // Take over the city
                tileDict[targetPos].strength = 5f;

                // Update visual tile on map
                if (attackerOwner == Owner.North)
                    tilemap.SetTile(targetPos, northCityTile);
                else if (attackerOwner == Owner.South)
                    tilemap.SetTile(targetPos, southCityTile);
            }
            else
            {
                // Update visual tile for normal ground
                if (attackerOwner == Owner.North)
                    tilemap.SetTile(targetPos, northTile);
                else if (attackerOwner == Owner.South)
                    tilemap.SetTile(targetPos, southTile);
            }
        }
    }


}
