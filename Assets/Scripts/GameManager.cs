using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GameManager : MonoBehaviour
{
    public int turn = 0;
    public GameObject[] circle;
    public GameObject[] fieldObjects;
    public int[] field;
    public int[] player1_Arr;
    public int[] player2_Arr;
    int[] playersCount;
    int[,] victoryCases;
    public Item[] player1_Items;
    public Item[] player2_Items;
    bool isUsingItem = false;
    bool isUsedItem = false;
    int curItemIndex;
    int curDecalIndex = -1;
    int[] playerItemCount;
    bool isPlaying = false;
    WaitForSeconds startDelay = new WaitForSeconds(0.1f);

    public UIManager uiManager;

    void Awake()
    {
        field = new int[36];
        player1_Arr = new int[36];
        player2_Arr = new int[36];
        playersCount = new int[2];
        playerItemCount = new int[2];

        victoryCases = new int[,] {
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
    }

    void Update()
    {
        if (isPlaying)
        {
            ClickFeild();
            DecalField();
        }
    }

    public void InitField()
    {
        for (int i = 0; i < 36; ++i)
        {
            field[i] = 0;
            player1_Arr[i] = 0;
            player2_Arr[i] = 0;
        }
        playersCount[0] = 0;
        playersCount[1] = 0;
        CreateItems(0);
        CreateItems(1);
        turn = 0;
        isUsedItem = false;
        StartCoroutine(StartDelay());
    }

    public void ClearField()
    {
        int index = 0;
        foreach (GameObject fieldObject in fieldObjects)
        {
            if (field[index++] != 0) Destroy(fieldObject.transform.GetChild(0).gameObject);
        }
    }

    void ClickFeild()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                int curPlayer = turn % 2;
                int index = -1;
                if (!isUsingItem)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        index = int.Parse(hit.transform.name.Split('_')[1]);
                        if (field[index] == 0)
                        {
                            Vector3 clickPos = hit.transform.position;
                            GameObject instCircle = Instantiate(circle[curPlayer], clickPos, Quaternion.identity);
                            instCircle.transform.SetParent(hit.transform);
                            field[index] = curPlayer + 1;
                            if (curPlayer == 0) player1_Arr.SetValue(1, index);
                            else player2_Arr.SetValue(1, index);
                            ++playersCount[curPlayer];

                            if (playersCount[curPlayer] > 5)
                            {
                                CheckVictory(curPlayer);
                            }

                            if (isPlaying) ChangeTurn(curPlayer);
                        }
                    }
                }
                else
                {
                    if (hit.transform.tag == "Floor" || hit.transform.tag == "Circle")
                    {
                        if (hit.transform.tag == "Floor")
                        {
                            index = int.Parse(hit.transform.name.Split('_')[1]);
                        }
                        else if (hit.transform.tag == "Circle")
                        {
                            index = int.Parse(hit.transform.parent.parent.name.Split('_')[1]);
                        }

                        if (curPlayer == 0) player1_Items[curItemIndex].OnAblity(index);
                        else if (curPlayer == 1) player2_Items[curItemIndex].OnAblity(index);

                        UseItem(curItemIndex);
                        UsedItem(curPlayer, curItemIndex);
                        if (isPlaying) ChangeTurn(curPlayer);
                    }
                }
            }
        }
    }

    void ChangeTurn(int curPlayer)
    {
        isUsedItem = false;
        uiManager.ChangeUI(curPlayer);
        if (curPlayer == 0 && playerItemCount[0] == 0)
        {
            CreateItems(0);
        }
        else if (curPlayer == 1 && playerItemCount[1] == 0)
        {
            CreateItems(1);
        }
        ++turn;
    }

    void CheckVictory(int curPlayer)
    {
        bool victory = false;
        int[] curArr = curPlayer == 0 ? player1_Arr : player2_Arr;
        for (int i = 0; i < 14; ++i)
        {
            for (int j = 0; j < 6; ++j)
            {
                if (curArr[victoryCases[i, j]] == 1) victory = true;
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
            uiManager.GameEnd(curPlayer);
            Debug.Log("플레이어" + curPlayer + " 승!");
            isPlaying = false;
        }
    }

    public void CreateItems(int player)
    {
        Item[] items;
        if (player == 0)
        {
            items = player1_Items;
            playerItemCount[0] = 3;   
        }
        else
        {
            items = player2_Items;
            playerItemCount[1] = 3;   
        }

        for (int i = 0; i < 3; ++i)
        {
            int ranType = UnityEngine.Random.Range(0, 6);
            switch (ranType)
            {
                // Hammer is 0
                case 1:
                case 2:
                    ranType = 1; // HandGun
                    break;
                case 3:
                case 4:
                    ranType = 2; // ShotGun
                    break;
                case 5:
                    ranType = 3; // WildCard
                    break;
            }
            items[i].itemType = (Item.Type)ranType;
            items[i].isUsed = false;
        }
    }

    public void UseItem(int index)
    {
        if (isUsedItem) return;
        if (turn % 2 == 0 && player1_Items[index].isUsed) return;
        else if (turn % 2 == 1 && player2_Items[index].isUsed) return;

        if (isUsingItem && curItemIndex != index)
        {
            isUsingItem = !isUsingItem;
            uiManager.UsingItem(curItemIndex, isUsingItem);
        }
        isUsingItem = !isUsingItem;
        curItemIndex = index;
        uiManager.UsingItem(index, isUsingItem);
        ClearFieldDecal();
    }

    void UsedItem(int curPlayer,int index)
    {
        isUsedItem = true;
        if (turn % 2 == 0)
        {
            player1_Items[index].isUsed = true;
            --playerItemCount[0];
        }
        else
        {
            player2_Items[index].isUsed = true;
            --playerItemCount[1];
        }
        uiManager.UsedItem(index);
    }

    public void DestroyCircle(int index)
    {
        Circle circle = fieldObjects[index].transform.GetChild(0).gameObject.GetComponent<Circle>();
        if (!circle.circleType)
        {
            player1_Arr[index] = 0;
            --playersCount[0];
        }
        else
        {
            player2_Arr[index] = 0;
            --playersCount[1];
        }
        field[index] = 0;
        Destroy(fieldObjects[index].transform.GetChild(0).gameObject);
    }

    public void ChangeCircle(int index)
    {
        Destroy(fieldObjects[index].transform.GetChild(0).gameObject);
        int curPlayer = turn % 2;
        GameObject instCircle = Instantiate(circle[curPlayer], fieldObjects[index].transform.position, Quaternion.identity);
        instCircle.transform.SetParent(fieldObjects[index].transform);
        field[index] = field[index] == 1 ? 2 : 1;
        if (curPlayer == 0)
        {
            player1_Arr[index] = 1;
            player2_Arr[index] = 0;
        }
        else
        {
            player1_Arr[index] = 0;
            player2_Arr[index] = 1;
        }
        CheckVictory(curPlayer);
    }

    void DecalField()
    {
        if (!isUsingItem) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            int index = -1;
            if (hit.transform.tag == "Floor" || hit.transform.tag == "Circle")
            {
                if (hit.transform.tag == "Floor")
                {
                    index = int.Parse(hit.transform.name.Split('_')[1]);
                }
                else if (hit.transform.tag == "Circle")
                {
                    index = int.Parse(hit.transform.parent.parent.name.Split('_')[1]);
                }

                if (curDecalIndex != index && curDecalIndex != -1) ClearFieldDecal();
                curDecalIndex = index;

                if (turn % 2 == 0)
                {
                    switch (player1_Items[curItemIndex].itemType)
                    {
                        case Item.Type.Hammer:
                            HammerDecal(index);
                            break;
                        case Item.Type.HandGun:
                            HandGunDecal();
                            break;
                        case Item.Type.Shotgun:
                            ShotgunDecal(index);
                            break;
                        case Item.Type.WildCard:
                            WildCardDecal(index);
                            break;
                    }
                }
                else
                {
                    switch (player2_Items[curItemIndex].itemType)
                    {
                        case Item.Type.Hammer:
                            HammerDecal(index);
                            break;
                        case Item.Type.HandGun:
                            HandGunDecal();
                            break;
                        case Item.Type.Shotgun:
                            ShotgunDecal(index);
                            break;
                        case Item.Type.WildCard:
                            WildCardDecal(index);
                            break;
                    }
                }
            }
        }
    }

    void HammerDecal(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            MeshRenderer fieldColor;
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0)
            {
                fieldColor = fieldObjects[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = fieldObjects[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = fieldObjects[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void HandGunDecal()
    {
        for (int i = 0; i < 36; ++i)
        {
            MeshRenderer fieldColor;
            fieldColor = fieldObjects[i].GetComponent<MeshRenderer>();
            fieldColor.material.color = Color.blue;
        }
    }

    void ShotgunDecal(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            MeshRenderer fieldColor;
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0)
            {
                fieldColor = fieldObjects[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = fieldObjects[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = fieldObjects[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void WildCardDecal(int location)
    {
        MeshRenderer fieldColor = fieldObjects[location].GetComponent<MeshRenderer>(); ;
        fieldColor.material.color = Color.blue;
    }

    void ClearFieldDecal()
    {
        foreach (GameObject field in fieldObjects)
        {
            MeshRenderer fieldColor = field.GetComponent<MeshRenderer>();
            fieldColor.material.color = Color.white;
        }
    }

    IEnumerator StartDelay()
    {
        yield return startDelay;
        isPlaying = true;
    }
}
