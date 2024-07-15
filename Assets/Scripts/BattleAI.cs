using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class BattleAI : MonoBehaviour
{
    public static BattleAI battleAI;
    public GameManager gameManager;

    int index;
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
        if (gameManager.playersCount[1] == 0) RandomIndex();
        else
        {
            ClearExcludedCases();
            ConnectionCircle();
        }
        gameManager.CreateCircle(index);
    }

    void RandomIndex()
    {
        while (true)
        {
            index = Random.Range(0, 36);
            if (gameManager.field[index] == 0) break;
        }
    }

    void ConnectionCircle()
    {
        int maxCase = -1;
        int maxCount = 0;
        for (int i = 0; i < 14; ++i)
        {
            if (excludedCases[i] == 1) continue;
            int count = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.player2_Arr[gameManager.victoryCases[i, j]] == 1)
                {
                    ++count;
                    continue;
                }
            }
            if (maxCount < count)
            {
                maxCount = count;
                maxCase = i;
            }
        }

        for (int i = 0; i < 6; ++i)
        {
            if (gameManager.field[gameManager.victoryCases[maxCase, i]] == 1) break;
            if (gameManager.field[gameManager.victoryCases[maxCase, i]] == 2) continue;
            index = gameManager.victoryCases[maxCase, i];
            break;
        }

        if (CheckAllExcludedCases()) RandomIndex();
        if (index == -1)
        {
            excludedCases.SetValue(1, maxCase);
            ConnectionCircle();
        }
    }

    bool CheckAllExcludedCases()
    {
        for (int i = 0; i < excludedCases.Length; ++i)
        {
            if (!excludedCases[i].Equals(1)) continue;
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
