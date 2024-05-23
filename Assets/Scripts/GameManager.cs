using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int turn = 0;
    public GameObject[] circle;
    public int[] field;
    public int[] player1_Arr;
    public int[] player2_Arr;
    int[] playersCount;
    int[ , ] victoryCases;

    public UIManager uiManager;

    void Awake()
    {
        field = new int[36];
        player1_Arr = new int[36];
        player2_Arr = new int[36];
        victoryCases = new int[ , ] {
            { 0, 1, 2, 3, 4, 5 },
            { 6, 7, 8, 9, 10, 11 },
            { 12, 13, 14, 15, 16, 17 },
            { 18, 19, 20, 21, 22, 23 },
            { 24, 25, 26, 27, 28, 29 },
            { 30, 31, 32, 33, 34, 35},
            { 0, 6, 12, 18, 24, 30 },
            { 1, 7, 13, 19, 25, 31 },
            { 2, 8, 14, 20, 26, 32 },
            { 3, 9, 15, 21, 27, 33 },
            { 4, 10, 16, 22, 28, 34 },
            { 5, 11, 17, 23, 29, 35 },
            { 0, 7, 14, 21, 28, 35 },
            { 5, 10, 15, 20, 25, 30 },
            };
            playersCount = new int[2];
    }

    void Update()
    {
        ClickFeild();
    }

    void ClickFeild()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Floor")
                {
                    int curPlayer =  turn % 2;
                    int index = int.Parse(hit.transform.name.Split('_')[1]);
                    if (field[index] == 0)
                    {
                        Vector3 clickPos = hit.transform.position;
                        Instantiate(circle[curPlayer], clickPos, Quaternion.identity);
                        field[index] = curPlayer + 1;
                        if (curPlayer == 0) player1_Arr.SetValue(1, index);
                        else player2_Arr.SetValue(1, index);
                        ++playersCount[curPlayer];

                        if (playersCount[curPlayer] > 5)
                        {
                            CheckVictory(curPlayer);
                        }
                        ++turn;
                    }
                }
            }
        }
    }

    void CheckVictory(int curPlayer)
    {
        bool victory = false;
        int[] curArr = curPlayer == 0 ? player1_Arr : player2_Arr;
        for (int i = 0; i < 14; ++i)
        {
            for (int j = 0; j < 6; ++j)
            {
                if(curArr[victoryCases[i, j]] == 1) victory = true;
                else 
                {
                    victory = false;
                    break;
                }
            }
            if (victory) break;
        }

        if (victory)
        {
            uiManager.GameEnd();
            Debug.Log("플레이어" + curPlayer + " 승!");
        }
    }
}
