using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playGround;
    public Button start_Button;
    public Button restart_Button;

    void Awake()
    {

    }


    public void GameStart()
    {
        playGround.SetActive(true);
        start_Button.gameObject.SetActive(false);
    }

    public void GameEnd()
    {
        restart_Button.gameObject.SetActive(true);
    }

    public void GameRestart()
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");
        foreach (GameObject circle in circles)
        {
            circle.SetActive(false);
        }
        playGround.SetActive(false);
        start_Button.gameObject.SetActive(true);
        restart_Button.gameObject.SetActive(false);
    }
}
