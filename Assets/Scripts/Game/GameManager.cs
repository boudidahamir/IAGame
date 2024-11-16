using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool startGame;
    public bool stopGame;
    public GameType gameType;
    public TextMeshProUGUI startTimer;
    public TextMeshProUGUI Scores;
    public TextMeshProUGUI RoundTimer;
    public GameObject image;
    public float roundTimer = 180f;
    public float roundTime = 180f;
    public float startGameTimer = 10f;
    public int seekerScore = 0, hiderScore = 0;

    public GameObject player;
    Vector3 initialPlayerPos;

    public GameObject adhoc;
    Vector3 initialAdhocPos;

    public GameObject Astar;
    Vector3 initialAstarPos;

    public GameObject Mcts;
    Vector3 initialMctsPos;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "PlayerSeek")
            gameType = GameType.PlayerSeek;
        else
            gameType= GameType.PlayerHide;

        startGame = false;

        initialPlayerPos = player.transform.position;
        initialAdhocPos = adhoc.transform.position;
        initialAstarPos = Astar.transform.position;
        initialMctsPos = Mcts.transform.position;
    }

    void Update()
    {
        if(!startGame)
        {
            startGameTimer -= Time.deltaTime;
            startTimer.text = Mathf.Floor(startGameTimer).ToString("F0");
            if (startGameTimer < 0 )
            {
                if(image != null)
                    image.SetActive(false);
                startTimer.gameObject.SetActive(false);
                startGame = true;
            }
        }
        else
        {
            Scores.text = "Hiders : "+hiderScore.ToString()+"\nSeekers : "+seekerScore.ToString();
            roundTimer -= Time.deltaTime;
            RoundTimer.text = Mathf.Floor(roundTimer).ToString("F0");
            if(roundTimer < 0)
            {
                hiderScore += 1;
                Scores.text = "Hiders : " + hiderScore.ToString() + "\nSeekers : " + seekerScore.ToString();
                if(hiderScore < 3 )
                { ResetRound(); }
            }

            if(hiderScore == 3)
            {
                Scores.text = "";
                RoundTimer.text = "";
                startTimer.gameObject.SetActive(true);
                startTimer.text = "Hiders Win";
                Time.timeScale = 0;
            }

            if (seekerScore == 3)
            {
                Scores.text = "";
                RoundTimer.text = ""; 
                startTimer.gameObject.SetActive(true);
                startTimer.text = "Seekers Win";
                Time.timeScale = 0;
            }
        }
    }

    public void ResetRound()
    {
        if (seekerScore != 3 && hiderScore != 3)
        {
            Time.timeScale = 1;
            RoundTimer.text = "";
            player.transform.position = initialPlayerPos;
            adhoc.transform.position = initialAdhocPos;
            Astar.transform.position = initialAstarPos;
            Mcts.transform.position = initialMctsPos;
            startGameTimer = 10;
            startGame = false;
            if (image != null)
                image.SetActive(true);
            startTimer.gameObject.SetActive(true);
            roundTimer = roundTime;
        }
        
    }
}

public enum GameType { PlayerHide, PlayerSeek}
