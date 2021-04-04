
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TilesheetConfig", menuName = "Configs/TilesheetConfig")]

public class TilesheetConfig : ScriptableObject
{

    public static int GroundTileId = 12;

    [SerializeField]

    private List<TilesheetCase> cases;

    public List<TilesheetCase> Cases { get { return cases; } }

    [SerializeField]
    private Material material;

    public Texture2D Texture;
    RandomNumberGenerator rng;
    public Sprite GetSprite(int tileId)
    {
        if (rng == null) {
            rng = new RandomNumberGenerator("textures");
        }
        TilesheetCase tileSheetCase = cases.Find(tCase => tCase.TileId == tileId);
        return tileSheetCase.Get(rng);
    }

    public Sprite GetFirstSprite(int tileId)
    {
        TilesheetCase tileSheetCase = cases.Find(tCase => tCase.TileId == tileId);
        return tileSheetCase.Sprite;
    }

    public void LoadTextureCases()
    {
        Debug.Log("Clearing cases...");
        cases.Clear();
        if (Texture == null)
        {
            Debug.Log("Empty case!");
            cases.Add(null);
            return;
        }
        Debug.Log(Texture.name);
        List<Sprite> sprites = Resources.LoadAll<Sprite>("Images/Tilesets/" + Texture.name).ToList();
        cases = new List<TilesheetCase>();
        int index = 0;
        foreach (Sprite sprite in sprites)
        {
            cases.Add(new TilesheetCase(sprite, material, index));
            index += 1;
        }
        Debug.Log($"Got cases: {cases.Count}");
    }
}

[System.Serializable]
public class TilesheetCase
{
    public TilesheetCase(Sprite firstSprite, Material material, int tileId)
    {
        if (tiles == null)
        {
            tiles = new List<TilesheetCaseTile>();
        }
        if (tiles.Count > 0)
        {
            tiles[0].Sprite = firstSprite;
        }
        else
        {
            tiles.Add(new TilesheetCaseTile(firstSprite, material, DistributionMax));
        }
        TileId = tileId;
        caseName = firstSprite.name;
        sprite = firstSprite;
    }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get {return sprite;} }

    public int DistributionMin = 0;
    public int DistributionMax = 10;

    [SerializeField]
    private string caseName = "none";
    public string CaseName { get { return caseName; } }

    [SerializeField]
    private List<TilesheetCaseTile> tiles;
    public List<TilesheetCaseTile> Tiles { get { return tiles; } }
    [field:SerializeField]
    public int TileId { get; private set; }

    private List<TilesheetCaseTile> distributions;

    public Sprite Get(RandomNumberGenerator rng) {
        if (distributions == null || distributions.Count == 0) {
            InitDistributions();
        }
        if (distributions.Count > 0) {
            return distributions[rng.Range(0, distributions.Count)].Sprite;
        }
        return Tiles[0].Sprite;
    }

    public void CalculateSpawnChances() {
        InitDistributions();
        foreach(TilesheetCaseTile tile in Tiles) {
            if (distributions.Count == 0 || tile.Distribution == 0) {
                tile.ActualSpawnChance = 0;
            } else {
                tile.ActualSpawnChance = Mathf.Round(1.0f * tile.Distribution / (1.0f * distributions.Count) * 100.0f);
            }
        }
    }

    private void InitDistributions() {
        distributions = new List<TilesheetCaseTile>();
        foreach(TilesheetCaseTile tile in Tiles) {
            for(int index = 0; index < tile.Distribution; index += 1) {
                distributions.Add(tile);
            }
        }
    }
}

[System.Serializable]
public class TilesheetCaseTile
{

    public TilesheetCaseTile(Sprite sprite, Material material, int distributionMax)
    {
        this.sprite = sprite;
        this.material = material;
        Distribution = distributionMax;
    }
    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } set { sprite = value; } }

    public Material material;
    public Material Material { get { return material; } }
    public int CaseCount { get; set; }

    [SerializeField]
    public int Distribution;

    public float ActualSpawnChance;
}
