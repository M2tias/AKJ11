using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour
{
    public void NextLevel()
    {
        #if UNITY_EDITOR
        MapGenerator.main.LevelEnd();
        #endif
    }
}
