using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AisCatchPlayer : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;


    private void Update()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.transform.position.x, 0, player.transform.position.z)) < 2f)
        {
            gameManager.seekerScore += 1;
            if (gameManager.seekerScore >= 3)
            {
                gameManager.seekerScore = 3;
            }
            if (gameManager.seekerScore < 3)
            {
                gameManager.ResetRound();
            }
        }
    }
}
