using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Configs : MonoBehaviour
{

    public static Configs main;
    void Awake()
    {
        main = this;
    }

    [field: SerializeField]
    public DebugConfig Debug { get; private set; }

    [field: SerializeField]
    public FollowTargetConfig Camera { get; private set; }

    [field: SerializeField]
    public CampaignStructureConfig Campaign { get; private set; }

    [field: SerializeField]
    public List<TorchConfig> TorchConfigs { get; private set; }

    public MusicConfig DefaultMusic;


    [field: SerializeField]
    public TilesheetConfig DefaultTileSheet {get; private set;}

}

