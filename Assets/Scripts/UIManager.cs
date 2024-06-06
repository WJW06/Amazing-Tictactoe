using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playGround;
    public Button start_Button;
    public Button restart_Button;
    public Button[] Item_Buttons;

    void Awake()
    {

    }


    public void GameStart()
    {
        playGround.SetActive(true);
        start_Button.gameObject.SetActive(false);
        foreach (Button button in Item_Buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    public void GameEnd()
    {
        restart_Button.gameObject.SetActive(true);
        foreach (Button button in Item_Buttons)
        {
            button.gameObject.SetActive(false);
        }
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
