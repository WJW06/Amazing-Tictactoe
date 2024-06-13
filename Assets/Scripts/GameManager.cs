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
    public GameObject[] fieldObject;
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

    public UIManager uiManager;

    void Awake()
    {
        field = new int[36];
        player1_Arr = new int[36];
        player2_Arr = new int[36];
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
        playersCount = new int[2];

        for (int i = 0; i < 3; ++i)
        {
            int ranType = UnityEngine.Random.Range(0, 4);
            player1_Items[i].itemType = (Item.Type)ranType;
            ranType = UnityEngine.Random.Range(0, 4);
            player2_Items[i].itemType = (Item.Type)ranType;
        }
    }

    void Update()
    {
        ClickFeild();
        DecalField();
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
                            isUsedItem = false;
                            uiManager.ChangeItems(curPlayer);

                            if (playersCount[curPlayer] > 5)
                            {
                                CheckVictory(curPlayer);
                            }
                            ++turn;
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

                        if (curPlayer == 0)
                        {
                            player1_Items[curItemIndex].OnAblity(index);
                            UseItem(curItemIndex);
                            UsedItem(curItemIndex);
                        }
                        else if (curPlayer == 1)
                        {
                            player2_Items[curItemIndex].OnAblity(index);
                            UseItem(curItemIndex);
                            UsedItem(curItemIndex);
                        }
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
        }
    }

    public void UseItem(int index)
    {
        if (isUsedItem) return;
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

    void UsedItem(int index)
    {
        isUsedItem = true;
        uiManager.UsedItem(index);
    }

    public void ChangeCircle(int index)
    {
        Destroy(fieldObject[index].transform.GetChild(0).gameObject);
        int curPlayer = turn % 2;
        GameObject instCircle = Instantiate(circle[curPlayer], fieldObject[index].transform.position, Quaternion.identity);
        instCircle.transform.SetParent(fieldObject[index].transform);
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
                        case Item.Type.Grenade:
                            GrenadeDecal(index);
                            break;
                        case Item.Type.Shotgun:
                            ShotgunDecal(index);
                            break;
                        case Item.Type.ReverseCard:
                            ReverseCardDecal(index);
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
                        case Item.Type.Grenade:
                            GrenadeDecal(index);
                            break;
                        case Item.Type.Shotgun:
                            ShotgunDecal(index);
                            break;
                        case Item.Type.ReverseCard:
                            ReverseCardDecal(index);
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
            if (index - 1 > -1 && index - 1 < 36 && index % 6 > 0)
            {
                fieldColor = fieldObject[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = fieldObject[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = fieldObject[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void GrenadeDecal(int location)
    {
        for (int i = -2; i < 3; ++i)
        {
            int index = location + (6 * i);
            MeshRenderer fieldColor;
            if (index - 2 > -1 && index - 2 < 36 && index % 6 > 1)
            {
                fieldColor = fieldObject[index - 2].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index - 1 > -1 && index - 1 < 36 && index % 6 > 0)
            {
                fieldColor = fieldObject[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = fieldObject[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = fieldObject[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 2 > 1 && index + 2 < 36 && index % 6 < 4)
            {
                fieldColor = fieldObject[index + 2].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void ShotgunDecal(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            MeshRenderer fieldColor;
            if (index - 1 > -1 && index - 1 < 36 && index % 6 > 0)
            {
                fieldColor = fieldObject[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = fieldObject[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = fieldObject[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void ReverseCardDecal(int location)
    {
        MeshRenderer fieldColor = fieldObject[location].GetComponent<MeshRenderer>(); ;
        fieldColor.material.color = Color.blue;
    }

    void ClearFieldDecal()
    {
        foreach (GameObject field in fieldObject)
        {
            MeshRenderer fieldColor = field.GetComponent<MeshRenderer>();
            fieldColor.material.color = Color.white;
        }
    }
}
