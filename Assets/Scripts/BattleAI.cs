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
    int hammer_Index;
    int handGun_Index;
    int shotGun_Index;
    int wildCard_Index;
    //int[] player1_Excluded;
    //int[] player2_Excluded;

    void Awake()
    {
        battleAI = this;
    }

    public void Think()
    {
        index = -1;

        CountingPlayer1();
        CountingPlayer2();
        CheckItems();

        Debug.Log("maxCount_P1: "+maxCount_Player1);
        Debug.Log("maxCount_P2: "+maxCount_Player2);
        if (wildCard_Index != -1)
        {
            UseWildCard();
            if (index != -1)
            {
                UseItem(wildCard_Index);
                return;
            }
        }
        if (maxCount_Player1 >= 4 && maxCount_Player1 > maxCount_Player2)
        {
            BlockingCircle();
        }
        else if (maxCount_Player2 != 0)
        {
            ConnectionCircle();
        }
        else
        {
            RandomIndex();
        }
        gameManager.CreateCircle(index);
    }

    void CountingPlayer1()
    {
        //if (CheckPlayer1ExcludedCases()) return;
        maxCount_Player1 = 0;
        maxCase_Player1 = -1;
        for (int i = 0; i < 14; ++i)
        {
            int count_Player1 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.field[gameManager.victoryCases[i, j]] == 2)
                {
                    count_Player1 = 0;
                    break;
                }
                else if (gameManager.field[gameManager.victoryCases[i, j]] == 1)
                {
                    ++count_Player1;
                }
            }
            if (maxCount_Player1 < count_Player1)
            {
                maxCount_Player1 = count_Player1;
                maxCase_Player1 = i;
            }
        }

        if (maxCase_Player1 == -1) return;

        Debug.Log("maxCase_P1:" + maxCase_Player1);
    }

    void CountingPlayer2()
    {
        //if (CheckPlayer2ExcludedCases()) return;
        maxCount_Player2 = 0;
        maxCase_Player2 = -1;
        for (int i = 0; i < 14; ++i)
        {
            int count_Player2 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.field[gameManager.victoryCases[i, j]] == 1)
                {
                    count_Player2 = 0;
                    break;
                }
                else if (gameManager.field[gameManager.victoryCases[i, j]] == 2)
                {
                    ++count_Player2;
                }
            }
            if (maxCount_Player2 < count_Player2)
            {
                maxCount_Player2 = count_Player2;
                maxCase_Player2 = i;
            }
        }

        if (maxCase_Player2 == -1) return;

        Debug.Log("maxCase_P2:" + maxCase_Player2);
    }

    void CheckItems()
    {
        hammer_Index = -1;
        handGun_Index = -1;
        shotGun_Index = -1;
        wildCard_Index = -1;
        for (int i = 0; i < gameManager.player2_Items.Length; ++i)
        {
            switch (gameManager.player2_Items[i].itemType)
            {
                case Item.Type.Hammer:
                    hammer_Index = i;
                    break;
                case Item.Type.HandGun:
                    handGun_Index = i;
                    break;
                case Item.Type.Shotgun:
                    shotGun_Index = i;
                    break;
                case Item.Type.WildCard:
                    wildCard_Index = i;
                    break;
            }
        }
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

    void UseItem(int type)
    {
        gameManager.player2_Items[type].OnAblity(index);

        gameManager.UseItem(type);
        gameManager.UsedItem(1, type);
        gameManager.ChangeTurn(1);
    }

    void UseWildCard()
    {
        int count_Player1;
        int count_Player2;
        int temp_Index = -1;

        for (int i = 0; i < 14; ++i)
        {
            count_Player2 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.field[gameManager.victoryCases[i, j]] == 2)
                {
                    ++count_Player2;
                }
                else if (gameManager.field[gameManager.victoryCases[i, j]] == 1)
                {
                    temp_Index = gameManager.victoryCases[i, j];
                }
            }
            if (count_Player2 == 5)
            {
                index = temp_Index;
                return;
            }
        }

        for (int i = 0; i < 14; ++i)
        {
            count_Player1 = 0;
            for (int j = 0; j < 6; ++j)
            {
                if (gameManager.field[gameManager.victoryCases[i, j]] == 1)
                {
                    ++count_Player1;
                    temp_Index = gameManager.victoryCases[i, j];
                }
            }
            if (count_Player1 == 5)
            {
                index = temp_Index;
                return;
            }
        }
    }

    //bool CheckPlayer1ExcludedCases()
    //{
    //    for (int i = 0; i < player1_Excluded.Length; ++i)
    //    {
    //        if (player1_Excluded[i].Equals(1)) continue;
    //        else return false;
    //    }
    //    return true;
    //}

    //bool CheckPlayer2ExcludedCases()
    //{
    //    for (int i = 0; i < player2_Excluded.Length; ++i)
    //    {
    //        if (player2_Excluded[i].Equals(1)) continue;
    //        else return false;
    //    }
    //    return true;
    //}

    //public void ClearExcludedCases()
    //{
    //    for (int i = 0; i < 14; ++i)
    //    {
    //        player1_Excluded[i] = 0;
    //        player2_Excluded[i] = 0;
    //    }
    //}
}
