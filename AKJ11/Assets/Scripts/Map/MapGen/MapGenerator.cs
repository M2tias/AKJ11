using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    TowerRoomGenerator towerRoomGenerator;
    void Start()
    {
        towerRoomGenerator = new TowerRoomGenerator(Configs.main.Tower);
        towerRoomGenerator.Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
