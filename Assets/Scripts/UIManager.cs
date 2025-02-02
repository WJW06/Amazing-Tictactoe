using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;
    public PhotonView PV;

    public GameObject playGround;

    public Text logo;
    public Button first_Button;
    public Button second_Button;
    public Button third_Button;
    public Button fourth_Button;
    public Button fifth_Button;
    int situation = 0;

    public GameObject setting_Interface;
    public Toggle fullScreen_Toggle;
    public Dropdown resolution_Dropdown;
    public Slider bgm_Slider;
    public Slider sfx_Slider;
    public InputField bgm_Input;
    public InputField sfx_Input;
    public float[] temp_Setting;

    public Text name_Text;
    public InputField name_Input;
    string player_Name;
    public Toggle canAIBacksies_Toggle;

    public Image network_Status;
    public bool isConnect = false;

    public Image room_Status;
    public Text title_Text;
    public Text error_Text;
    public Button enter_Button;
    public InputField room_Input;
    bool isCreateRoom;
    bool isMaster;

    public Image room_Base;
    public Text room_Name;
    public Text player1_name;
    public Text player2_name;
    public Button start_Button;
    public Toggle canOnlineBacksise_Toggle;
    bool isOnlineBacksise = false;

    public Button[] item_Buttons;
    public Image[] enemyItems;
    public Sprite[] itemSprite;
    public Button backsies_Button;
    public Image backsies;

    public Text timeText;
    public Image playerColor;
    Color player1_Color = new Color(255, 0, 0);
    Color player2_Color = new Color(0, 100, 255);

    public Image messageBanner;
    public Text messageText;
    public Button home_Button;

    float time;
    float prevSFX;
    bool isPlaying;
    bool isFullScreen = true;
    WaitForSeconds startDelay = new WaitForSeconds(0.2f);


    void Awake()
    {
        uiManager = this;
        temp_Setting = new float[4];
        int isFull = PlayerPrefs.HasKey("FullScreen") ? PlayerPrefs.GetInt("FullScreen") : 1;
        isFullScreen = isFull == 0 ? false : true;
        resolution_Dropdown.onValueChanged.AddListener(SetResolution);
        SetResolution((int)PlayerPrefs.GetFloat("Resolution"));
    }

    void Update()
    {
        if (isPlaying)
        {
            time += Time.deltaTime;
            int min = Mathf.FloorToInt(time / 60);
            int sec = Mathf.FloorToInt(time % 60);
            timeText.text = string.Format("Time: {0:D2}:{1:D2}", min, sec);
        }
    }

    public string GetPlayerName()
    {
        return player_Name;
    }

    void Situation0()
    {
        situation = 0;
        logo.gameObject.SetActive(true);
        first_Button.gameObject.SetActive(true);
        second_Button.gameObject.SetActive(true);
        third_Button.gameObject.SetActive(true);
        fourth_Button.gameObject.SetActive(false);
        name_Text.gameObject.SetActive(false);
        name_Input.gameObject.SetActive(false);
        canAIBacksies_Toggle.gameObject.SetActive(false);

        first_Button.GetComponentInChildren<Text>().text = "Start";
        second_Button.GetComponentInChildren<Text>().text = "Setting";
        third_Button.GetComponentInChildren<Text>().text = "Exit";
    }

    void Situation1()
    {
        situation = 1;
        logo.gameObject.SetActive(false);
        third_Button.gameObject.SetActive(true);
        fourth_Button.gameObject.SetActive(true);
        fifth_Button.gameObject.SetActive (false);
        name_Text.gameObject.SetActive(true);
        name_Input.gameObject.SetActive(true);
        canAIBacksies_Toggle.gameObject.SetActive(true);

        first_Button.GetComponentInChildren<Text>().text = "P1 vs AI";
        second_Button.GetComponentInChildren<Text>().text = "P1 vs P2";
        third_Button.GetComponentInChildren<Text>().text = "Back";
        fourth_Button.GetComponentInChildren<Text>().text = "Join Server";
    }

    void Situation2()
    {
        situation = 2;
        third_Button.gameObject.SetActive(false);
        fifth_Button.gameObject.SetActive(true);
        name_Text.gameObject.SetActive(false);
        name_Input.gameObject.SetActive(false);
        canAIBacksies_Toggle.gameObject.SetActive(false);
        first_Button.GetComponentInChildren<Text>().fontSize = 26;
        second_Button.GetComponentInChildren<Text>().fontSize = 26;

        first_Button.GetComponentInChildren<Text>().text = "Join Room";
        second_Button.GetComponentInChildren<Text>().text = "Disconnect";
        fourth_Button.GetComponentInChildren<Text>().text = "Create Room";
    }

    public void FirstButton()
    {
        switch(situation)
        {
            case 0:
                Situation1();
                break;
            case 1:
                AIMode();
                break;
            case 2:
                JoinRoom();
                break;
        }
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void SecondButton()
    {
        switch (situation)
        {
            case 0:
                SettingButton();
                break;
            case 1:
                BattleMode();
                break;
            case 2:
                Disconnect();
                break;
        }
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void ThirdButton()
    {
        switch (situation)
        {
            case 0:
                ExitButton();
                break;
            case 1:
                Situation0();
                break;
        }
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void FourthButton()
    {
        switch (situation)
        {
            case 1:
                JoinServer();
                break;
            case 2:
                CreateRoom();
                break;
        }
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void FifthButton()
    {
        RandomMatch();
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    void MainButtons(bool b)
    {
        first_Button.gameObject.SetActive(b);
        second_Button.gameObject.SetActive(b);
        third_Button.gameObject.SetActive(b);
    }

    public void SettingButton()
    {
        logo.gameObject.SetActive(false);
        MainButtons(true);

        setting_Interface.SetActive(true);
        temp_Setting[0] = PlayerPrefs.HasKey("FullScreen") ? PlayerPrefs.GetInt("FullScreen") : 1;
        temp_Setting[1] = PlayerPrefs.HasKey("Resolution") ? PlayerPrefs.GetFloat("Resolution") : 0;
        temp_Setting[2] = PlayerPrefs.HasKey("BGM") ? PlayerPrefs.GetFloat("BGM") : 10;
        temp_Setting[3] = PlayerPrefs.HasKey("SFX") ? PlayerPrefs.GetFloat("SFX") : 10;
        fullScreen_Toggle.isOn = temp_Setting[0] == 1;
        resolution_Dropdown.value = (int)temp_Setting[1];
        bgm_Input.text = temp_Setting[2].ToString();
        sfx_Input.text = temp_Setting[3].ToString();
        prevSFX = temp_Setting[3];
    }

    public void SetFullScreen()
    {
        isFullScreen = fullScreen_Toggle.isOn;
        SetResolution(resolution_Dropdown.value);
    }

    public void SetResolution(int value)
    {
        switch (value)
        {
            case 0:
                Screen.SetResolution(1920, 1200, isFullScreen);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
            case 2:
                Screen.SetResolution(1680, 1050, isFullScreen);
                break;
            case 3:
                Screen.SetResolution(1600, 900, isFullScreen);
                break;
            case 4:
                Screen.SetResolution(1400, 1050, isFullScreen);
                break;
            case 5:
                Screen.SetResolution(1280, 1024, isFullScreen);
                break;
            case 6:
                Screen.SetResolution(1280, 768, isFullScreen);
                break;
            case 7:
                Screen.SetResolution(1280, 720, isFullScreen);
                break;
            case 8:
                Screen.SetResolution(1024, 768, isFullScreen);
                break;
            case 9:
                Screen.SetResolution(800, 600, isFullScreen);
                break;
        }
    }

    public void SlideBGM()
    {
        bgm_Slider.value = Mathf.FloorToInt(bgm_Slider.value);
        bgm_Input.text = bgm_Slider.value.ToString();
        AudioManager.audioManager.ChangeBGMVolume(float.Parse(bgm_Input.text) / 10);
    }

    public void InputBGM()
    {
        if (bgm_Input.text == "") bgm_Input.text = "0";
        bgm_Slider.value = float.Parse(bgm_Input.text);
        AudioManager.audioManager.ChangeBGMVolume(float.Parse(bgm_Input.text) / 10);
    }

    public void SlideSFX()
    {
        sfx_Slider.value = Mathf.FloorToInt(sfx_Slider.value);
        sfx_Input.text = sfx_Slider.value.ToString();
        AudioManager.audioManager.ChangeSFXVolume(float.Parse(sfx_Input.text) / 10);
        if (prevSFX != sfx_Slider.value) AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
        prevSFX = sfx_Slider.value;
    }

    public void InputSFX()
    {
        if (sfx_Input.text == "") sfx_Input.text = "0";
        sfx_Slider.value = float.Parse(sfx_Input.text);
        AudioManager.audioManager.ChangeSFXVolume(float.Parse(sfx_Input.text) / 10);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
    }

    public void CancleButton()
    {
        setting_Interface.SetActive(false);
        logo.gameObject.SetActive(true);
        MainButtons(true);

        isFullScreen = temp_Setting[0] == 1 ? true : false;
        SetResolution((int)temp_Setting[1]);
        AudioManager.audioManager.ChangeBGMVolume(temp_Setting[2] / 10);
        AudioManager.audioManager.ChangeSFXVolume(temp_Setting[3] / 10);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void ConfirmButton()
    {
        setting_Interface.SetActive(false);
        logo.gameObject.SetActive(true);
        MainButtons(true);

        PlayerPrefs.SetInt("FullScreen", isFullScreen == true ? 1 : 0);
        PlayerPrefs.SetFloat("Resolution", resolution_Dropdown.value);
        PlayerPrefs.SetFloat("BGM", float.Parse(bgm_Input.text));
        PlayerPrefs.SetFloat("SFX", float.Parse(sfx_Input.text));
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void ExitButton()
    {
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
        Application.Quit();
    }

    bool CheckName()
    {
        Text input = name_Input.placeholder.GetComponent<Text>();
        string name = name_Input.text;

        if (name == "")
        {
            input.fontSize = 24;
            input.text = "Please enter your name!";
            return false;
        }
        else if (name.Length < 2 ||  name.Length > 10)
        {
            name_Input.text = "";
            input.fontSize = 20;
            input.text = "Please specify the number of\ncharacters between 2 and 10!";
            return false;
        }

        player_Name = name;
        return true;
    }

    public void JoinServer()
    {
        if (!CheckName()) return;
        GameManager.gameManager.isAIBattle = false;
        network_Status.gameObject.SetActive(true);
        NetworkManager.networkManager.Connect();
        NetworkManager.networkManager.JoinServer();
        Situation2();
        ActiveButtons(false);
        isMaster = false;
    }

    public void AIMode()
    {
        if (!CheckName()) return;
        GameManager.gameManager.SetCanBacksies(canAIBacksies_Toggle.isOn);
        GameManager.gameManager.isAIBattle= true;
        GameStart();
    }

    public void BattleMode()
    {
        if (!CheckName()) return;
        GameManager.gameManager.SetCanBacksies(true);
        GameManager.gameManager.isAIBattle = false;
        GameStart();
    }

    public void CloseStatus()
    {
        if (!isConnect || !network_Status) return;
        network_Status.gameObject.SetActive(false);
    }

    public void RandomMatch()
    {
        network_Status.gameObject.SetActive(true);
        NetworkManager.networkManager.RandomMatch();
    }

    public void ActiveButtons(bool b)
    {
        first_Button.gameObject.SetActive(b);
        second_Button.gameObject.SetActive(b);
        fourth_Button.gameObject.SetActive(b);
        fifth_Button.gameObject.SetActive(b);
    }

    public void JoinRoom()
    {
        ActiveButtons(false);
        error_Text.gameObject.SetActive(false);
        title_Text.text = "Join Room";
        room_Input.text = "";
        enter_Button.GetComponentInChildren<Text>().text = "Join";
        isCreateRoom = false;
        room_Status.gameObject.SetActive(true);
        canOnlineBacksise_Toggle.gameObject.SetActive(false);
    }

    public void CreateRoom()
    {
        ActiveButtons(false);
        error_Text.gameObject.SetActive(false);
        title_Text.text = "Create Room";
        room_Input.text = "";
        enter_Button.GetComponentInChildren<Text>().text = "Create";
        isCreateRoom = true;
        room_Status.gameObject.SetActive(true);
        canOnlineBacksise_Toggle.gameObject.SetActive(true);
    }

    public void LeaveRoom()
    {
        isMaster = false;
    }

    public void CloseRoom(bool isCancle)
    {
        room_Status.gameObject.SetActive(false);
        if (isCancle) ActiveButtons(true);
    }

    public void EnterButton()
    {
        if (room_Input.text == "")
        {
            error_Text.gameObject.SetActive(true);
            error_Text.text = "<color=red>Error: The value is empty.</color>";
            return;
        }
        else if (room_Input.text.Length > 20)
        {
            error_Text.gameObject.SetActive(true);
            error_Text.text = "<color=red>The name is too long. (20 characters or less)</color>";
            return;
        }

        network_Status.gameObject.SetActive(true);
        if (isCreateRoom)
        {
            isOnlineBacksise = canOnlineBacksise_Toggle.isOn;
            NetworkManager.networkManager.CreateRoom();
        }
        else NetworkManager.networkManager.JoinRoom();
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void CreateRoomFailed()
    {
        error_Text.gameObject.SetActive(true);
        error_Text.text = "<color=red>The name already exists.</color>";
    }

    public void JoinRoomFailed()
    {
        error_Text.gameObject.SetActive(true);
        error_Text.text = "<color=red>The name does not exist.</color>";
    }
    public void ShowRoom(string room_name, string p1_name, string p2_name)
    {
        room_Status.gameObject.SetActive(false);
        room_Base.gameObject.SetActive(true);
        room_Name.text = room_name;
        player1_name.text = p1_name;
        player2_name.text = p2_name;
    }

    public void RoomRenewal(string p1, string p2, bool isP2)
    {
        player1_name.text = p1;
        player2_name.text = p2;

        if (!isP2)
        {
            isMaster = true;
            start_Button.gameObject.SetActive(true);
        }
        else if (!isMaster) start_Button.gameObject.SetActive(false);
        else PV.RPC("SetOnlineBacksies", RpcTarget.All, isOnlineBacksise);
    }

    [PunRPC]
    public void SetOnlineBacksies(bool bOnlineBacksise)
    {
        isOnlineBacksise = bOnlineBacksise;
        print("isOB: " + bOnlineBacksise);
    }

    public void LeaveButton()
    {
        room_Base.gameObject.SetActive(false);
        Situation2();
        NetworkManager.networkManager.LeaveRoom();
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void StartButton()
    {
        if (player2_name.text == "") return;
        PV.RPC("OnlineMode", RpcTarget.All);
        PV.RPC("IsP2", RpcTarget.All);
        PV.RPC("GameStart", RpcTarget.All);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
    }

    public void Disconnect()
    {
        if (!isConnect) return;
        network_Status.gameObject.SetActive(true);
        NetworkManager.networkManager.Disconnect();
        Situation1();
    }

    [PunRPC]
    public void OnlineMode()
    {
        GameManager.gameManager.SetOnline(true);
        GameManager.gameManager.SetCanBacksies(isOnlineBacksise);
    }

    [PunRPC]
    public void IsP2()
    {
        if (!isMaster) GameManager.gameManager.SetP2(true);
        else GameManager.gameManager.SetP2(false);
    }

    [PunRPC]
    public void GameStart()
    {
        CloseRoom(false);
        ActiveButtons(false);
        room_Base.gameObject.SetActive(false);
        third_Button.gameObject.SetActive(false);
        name_Text.gameObject.SetActive(false);
        name_Input.gameObject.SetActive(false);
        canAIBacksies_Toggle.gameObject.SetActive(false);

        GameManager.gameManager.InitField();
        playGround.SetActive(true);
        backsies_Button.gameObject.SetActive(GameManager.gameManager.GetCanBacksies());
        backsies_Button.enabled = false;
        timeText.gameObject.SetActive(true);
        playerColor.gameObject.SetActive(true);
        time = 0;
        isPlaying = true;
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
    }

    public void ShowItems(int curPlayer)
    {
        for (int i = 0; i < 3; ++i)
        {
            SetItemImage(0, i);
            SetItemImage(1, i);
            item_Buttons[i].gameObject.SetActive(true);
            enemyItems[i].gameObject.SetActive(true);
        }
        
        ChangeUI(curPlayer);
    }

    public void BacksiesButton()
    {
        if (GameManager.gameManager.GetOnline() &&
          ((GameManager.gameManager.turn % 2 == 0 && isMaster) ||
           (GameManager.gameManager.turn % 2 == 1 && !isMaster))) return;

        if (!GameManager.gameManager.GetOnline())
        {
            backsies.gameObject.SetActive(true);
            backsies_Button.enabled = false;
        }
        else PV.RPC("OnlineBacksiesButton", RpcTarget.All);
    }

    [PunRPC]
    public void OnlineBacksiesButton()
    {
        backsies.gameObject.SetActive(true);
        backsies_Button.enabled = false;
    }

    public void YesButton()
    {
        if (GameManager.gameManager.GetOnline() &&
          ((GameManager.gameManager.turn % 2 == 0 && isMaster) ||
           (GameManager.gameManager.turn % 2 == 1 && !isMaster))) return;
        GameManager.gameManager.Backsies(true);
    }

    public void NoButton()
    {
        if (GameManager.gameManager.GetOnline() &&
          ((GameManager.gameManager.turn % 2 == 0 && isMaster) ||
           (GameManager.gameManager.turn % 2 == 1 && !isMaster))) return;
        GameManager.gameManager.Backsies(false);
    }

    public void EndBacksies()
    {
        backsies.gameObject.SetActive(false);
        if (GameManager.gameManager.turn == 0) backsies_Button.enabled = false;
        else backsies_Button.enabled = true;
    }

    public void SetItemImage(int curPlayer, int index)
    {
        Image buttonImg;
        Item player = curPlayer == 0 ? GameManager.gameManager.player1_Items[index] : GameManager.gameManager.player2_Items[index];
        int itemType = (int)player.itemType;

        if (curPlayer == 0) buttonImg = item_Buttons[index].GetComponentsInChildren<Image>()[1];
        else buttonImg = enemyItems[index].GetComponentsInChildren<Image>()[1];

        buttonImg.sprite = itemSprite[itemType];
    }

    public void ChangeUI(int curPlayer)
    {
        backsies_Button.enabled = true;

        if (curPlayer == 0) playerColor.color = player1_Color;
        else playerColor.color = player2_Color;

        for (int i = 0; i < 3; ++i)
        {
            Item player = curPlayer == 0 ? GameManager.gameManager.player2_Items[i] : GameManager.gameManager.player1_Items[i];
            if (player.isUsed)
            {
                Image item_Image = item_Buttons[i].GetComponentsInChildren<Image>()[1];
                item_Image.sprite = null;
            }
            else
            {
                int itemType = (int)player.itemType;
                Image buttonImg = item_Buttons[i].GetComponentsInChildren<Image>()[1];
                buttonImg.sprite = itemSprite[itemType];
            }
            Item enemy = curPlayer == 0 ? GameManager.gameManager.player1_Items[i] : GameManager.gameManager.player2_Items[i];
            if (enemy.isUsed)
            {
                Image buttonImg = enemyItems[i].GetComponentsInChildren<Image>()[1];
                buttonImg.sprite = null;
            }
            else
            {
                int itemType = (int)enemy.itemType;
                Image buttonImg = enemyItems[i].GetComponentsInChildren<Image>()[1];
                buttonImg.sprite = itemSprite[itemType];
            }
        }
    }

    public void UsingItem(int index, bool isUsing)
    {
        if (GameManager.gameManager.GetOnline())
        {
            if (isUsing)
            {
                item_Buttons[index].image.color = Color.yellow;
            }
            else
            {
                item_Buttons[index].image.color = Color.white;
            }
        }
        else
        {
            if (isUsing)
            {
                item_Buttons[index].image.color = Color.yellow;
            }
            else
            {
                item_Buttons[index].image.color = Color.white;
            }
        }
    }

    public void UsedItem(int index)
    {
        Image item_Image = item_Buttons[index].GetComponentsInChildren<Image>()[1];
        item_Image.sprite = null;
    }

    public void WinnerBanner(string winner)
    {
        messageText.text = winner;
    }

    public void GameEnd(string winner)
    {
        home_Button.gameObject.SetActive(true);
        messageBanner.gameObject.SetActive(true);

        backsies_Button.gameObject.SetActive(false);
        backsies.gameObject.SetActive(false);
        foreach (Button button in item_Buttons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Image enemyItem in enemyItems)
        {
            enemyItem.gameObject.SetActive(false);
        }

        WinnerBanner(winner);
        isPlaying = false;
        GameManager.gameManager.SetPlaying(isPlaying);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Win);
        AudioManager.audioManager.PlayBGM(false);
    }

    public void HomeButton()
    {
        if (GameManager.gameManager.GetOnline())
        {
            NetworkManager.networkManager.OnDisconnected(DisconnectCause.SendException);
        }
        GameManager.gameManager.ClearField();
        timeText.gameObject.SetActive(false);
        home_Button.gameObject.SetActive(false);
        playGround.SetActive(false);
        playerColor.gameObject.SetActive(false);
        messageBanner.gameObject.SetActive(false);
        Situation0();
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
        AudioManager.audioManager.PlayBGM(true);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Button);
        if (GameManager.gameManager.GetOnline())
        {
            GameManager.gameManager.SetOnline(false);
            PhotonNetwork.Disconnect();
        }
    }
}
