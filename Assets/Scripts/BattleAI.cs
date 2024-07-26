using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class BattleAI : MonoBehaviour
{
    public static BattleAI battleAI;
    public GameManager gameManager;

    int index;
    int maxCount_Player1;
    int maxCount_Player2;
    int maxCase_Player1;
    int maxCase_Player2;
    bool hasHammer;
    bool hasHandGun;
    bool hasShotGun;
    bool hasWildCard;
    int[] excludedCases;

    void Awake()
    {
        battleAI = this;
        excludedCases = new int[14];
    }

    public void Think()
    {
        index = -1;
        CountingPlayer1();
        CountingPlayer2();
        Debug.Log("maxCount_P1: "+maxCount_Player1);
        Debug.Log("maxCount_P2: "+maxCount_Player2);
        if (maxCount_Player1 >= 4 && maxCount_Player1 > maxCount_Player2) BlockingCircle();
        else if (maxCount_Player2 == 0 && index == -1) RandomIndex();
        else if (index == -1) ConnectionCircle();
        gameManager.CreateCircle(index);
    }

    void CountingPlayer1()
    {
        ClearExcludedCases();
        maxCount_Player1 = 0;
        maxCase_Player1 = -1;
        for (int i = 0; i < 14; ++i)
        {
            if (excludedCases[i] == 1) continue;
            int count_Player1 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.player1_Arr[gameManager.victoryCases[i, j]] == 1)
                {
                    ++count_Player1;
                    continue;
                }
            }
            if (maxCount_Player1 < count_Player1)
            {
                maxCount_Player1 = count_Player1;
                maxCase_Player1 = i;
            }
        }

        if (CheckAllExcludedCases() || maxCase_Player1 == -1)
        {
            RandomIndex();
            return;
        }

        bool isCan = true;
        for (int i = 0; i < 6; ++i)
        {
            if (gameManager.field[gameManager.victoryCases[maxCase_Player1, i]] == 2)
            {
                isCan = false;
                break;
            }
        }

        Debug.Log("maxCase_P1:" + maxCase_Player1);
        // if (!isCan)
        // {
        //     excludedCases.SetValue(1, maxCase_Player1);
        //     CountingPlayer1();
        // }
    }

    void CountingPlayer2()
    {
        ClearExcludedCases();
        maxCount_Player2 = 0;
        maxCase_Player2 = -1;
        for (int i = 0; i < 14; ++i)
        {
            if (excludedCases[i] == 1) continue;
            int count_Player2 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.player2_Arr[gameManager.victoryCases[i, j]] == 1)
                {
                    ++count_Player2;
                    continue;
                }
            }
            if (maxCount_Player2 < count_Player2)
            {
                maxCount_Player2 = count_Player2;
                maxCase_Player2 = i;
            }
        }

        if (CheckAllExcludedCases() || maxCase_Player2 == -1)
        {
            RandomIndex();
            return;
        }

        bool isCan = true;
        for (int i = 0; i < 6; ++i)
        {
            if (gameManager.field[gameManager.victoryCases[maxCase_Player2, i]] == 1)
            {
                isCan = false;
                break;
            }
        }

        Debug.Log("maxCase_P2:" + maxCase_Player2);
        if (!isCan) excludedCases.SetValue(1, maxCase_Player2);
        // if (!isCan)
        // {
        //     CountingPlayer2();
        // }
    }

    void RandomIndex()
    {
        Debug.Log("Random");
        while (true)
        {
            index = Random.Range(0, 36);
            if (gameManager.field[index] == 0) break;
        }
    }

    void ConnectionCircle()
    {
        Debug.Log("Connection");
        for (int i = 0; i < 6; ++i)
        {
            if (gameManager.field[gameManager.victoryCases[maxCase_Player2, i]] == 2) continue;
            index = gameManager.victoryCases[maxCase_Player2, i];
            break;
        }
    }

    void BlockingCircle()
    {
        Debug.Log("Blocking");  
        for (int i = 0; i < 6; ++i)
        {
            if (gameManager.field[gameManager.victoryCases[maxCase_Player1, i]] == 1) continue;
            index = gameManager.victoryCases[maxCase_Player1, i];
            break;
        }
    }

    bool CheckAllExcludedCases()
    {
        for (int i = 0; i < excludedCases.Length; ++i)
        {
            if (excludedCases[i].Equals(1)) continue;
            else return false;
        }
        return true;
    }

    void ClearExcludedCases()
    {
        for (int i = 0; i < 14; ++i)
        {
            excludedCases[i] = 0;
        }
    }
}
