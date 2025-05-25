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
    public List<BSim_CityData> cityDataList; // Drag and drop CityData assets into this
    void Start()
    {
        // Initialize all tiles
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

        // Set initial city states from ScriptableObjects
        foreach (var cityData in cityDataList)
        {
            Vector3Int pos = cityData.position;
            Owner owner = cityData.owner;
            float strength = cityData.baseStrength;

            SetCityOwner(pos, owner, strength);
        }
    }
    /*
    // Add city positions manually
    cityPositions.Add(new Vector3Int(4, 2, 0), Owner.North);
    cityPositions.Add(new Vector3Int(10, 3, 0), Owner.South);

    // Manually mark cities here, or in a separate setup method
    SetCityOwner(new Vector3Int(-4, 0, 0), Owner.North); //Birmingham

    SetCityOwner(new Vector3Int(5, -2, 0), Owner.North); //manchester
    SetCityOwner(new Vector3Int(8, 2, 0), Owner.North); //leeds
    SetCityOwner(new Vector3Int(4, -7, 0), Owner.North); //liverpool
    SetCityOwner(new Vector3Int(4, 2, 0), Owner.North); //sheffield
    SetCityOwner(new Vector3Int(-14, -9, 0), Owner.North); //cardiff
    SetCityOwner(new Vector3Int(-5, 2, 0), Owner.North); //coventry
    SetCityOwner(new Vector3Int(8, 0, 0), Owner.North); //bradford
    SetCityOwner(new Vector3Int(0, 4, 0), Owner.North); //nottingham
    SetCityOwner(new Vector3Int(18, 2, 0), Owner.North); //newcastle-upon-tyne

    SetCityOwner(new Vector3Int(-14, 11, 0), Owner.South); //London

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
    void Update()
        {
        SimulateTurn();
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
        print("Set city in " + position + " owner as: " + owner);
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

            if (tile.owner == Owner.Neutral)
                continue;

            // Check for overwhelming enemy presence
            int enemyNeighborCount = 0;
            foreach (var neighbor in GetHexNeighbors(tile.position))
            {
                if (!tileDict.ContainsKey(neighbor)) continue;
                var neighborTile = tileDict[neighbor];
                if (neighborTile.owner != tile.owner && neighborTile.owner != Owner.Neutral)
                    enemyNeighborCount++;
            }

            if (enemyNeighborCount >= 5)
                continue;

            // Gather valid attack targets
            var potentialTargets = new List<Vector3Int>();
            foreach (var neighbor in GetHexNeighbors(tile.position))
            {
                if (!tileDict.ContainsKey(neighbor)) continue;
                var neighborTile = tileDict[neighbor];
                if (neighborTile.owner != tile.owner)
                    potentialTargets.Add(neighbor);
            }

            // Choose one random target and attempt attack
            if (potentialTargets.Count > 0)
            {
                Vector3Int chosenTarget = potentialTargets[Random.Range(0, potentialTargets.Count)];

                float myPower = tile.strength * Random.Range(0, 4);
                float enemyPower = tileDict[chosenTarget].strength;

                if (myPower > enemyPower * 1.2f) // Increase the multiplier to make attacks more likely to succeed
                    conversions.Add((tile.position, chosenTarget));
            }
        }

        // Apply conversions
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
                tileDict[targetPos].strength = Random.Range(1f, 3f); // weaken captured city

                // Update visual tile on map
                tilemap.SetTile(targetPos, attackerOwner == Owner.North ? northCityTile : southCityTile);
            }
            else
            {
                // Update normal tile visual
                tilemap.SetTile(targetPos, attackerOwner == Owner.North ? northTile : southTile);
            }
        }
    }
}
