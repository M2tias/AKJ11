
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TileStyle", menuName = "Configs/TileStyle")]

public class TileStyle : ScriptableObject
{

    public static int GroundTileId = 12;

    [field: SerializeField]
    public Color ColorTint { get; private set; } = Color.white;
    [field: SerializeField]
    public Color GroundTint { get; private set; } = Color.white;
    [SerializeField]
    private Sprite groundSprite;
    public Sprite GroundSprite { get { return groundSprite == null ? cases[TileStyle.GroundTileId] : groundSprite; } }

    [field: SerializeField]
    public int LayerOrder { get; private set; } = 0;

    [SerializeField]
    private Texture2D texture;
    [SerializeField]
    [HideInInspector]
    private Texture2D previousTexture;

    [SerializeField]
    private List<Sprite> cases;
    public List<Sprite> Cases { get { return cases; } }


    public void OnValidate()
    {
        LoadNewTexturesIfNeeded();
    }

    public void LoadNewTexturesIfNeeded()
    {
        if (previousTexture != texture || previousTexture == null || cases == null || cases.Count < 5)
        {
            LoadTextureCases();
            previousTexture = texture;
        }
    }

    private void LoadTextureCases()
    {
        Debug.Log("Clearing cases...");
        cases.Clear();
        if (texture == null)
        {
            Debug.Log("Empty case!");
            cases.Add(null);
            return;
        }
        Debug.Log(texture.name);
        cases = Resources.LoadAll<Sprite>("Images/Tilesets/" + texture.name).ToList();
        Debug.Log($"Got cases: {cases.Count}");
    }

}