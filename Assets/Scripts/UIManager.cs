using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;

    public GameObject playGround;
    public Button start_Button;
    public Button setting_Button;
    public GameObject setting_Interface;
    public Toggle fullScreen_Toggle;
    public Dropdown resolution_Dropdown;
    public Slider bgm_Slider;
    public Slider sfx_Slider;
    public InputField bgm_Input;
    public InputField sfx_Input;
    public float[] temp_Setting;
    public Button exit_Button;
    public Button battleMode_Button;
    public Button aiMode_Button;
    public Button back_Button;
    public Button restart_Button;
    public Button[] item_Buttons;
    public Image[] enemyItems;
    public Sprite[] itemSprite;
    public Image messageBanner;
    public Text messageText;
    public Text timeText;
    public Image playerColor;
    Color player1_Color = new Color(255, 0, 0);
    Color player2_Color = new Color(0, 100, 255);
    float time;
    float prevSFX;
    bool isPlaying;
    bool isFullScreen = true;

    void Awake()
    {
        uiManager = this;
        temp_Setting = new float[4];
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

    public void StartButton()
    {
        MainButton(false);
        SubButton(true);
    }

    public void SettingButton()
    {
        MainButton(false);
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
                Screen.SetResolution(1280, 720, isFullScreen);
                break;
            case 7:
                Screen.SetResolution(1024, 768, isFullScreen);
                break;
            case 8:
                Screen.SetResolution(800, 600, isFullScreen);
                break;
            case 9:
                Screen.SetResolution(640, 480, isFullScreen);
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
        MainButton(true);
        isFullScreen = temp_Setting[0] == 1 ? true : false;
        SetResolution((int)temp_Setting[1]);
        AudioManager.audioManager.ChangeBGMVolume(temp_Setting[2] / 10);
        AudioManager.audioManager.ChangeSFXVolume(temp_Setting[3] / 10);
    }

    public void ConfirmButton()
    {
        setting_Interface.SetActive(false);
        MainButton(true);
        PlayerPrefs.SetInt("FullScreen", isFullScreen == true ? 1 : 0);
        PlayerPrefs.SetFloat("Resolution", resolution_Dropdown.value);
        PlayerPrefs.SetFloat("BGM", float.Parse(bgm_Input.text));
        PlayerPrefs.SetFloat("SFX", float.Parse(sfx_Input.text));
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    void MainButton(bool active)
    {
        start_Button.gameObject.SetActive(active);
        setting_Button.gameObject.SetActive(active);
        exit_Button.gameObject.SetActive(active);
    }

    public void AIMode()
    {
        GameManager.gameManager.isPlayerBattle = false;
        GameStart();
    }

    public void BattleMode()
    {
        GameManager.gameManager.isPlayerBattle = true;
        GameStart();
    }

    public void BackButton()
    {
        SubButton(false);
        MainButton(true);
    }

    void SubButton(bool active)
    {
        battleMode_Button.gameObject.SetActive(active);
        aiMode_Button.gameObject.SetActive(active);
        back_Button.gameObject.SetActive(active);
    }

    public void GameStart()
    {
        GameManager.gameManager.InitField();
        SubButton(false);
        playGround.SetActive(true);
        timeText.gameObject.SetActive(true);
        playerColor.gameObject.SetActive(true);
        for (int i = 0; i < 3; ++i)
        {
            SetItemImage(0, i);
            SetItemImage(1, i);
            item_Buttons[i].gameObject.SetActive(true);
            enemyItems[i].gameObject.SetActive(true);
        }
        ChangeUI(1);
        time = 0;
        isPlaying = true;
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
    }

    public void GameEnd(int winner)
    {
        restart_Button.gameObject.SetActive(true);
        foreach (Button button in item_Buttons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Image enemyItem in enemyItems)
        {
            enemyItem.gameObject.SetActive(false);
        }
        messageBanner.gameObject.SetActive(true);
        string winPlayer;
        if (winner == 0) winPlayer = "Player1 Win!";
        else winPlayer = "Player2 Win!";
        messageText.text = winPlayer;
        isPlaying = false;
    }

    public void GameRestart()
    {
        GameManager.gameManager.ClearField();
        timeText.gameObject.SetActive(false);
        restart_Button.gameObject.SetActive(false);
        playGround.SetActive(false);
        playerColor.gameObject.SetActive(false);
        MainButton(true);
        messageBanner.gameObject.SetActive(false);
        AudioManager.audioManager.PlaySFX(AudioManager.SFX.Hammer);
        AudioManager.audioManager.PlayBGM(true);
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
        if (isUsing)
        {
            item_Buttons[index].image.color = Color.yellow;
        }
        else
        {
            item_Buttons[index].image.color = Color.white;
        }
    }

    public void UsedItem(int index)
    {
        Image item_Image = item_Buttons[index].GetComponentsInChildren<Image>()[1];
        item_Image.sprite = null;
    }
}
