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
        Dictionary<Vector3Int, float> tileBonusStrength = new();
        List<(Vector3Int attacker, Vector3Int target)> conversions = new();

        // === 1. Calculate tile bonus from cities ===
        foreach (var city in cityPositions)
        {
            Vector3Int cityPos = city.Key;
            Owner cityOwner = city.Value.owner;
            float cityStrength = city.Value.strength;

            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dy = Mathf.Max(-3, -dx - 3); dy <= Mathf.Min(3, -dx + 3); dy++)
                {
                    int dz = -dx - dy;
                    Vector3Int offset = new Vector3Int(dx, dy, dz);
                    Vector3Int pos = cityPos + offset;

                    if (!tileDict.ContainsKey(pos)) continue;
                    if (tileDict[pos].owner != cityOwner) continue;

                    int dist = GetHexDistance(cityPos, pos);
                    if (dist > 3) continue;

                    float bonus = Mathf.Max(0, cityStrength - dist);
                    if (!tileBonusStrength.ContainsKey(pos))
                        tileBonusStrength[pos] = 0;
                    tileBonusStrength[pos] += bonus;
                }
            }
        }

        // === 2. Tile combat with bonuses ===
        foreach (var kvp in tileDict)
        {
            var tile = kvp.Value;
            if (tile.owner == Owner.Neutral) continue;

            // Check if surrounded — skip attacking if surrounded by 5+ enemies
            int enemyCount = 0;
            var neighbors = GetHexNeighbors(tile.position);
            foreach (var neighbor in neighbors)
            {
                if (!tileDict.ContainsKey(neighbor)) continue;
                if (tileDict[neighbor].owner != tile.owner && tileDict[neighbor].owner != Owner.Neutral)
                    enemyCount++;
            }
            if (enemyCount >= 5) continue;

            // Choose one random enemy tile to attack
            var enemyNeighbors = neighbors.Where(n =>
                tileDict.ContainsKey(n) &&
                tileDict[n].owner != tile.owner &&
                tileDict[n].owner != Owner.Neutral
            ).ToList();

            if (enemyNeighbors.Count == 0) continue;

            var target = enemyNeighbors[Random.Range(0, enemyNeighbors.Count)];

            float basePower = tile.strength + Random.Range(0f, 2f);
            float bonus = tileBonusStrength.ContainsKey(tile.position) ? tileBonusStrength[tile.position] : 0f;
            float myPower = basePower + bonus;

            float enemyPower = tileDict[target].strength + Random.Range(0f, 1f);

            if (myPower > enemyPower)
                conversions.Add((tile.position, target));
        }

        // === 3. Apply conversions and visuals ===
        foreach (var conversion in conversions)
        {
            var attackerOwner = tileDict[conversion.attacker].owner;
            var targetPos = conversion.target;

            tileDict[targetPos].owner = attackerOwner;
            tileDict[targetPos].strength = 1f;

            if (cityPositions.ContainsKey(targetPos))
            {
                cityPositions[targetPos] = (attackerOwner, 3f); // Optional: cities lose strength on conquest
                tileDict[targetPos].strength = 3f;

                tilemap.SetTile(targetPos, attackerOwner == Owner.North ? northCityTile : southCityTile);
            }
            else
            {
                tilemap.SetTile(targetPos, attackerOwner == Owner.North ? northTile : southTile);
            }
        }

        // === 4. Apply tinting to contested tiles ===
        foreach (var kvp in tileDict)
        {
            var tile = kvp.Value;
            var neighbors = GetHexNeighbors(tile.position);

            bool isContested = neighbors.Any(n =>
                tileDict.ContainsKey(n) &&
                tileDict[n].owner != tile.owner &&
                tileDict[n].owner != Owner.Neutral
            );

            if (isContested)
            {
                float bonus = tileBonusStrength.ContainsKey(tile.position) ? tileBonusStrength[tile.position] : 0f;
                float tintIntensity = Mathf.Clamp01(bonus / 5f); // normalize for visual brightness

                // Example coloring: red for South, blue for North, intensity based on bonus
                Color color = Color.white;
                if (tile.owner == Owner.North)
                    color = Color.Lerp(Color.white, Color.cyan, tintIntensity);
                else if (tile.owner == Owner.South)
                    color = Color.Lerp(Color.white, Color.red, tintIntensity);

                tilemap.SetColor(tile.position, color);
            }
            else
            {
                tilemap.SetColor(tile.position, Color.white); // Reset color
            }
        }
    }

}
