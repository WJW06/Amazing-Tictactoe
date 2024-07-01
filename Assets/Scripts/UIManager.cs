using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;

    public GameObject playGround;
    public Button battleMode_Button;
    public Button aiMode_Button;
    public Button start_Button;
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
    bool isPlaying;

    void Awake()
    {
        uiManager = this;
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
        battleMode_Button.gameObject.SetActive(true);
        aiMode_Button.gameObject.SetActive(true);
        start_Button.gameObject.SetActive(false);
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

    public void GameStart()
    {
        GameManager.gameManager.InitField();
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
        start_Button.gameObject.SetActive(false);
        battleMode_Button.gameObject.SetActive(false);
        aiMode_Button.gameObject.SetActive(false);
        AudioManager.audioManager.PlaySfx(AudioManager.Sfx.Hammer);
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
        messageBanner.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        playerColor.gameObject.SetActive(false);
        start_Button.gameObject.SetActive(true);
        restart_Button.gameObject.SetActive(false);
        playGround.SetActive(false);
        AudioManager.audioManager.PlaySfx(AudioManager.Sfx.Hammer);
        AudioManager.audioManager.PlayBgm(true);
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
