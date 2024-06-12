using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Default = -1, Hammer, Grenade, Shotgun, ReverseCard };
    public Type itemType;
    public GameManager gameManager;
    public bool isUsed = false;
    WaitForSeconds waitScond = new WaitForSeconds(0.5f);

    public void OnAblity(int location)
    {
        switch (itemType)
        {
            case Type.Hammer:
                HammerAblity(location);
                break;
            case Type.Grenade:
                GrenadeAblity(location);
                break;
            case Type.Shotgun:
                ShotgunAblity(location);
                break;
            case Type.ReverseCard:
                ReverseCardAblity(location);
                break;
        }
        isUsed = true;
    }

    void HammerAblity(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            if (index - 1 > -1 && index - 1 < 36 && gameManager.field[index - 1] != 0)
            {
                gameManager.field[index - 1] = 0;
                Destroy(gameManager.fieldObject[index - 1].transform.GetChild(0).gameObject);
            }
            if (index > -1 && index < 36 && gameManager.field[index] != 0)
            {
                gameManager.field[index] = 0;
                Destroy(gameManager.fieldObject[index].transform.GetChild(0).gameObject);
            }

            if (index + 1 > -1 && index + 1 < 36 && gameManager.field[index + 1] != 0)
            {
                gameManager.field[index + 1] = 0;
                Destroy(gameManager.fieldObject[index + 1].transform.GetChild(0).gameObject);
            }
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index - 1));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index + 1));
        }
    }

    void GrenadeAblity(int location)
    {
        for (int i = -2; i < 3; ++i)
        {
            int index = location + (6 * i);
            if (index - 1 > -1 && index - 2 < 36 && gameManager.field[index - 2] != 0)
            {
                gameManager.field[index - 2] = 0;
                Destroy(gameManager.fieldObject[index - 2].transform.GetChild(0).gameObject);
            }
            if (index - 1 > -1 && index - 1 < 36 && gameManager.field[index - 1] != 0)
            {
                gameManager.field[index - 1] = 0;
                Destroy(gameManager.fieldObject[index - 1].transform.GetChild(0).gameObject);
            }
            if (index > -1 && index < 36 && gameManager.field[index] != 0)
            {
                gameManager.field[index] = 0;
                Destroy(gameManager.fieldObject[index].transform.GetChild(0).gameObject);
            }
            if (index + 1 > -1 && index + 1 < 36 && gameManager.field[index + 1] != 0)
            {
                gameManager.field[index + 1] = 0;
                Destroy(gameManager.fieldObject[index + 1].transform.GetChild(0).gameObject);
            }
            if (index + 2 > -1 && index + 2 < 36 && gameManager.field[index + 2] != 0)
            {
                gameManager.field[index + 2] = 0;
                Destroy(gameManager.fieldObject[index + 2].transform.GetChild(0).gameObject);
            }
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index - 2));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index - 1));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index + 1));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index + 2));
        }
    }

    void ShotgunAblity(int location)
    {
        int[] indexs = new int[3];
        for (int i = 0; i < 3; ++i)
        {
            int ranV = UnityEngine.Random.Range(-1, 2);
            int ranH = UnityEngine.Random.Range(-1, 2);
            int index = location + (6 * ranV) + ranH;

            if (Array.IndexOf(indexs, index) == -1)
            {
                indexs.SetValue(index, i);
            }
            else
            {
                --i;
                continue;
            }

            if (index > -1 && index < 36 && gameManager.field[index] != 0)
            {
                gameManager.field[index] = 0;
                Destroy(gameManager.fieldObject[index].transform.GetChild(0).gameObject);
            }
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
        }
    }

    void ReverseCardAblity(int location)
    {
        if (gameManager.field[location] != 0)
        {
            gameManager.ChangeCircle(location);
        }
    }

    IEnumerator ChangeFieldColor(int index)
    {
        MeshRenderer fieldColor = gameManager.fieldObject[index].GetComponent<MeshRenderer>();

        fieldColor.material.color = Color.red;
        yield return waitScond;
        fieldColor.material.color = Color.white;
    }
}
