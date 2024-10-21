using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public void PlayerHideGame()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayerSeekGame()
    {
        SceneManager.LoadScene(2);
    }
}
