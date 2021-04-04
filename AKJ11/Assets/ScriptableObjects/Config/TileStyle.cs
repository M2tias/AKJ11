
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TileStyle", menuName = "Configs/TileStyle")]

public class TileStyle : ScriptableObject
{

    [field: SerializeField]
    public Color ColorTint { get; private set; } = Color.white;
    [field: SerializeField]
    public Color GroundTint { get; private set; } = Color.white;

    [SerializeField]
    private Sprite groundSprite;
    public Sprite GroundSprite { get { return groundSprite; } }
    public Sprite DefaultGroundSprite { get { return groundSprite == null ? GetDefaultSprite() : groundSprite; } }


    [field: SerializeField]
    public int LayerOrder { get; private set; } = 0;

    [SerializeField]
    private TilesheetConfig tilesheetConfig;
    public TilesheetConfig TilesheetConfig { get { return tilesheetConfig; } }

    public Sprite GetSprite(int tileId) {
        return tilesheetConfig.GetSprite(tileId);
    }

    public Sprite GetDefaultSprite() {
        if (tilesheetConfig != null) {
            return tilesheetConfig.GetFirstSprite(BlobGrid.EmptyTileId);
        }
        return null;
    }

}