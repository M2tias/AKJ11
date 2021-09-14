using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZarguufDeath : MonoBehaviour
{
    private bool dead = false;
    public void Death()
    {
        dead = true;
        GameOverView.main.Show();
        GameStateManager.main.StopTime();
        GameStateManager.main.GameOver(false);
        //Destroy(gameObject);
    }

    private void Update() {
        if (dead && Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }
    }
}
