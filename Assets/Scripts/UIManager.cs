using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playGround;
    public Button start_Button;
    public Button restart_Button;
    public Button[] item_Buttons;
    public Image[] enemyItems;
    public Sprite[] itemSprite;
    public GameManager gameManager;


    public void GameStart()
    {
        playGround.SetActive(true);
        start_Button.gameObject.SetActive(false);
        for (int i = 0; i < 3; ++i)
        {
            item_Buttons[i].gameObject.SetActive(true);
            enemyItems[i].gameObject.SetActive(true);
        }
        ChangeItems(1);
    }

    public void GameEnd()
    {
        restart_Button.gameObject.SetActive(true);
        foreach (Button button in item_Buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void GameRestart()
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");
        foreach (GameObject circle in circles)
        {
            circle.SetActive(false);
        }
        playGround.SetActive(false);
        start_Button.gameObject.SetActive(true);
        restart_Button.gameObject.SetActive(false);
    }

    public void ChangeItems(int curPlayer)
    {
        for (int i = 0; i < 3; ++i)
        {
            Item player = curPlayer == 0 ? gameManager.player2_Items[i] : gameManager.player1_Items[i];
            if (player.isUsed)
            {
                Image buttonImg = item_Buttons[i].GetComponentsInChildren<Image>()[1];
                buttonImg.sprite = null;
            }
            else
            {
                int itemType = (int)player.itemType;
                Image buttonImg = item_Buttons[i].GetComponentsInChildren<Image>()[1];
                buttonImg.sprite = itemSprite[itemType];
            }

            Item enemy = curPlayer == 0 ? gameManager.player1_Items[i] : gameManager.player2_Items[i];
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
