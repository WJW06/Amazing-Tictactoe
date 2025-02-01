using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour, IPunObservable
{
    public class Backs
    {
        public int[] m_field;
        public int m_P1Item_1;
        public bool m_P1ItemUsed_1;
        public int m_P1Item_2;
        public bool m_P1ItemUsed_2;
        public int m_P1Item_3;
        public bool m_P1ItemUsed_3;
        public int m_P2Item_1;
        public bool m_P2ItemUsed_1;
        public int m_P2Item_2;
        public bool m_P2ItemUsed_2;
        public int m_P2Item_3;
        public bool m_P2ItemUsed_3;

        public Backs(int[] field, int p1Item_1, bool p1ItemUsed_1,
            int p1Item_2, bool p1ItemUsed_2, int p1Item_3, bool p1ItemUsed_3,
            int p2Item_1, bool p2ItemUsed_1, int p2Item_2, bool p2ItemUsed_2,
            int p2Item_3, bool p2ItemUsed_3)
        {
            m_field = (int[])field.Clone();
            m_P1Item_1 = p1Item_1;
            m_P1ItemUsed_1 = p1ItemUsed_1;
            m_P1Item_2 = p1Item_2;
            m_P1ItemUsed_2 = p1ItemUsed_2;
            m_P1Item_3 = p1Item_3;
            m_P1ItemUsed_3 = p1ItemUsed_3;
            m_P2Item_1 = p2Item_1;
            m_P2ItemUsed_1 = p2ItemUsed_1;
            m_P2Item_2 = p2Item_2;
            m_P2ItemUsed_2 = p2ItemUsed_2;
            m_P2Item_3 = p2Item_3;
            m_P2ItemUsed_3 = p2ItemUsed_3;
        }
    }

    public static GameManager gameManager;
    public PhotonView PV;

    public bool isAIBattle;
    public int turn = 0;
    public Floor[] floors;
    public int[] field;
    public int[] player1_Arr;
    public int[] player2_Arr;
    public int[] playersCount;
    public int[] itemsCount;
    public int[,] victoryCases;
    public Item[] player1_Items;
    public Item[] player2_Items;
    bool isUsingItem = false;
    bool isUsedItem = false;
    int curItemIndex;
    int[] playerItemCount;
    Stack<Backs> backs;
    bool isBacksiesing = false;
    int curDecalIndex = -1;
    public bool isUsingHandGun = false;
    bool isPlaying = false;
    bool isCanBacksies = false;
    bool isOnline = false;
    bool isP2 = false;
    bool isSecondCreate = false;
    WaitForSeconds startDelay = new WaitForSeconds(0.1f);

    void Awake()
    {
        gameManager = this;

        field = new int[36];
        player1_Arr = new int[36];
        player2_Arr = new int[36];
        playersCount = new int[2];
        playerItemCount = new int[2];
        itemsCount = new int[2];
        backs = new Stack<Backs>();

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
            if (isOnline && ((!isP2 && turn % 2 == 1) || (isP2 && turn % 2 == 0))) return;
            ClickField();
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
        isSecondCreate = false;
        backs.Clear();

        if (isOnline)
        {
            print("OnlineItem");
            OnlineItems(0);
            OnlineItems(1);
        }
        else
        {
            print("OfflineItem");
            CreateItems(0);
            CreateItems(1);
        }

        turn = 0;
        isUsedItem = false;

        StartCoroutine("StartDelay");
    }

    public bool GetPlaying()
    {
        return isPlaying;
    }

    public void SetPlaying(bool playing)
    {
        isPlaying = playing;
    }

    public void SetCanBacksies(bool bCanBacksies)
    {
        isCanBacksies = bCanBacksies;
    }

    public bool GetCanBacksies()
    {
        return isCanBacksies;
    }

    public void SetOnline(bool online)
    {
        isOnline = online;
    }

    public bool GetOnline()
    {
        return isOnline;
    }

    public void SetP2(bool p2)
    {
        isP2 = p2;
    }

    public bool GetP2()
    {
        return isP2;
    }

    public void ClearField()
    {
        foreach (Floor floor in floors)
        {
            floor.UnSetCircle();
        }
    }

    [PunRPC]
    public void SaveBack()
    {
        if (isCanBacksies)
        {
            backs.Push(new Backs(field,
                (int)player1_Items[0].itemType, player1_Items[0].isUsed,
                (int)player1_Items[1].itemType, player1_Items[1].isUsed,
                (int)player1_Items[2].itemType, player1_Items[2].isUsed,
                (int)player2_Items[0].itemType, player2_Items[0].isUsed,
                (int)player2_Items[1].itemType, player2_Items[1].isUsed,
                (int)player2_Items[2].itemType, player2_Items[2].isUsed));
        }
    }


    void AIThink()
    {
        BattleAI.battleAI.Think();
        CancelInvoke("AIThink");
    }

    void ClickField()
    {
        if (isUsingHandGun) return;
        if (isAIBattle && turn % 2 == 1)
        {
            Invoke("AIThink", 0.5f);
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                int curPlayer = turn % 2;
                int index = -1;

                if (!isUsingItem) // 아이템 미사용
                {
                    if (hit.transform.tag == "Floor")
                    {
                        if (!isOnline) SaveBack();
                        else PV.RPC("SaveBack", RpcTarget.All);

                        Floor floor = hit.transform.GetComponent<Floor>();
                        index = floor.floorIndex;
                        if (isOnline) PV.RPC("CreateCircle", RpcTarget.All, index, curPlayer);
                        else CreateCircle(index, curPlayer);
                    }
                }
                else // 아이템 사용
                {
                    if (hit.transform.tag == "Floor" || hit.transform.tag == "Circle")
                    {
                        if (!isOnline) SaveBack();
                        else PV.RPC("SaveBack", RpcTarget.All);

                        if (hit.transform.tag == "Floor")
                        {
                            Floor floor = hit.transform.GetComponent<Floor>();
                            index = floor.floorIndex;
                        }
                        else if (hit.transform.tag == "Circle")
                        {
                            Floor floor = hit.transform.GetComponentInParent<Floor>();
                            index = floor.floorIndex;
                        }

                        if (isOnline) PV.RPC("OnAbility", RpcTarget.All, curPlayer, index);
                        else OnAbility(curPlayer, index);

                        UseItem(curItemIndex);
                        
                        if (isOnline)
                        {
                            PV.RPC("UsedItem", RpcTarget.All, curPlayer, curItemIndex);
                            if (isPlaying) PV.RPC("ChangeTurn", RpcTarget.All, curPlayer);
                        }
                        else
                        {
                            UsedItem(curPlayer, curItemIndex);
                            if (isPlaying) ChangeTurn(curPlayer);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void CreateCircle(int index, int curPlayer)
    {
        if (field[index] == 0 || isBacksiesing)
        {
            floors[index].SetCircle(curPlayer);
            field[index] = curPlayer + 1;
            if (curPlayer == 0) player1_Arr.SetValue(1, index);
            else player2_Arr.SetValue(1, index);
            AudioManager.audioManager.PlaySFX((AudioManager.SFX)curPlayer);
            ++playersCount[curPlayer];

            if (playersCount[curPlayer] > 5)
            {
                CheckVictory(curPlayer);
            }

            if (isPlaying && !isBacksiesing) ChangeTurn(curPlayer);
        }
    }

    [PunRPC]
    public void ChangeTurn(int curPlayer)
    {
        ++turn;
        isUsedItem = false;
        UIManager.uiManager.ChangeUI(curPlayer);

        if (curPlayer == 0 && playerItemCount[0] == 0)
        {
            if (isOnline)
            {
                if (itemsCount[0] == 0) OnlineItems(0);
                else --itemsCount[0];
            }
            else
            {
                if (itemsCount[0] == 0) CreateItems(0);
                else --itemsCount[0];
            }
        }
        else if (curPlayer == 1 && playerItemCount[1] == 0)
        {
            if (isOnline)
            {
                if (itemsCount[1] == 0) OnlineItems(1);
                else --itemsCount[1];
            }
            else
            {
                if (itemsCount[1] == 0) CreateItems(1);
                else --itemsCount[1];
            }
        }

        if (playersCount[0] + playersCount[1] >= 36)
        {
            Debug.Log("무승부");
            isPlaying = false;
            UIManager.uiManager.GameEnd("-Draw-");
            AudioManager.audioManager.PlaySFX(AudioManager.SFX.Win);
            AudioManager.audioManager.PlayBGM(false);
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
            Debug.Log("플레이어" + curPlayer + " 승!");
            if (isOnline)
            {
                string winner = NetworkManager.networkManager.GetWinnerName(curPlayer);
                UIManager.uiManager.GameEnd(winner);
            }
            else
            {
                if (curPlayer == 0) UIManager.uiManager.GameEnd(UIManager.uiManager.GetPlayerName() + " Win!");
                else UIManager.uiManager.GameEnd("Player2 Win!");
            }
        }
    }

    void OnlineItems(int player)
    {
        if (isP2) return;

        int[] ranTypes = new int[3];
        for (int i = 0; i < 3; ++i)
        {
            int type = RandomItemType(UnityEngine.Random.Range(0, 10));
            ranTypes[i] = type;
        }
        PV.RPC("OnlineCreateItems", RpcTarget.All, ranTypes, player);
    }

    [PunRPC]
    public void OnlineCreateItems(int[] ranTypes, int player)
    {
        Item[] items;

        if (player == 0)
        {
            items = player1_Items;
            playerItemCount[0] = 3;
            itemsCount[0] = 2;
        }
        else
        {
            items = player2_Items;
            playerItemCount[1] = 3;
            itemsCount[1] = 2;
        }

        for (int i = 0; i < 3; ++i)
        {
            items[i].itemType = (Item.Type)ranTypes[i];
            items[i].isUsed = false;
        }

        if (isSecondCreate) UIManager.uiManager.ShowItems(player);
        else if (player == 1)
        {
            UIManager.uiManager.ShowItems(player);
            isSecondCreate = true;
        }
    }

    public void CreateItems(int player)
    {
        Item[] items;

        if (player == 0)
        {
            items = player1_Items;
            playerItemCount[0] = 3;
            itemsCount[0] = 2;
        }
        else
        {
            items = player2_Items;
            playerItemCount[1] = 3;
            itemsCount[1] = 2;
        }

        for (int i = 0; i < 3; ++i)
        {
            int ranType = RandomItemType(UnityEngine.Random.Range(0, 10));

            items[i].itemType = (Item.Type)ranType;
            items[i].isUsed = false;
        }

        if (isSecondCreate) UIManager.uiManager.ShowItems(player);
        else if (player == 1)
        {
            UIManager.uiManager.ShowItems(player);
            isSecondCreate = true;
        }
    }
    
    void BackItem(int player)
    {

    }

    int RandomItemType(int type)
    {
        int ranType = type;
        switch (ranType)
        {
            case 0:
            case 1:
                ranType = 0; // Hammer
                break;
            case 2:
            case 3:
            case 4:
                ranType = 1; // HandGun
                break;
            case 5:
            case 6:
            case 7:
                ranType = 2; // ShotGun
                break;
            case 8:
            case 9:
                ranType = 3; // WildCard
                break;
        }
        return ranType;
    }

    public void UseItem(int index)
    {
        if (isOnline && ((!isP2 && turn % 2 == 1) || (isP2 && turn % 2 == 0))) return;

        if (isUsedItem || (turn % 2 == 1 && isAIBattle)) return;
        if (turn % 2 == 0 && player1_Items[index].isUsed) return;
        else if (turn % 2 == 1 && player2_Items[index].isUsed) return;

        if (isOnline) PV.RPC("UseItemSub", RpcTarget.All, index);
        else UseItemSub(index);
    }

    [PunRPC]
    public void UseItemSub(int index)
    {
        if (isUsingItem && curItemIndex != index)
        {
            isUsingItem = !isUsingItem;
            UIManager.uiManager.UsingItem(curItemIndex, isUsingItem);
        }
        isUsingItem = !isUsingItem;
        curItemIndex = index;
        UIManager.uiManager.UsingItem(curItemIndex, isUsingItem);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Item);


        if (isOnline) PV.RPC("ClearFieldDecal", RpcTarget.All);
        else ClearFieldDecal();
    }

    [PunRPC]
    public void UsedItem(int curPlayer, int index)
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
        UIManager.uiManager.UsedItem(index);
    }

    [PunRPC]
    public void OnAbility(int curPlayer, int index)
    {
        if (curPlayer == 0) player1_Items[curItemIndex].OnAbility(index);
        else player2_Items[curItemIndex].OnAbility(index);
    }

    public void DestroyCircle(int index)
    {
        Floor floor = floors[index];
        int curPlayer = floor.UnSetCircle();
        if (curPlayer == 0) player1_Arr[index] = 0;
        else player2_Arr[index] = 0;
        field[index] = 0;
        --playersCount[curPlayer];
    }

    public void ChangeCircle(int index)
    {
        Floor floor = floors[index];
        int curPlayer = turn % 2;
        floor.SetCircle(curPlayer);
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

    void SetBackState()
    {
        --turn;
        field = backs.Peek().m_field;
        player1_Items[0].itemType = (Item.Type)backs.Peek().m_P1Item_1;
        player1_Items[0].isUsed = backs.Peek().m_P1ItemUsed_1;
        player1_Items[1].itemType = (Item.Type)backs.Peek().m_P1Item_2;
        player1_Items[1].isUsed = backs.Peek().m_P1ItemUsed_2;
        player1_Items[2].itemType = (Item.Type)backs.Peek().m_P1Item_3;
        player1_Items[2].isUsed = backs.Peek().m_P1ItemUsed_3;
        player2_Items[0].itemType = (Item.Type)backs.Peek().m_P2Item_1;
        player2_Items[0].isUsed = backs.Peek().m_P2ItemUsed_1;
        player2_Items[1].itemType = (Item.Type)backs.Peek().m_P2Item_2;
        player2_Items[1].isUsed = backs.Peek().m_P2ItemUsed_2;
        player2_Items[2].itemType = (Item.Type)backs.Peek().m_P2Item_3;
        player2_Items[2].isUsed = backs.Peek().m_P2ItemUsed_3;

        SetBackView();
    }

    void SetBackView()
    {
        for (int i = 0; i < 36; ++i)
        {
            if (field[i] != 0)
            {
                if ((field[i] == 1 && player2_Arr[i] == 1) ||
                    (field[i] == 2 && player1_Arr[i] == 1))
                {
                    ChangeCircle(i);
                }
                else if (field[i] == 1 && player1_Arr[i] == 0)
                {
                    CreateCircle(i, 0);
                }
                else if (field[i] == 2 && player2_Arr[i] == 0)
                {
                    CreateCircle(i, 1);
                }
            }
            else DestroyCircle(i);
        }

        backs.Pop();
        UIManager.uiManager.ChangeUI((turn - 1) % 2);
    }

    public void Backsies(bool isBacksies)
    {
        if (!isOnline)
        {
            isBacksiesing = isBacksies;
            if (isBacksies)
            {
                if (isAIBattle)
                {
                    SetBackState();
                    SetBackState();
                }
                else SetBackState();
            }
            UIManager.uiManager.EndBacksies();
            isBacksiesing = false;
        }
        else PV.RPC("OnlineBacksies", RpcTarget.All, isBacksies);
    }

    [PunRPC]
    public void OnlineBacksies(bool isBacksies)
    {
        isBacksiesing = isBacksies;
        if (isBacksies)
        {
            SetBackState();
        }
        UIManager.uiManager.EndBacksies();
        isBacksiesing = false;
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
                    Floor floor = hit.transform.GetComponent<Floor>();
                    index = floor.floorIndex;
                }
                else if (hit.transform.tag == "Circle")
                {
                    Floor floor = hit.transform.GetComponentInParent<Floor>();
                    index = floor.floorIndex;
                }

                if (isOnline) PV.RPC("FillDecalFieled", RpcTarget.All, index);
                else FillDecalFieled(index);
            }
        }
    }

    [PunRPC]
    public void FillDecalFieled(int index)
    {
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

    void HammerDecal(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            MeshRenderer fieldColor;
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0)
            {
                fieldColor = floors[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = floors[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = floors[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void HandGunDecal()
    {
        for (int i = 0; i < 36; ++i)
        {
            MeshRenderer fieldColor;
            fieldColor = floors[i].GetComponent<MeshRenderer>();
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
                fieldColor = floors[index - 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index > -1 && index < 36)
            {
                fieldColor = floors[index].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5)
            {
                fieldColor = floors[index + 1].GetComponent<MeshRenderer>();
                fieldColor.material.color = Color.blue;
            }
        }
    }

    void WildCardDecal(int location)
    {
        MeshRenderer fieldColor = floors[location].GetComponent<MeshRenderer>(); ;
        fieldColor.material.color = Color.blue;
    }

    [PunRPC]
    public void ClearFieldDecal()
    {
        foreach (Floor floor in floors)
        {
            MeshRenderer fieldColor = floor.GetComponent<MeshRenderer>();
            fieldColor.material.color = Color.white;
        }
    }

    IEnumerator StartDelay()
    {
        yield return startDelay;
        isPlaying = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isUsingItem);
        }
        else
        {
            isUsingItem = (bool)stream.ReceiveNext();
        }
    }
}
