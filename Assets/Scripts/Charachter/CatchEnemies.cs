using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CatchEnemies : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject adhoc;
    public GameObject Astar;
    public GameObject Mcts;
    public GameObject interaction;

    private void Update()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(adhoc.transform.position.x, 0, adhoc.transform.position.z)) < 2f ||
    Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(Astar.transform.position.x, 0, Astar.transform.position.z)) < 1f ||
    Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(Mcts.transform.position.x, 0, Mcts.transform.position.z)) < 1f)
        {
            interaction.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameManager.seekerScore += 1;
                if(gameManager.seekerScore >= 3)
                {
                    gameManager.seekerScore = 3;
                }
                if (gameManager.seekerScore < 3)
                {
                    gameManager.ResetRound();
                }
            }
        }

        else
        {
            interaction.SetActive(false);
        }
    }
}
