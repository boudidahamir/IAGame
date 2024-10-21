using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool startGame;
    float startGameTimer;
    private void Awake()
    {
        startGame = false;
        startGameTimer = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startGame)
        {
            startGameTimer -= Time.deltaTime;
            if(startGameTimer < 0 )
            {
                startGame = true;
            }
        }
    }
}
