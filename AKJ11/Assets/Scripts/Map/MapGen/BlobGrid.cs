using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlobGrid
{
    // this lookup table was manually created using
    // this as a resource: https://www.boristhebrave.com/2013/07/14/tileset-roundup/
    public static Dictionary<int, int> lookup = new Dictionary<int, int> {
        {0, 15}, {2, 20}, {8, 38}, {10, 26}, {11, 23}, {16, 36}, {18, 24},
        {22, 21}, {24, 37}, {26, 25}, {27, 42}, {30, 41}, {31, 22}, {64, 0},
        {66, 10}, {72, 6}, {74, 16}, {75, 31}, {80, 4}, {82, 14}, {86, 32},
        {88, 5}, {90, 18}, {91, 34}, {94, 35}, {95, 8}, {104, 3}, {106, 43},
        {107, 13}, {120, 30}, {122, 44}, {123, 17}, {126, 47}, {127, 7},
        {208, 1}, {210, 40}, {214, 11}, {216, 33}, {218, 45}, {219, 46},
        {222, 19}, {223, 9}, {248, 2}, {250, 28}, {251, 27}, {254, 29}, {255, 12}
    };
    public static int EmptyTileId { get { return lookup[255]; } }

    public static async UniTask Run(NodeContainer nodeContainer)
    {
        await FindConfigurations(nodeContainer);
    }

    private static async UniTask FindConfigurations(NodeContainer nodeContainer)
    {
        DelayCounter counter = new DelayCounter(500);
        foreach(MapNode node in nodeContainer.Nodes) {
            BlobTileConfiguration blobConfig = FindConfiguration(nodeContainer, node.WorldX, node.WorldY);
            node.SetSpriteConfig(blobConfig.TileID);
            await Configs.main.Debug.DelayIfCounterFinished(counter);
        }
    }

    private static BlobTileConfiguration FindConfiguration(NodeContainer nodeContainer, int x, int y)
    {
        List<bool> neighbors = nodeContainer.GetNeighborVisibility(x, y);
        BlobTileConfiguration blobConfig;
        blobConfig.topLeftIsWall = neighbors[0];
        blobConfig.topIsWall = neighbors[1];
        blobConfig.topRightIsWall = neighbors[2];
        blobConfig.rightIsWall = neighbors[3];
        blobConfig.bottomRightIsWall = neighbors[4];
        blobConfig.bottomIsWall = neighbors[5];
        blobConfig.bottomLeftIsWall = neighbors[6];
        blobConfig.leftIsWall = neighbors[7];
        return blobConfig;
    }

}

public struct BlobTileConfiguration
{
    public bool topIsWall;
    public bool rightIsWall;
    public bool bottomIsWall;
    public bool leftIsWall;
    public bool topLeftIsWall;
    public bool bottomLeftIsWall;
    public bool topRightIsWall;
    public bool bottomRightIsWall;

    public int top { get { return BoolToInt(topIsWall); } }
    public int right { get { return BoolToInt(rightIsWall); } }
    public int bottom { get { return BoolToInt(bottomIsWall); } }
    public int left { get { return BoolToInt(leftIsWall); } }
    public int topLeft { get { return (!topIsWall || !leftIsWall) ? 0 : (topLeftIsWall ? 1 : 0); } }
    public int bottomRight { get { return (!bottomIsWall || !rightIsWall) ? 0 : (bottomRightIsWall ? 1 : 0); } }
    public int bottomLeft { get { return (!bottomIsWall || !leftIsWall) ? 0 : (bottomLeftIsWall ? 1 : 0); } }
    public int topRight { get { return (!topIsWall || !rightIsWall) ? 0 : (topRightIsWall ? 1 : 0); } }

    public int Config
    {
        get
        {
            return (
                topLeft + 2 * top + 4 * topRight +
                8 * left + 16 * right +
                32 * bottomLeft + 64 * bottom + 128 * bottomRight
            );
        }
    }

    public static int BoolToInt(bool boolValue)
    {
        return System.Convert.ToInt32(boolValue);
    }

    public int TileID
    {
        get { return BlobGrid.lookup[Config]; }
    }
}
