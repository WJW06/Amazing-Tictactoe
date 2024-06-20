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
                HammerAbility(location);
                break;
            case Type.Grenade:
                GrenadeAbility(location);
                break;
            case Type.Shotgun:
                ShotgunAbility(location);
                break;
            case Type.ReverseCard:
                ReverseCardAbility(location);
                break;
        }
    }

    void HammerAbility(int location)
    {
        for (int i = -1; i < 2; ++i)
        {
            int index = location + (6 * i);
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0 && gameManager.field[index - 1] != 0)
            {
                gameManager.DestroyCircle(index - 1);
            }
            if (index > -1 && index < 36 && gameManager.field[index] != 0)
            {
                gameManager.DestroyCircle(index);
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5 && gameManager.field[index + 1] != 0)
            {
                gameManager.DestroyCircle(index + 1);
            }
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0) StartCoroutine(ChangeFieldColor(index - 1));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5) StartCoroutine(ChangeFieldColor(index + 1));
        }
    }

    void GrenadeAbility(int location)
    {
        for (int i = -2; i < 3; ++i)
        {
            int index = location + (6 * i);
            if (index - 1 > -1 && index - 2 < 34 && index % 6 > 1 && gameManager.field[index - 2] != 0)
            {
                gameManager.DestroyCircle(index - 2);
            }
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0 && gameManager.field[index - 1] != 0)
            {
                gameManager.DestroyCircle(index - 1);
            }
            if (index > -1 && index < 36 && gameManager.field[index] != 0)
            {
                gameManager.DestroyCircle(index);
            }
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5 && gameManager.field[index + 1] != 0)
            {
                gameManager.DestroyCircle(index + 1);
            }
            if (index + 2 > 1 && index + 2 < 36 && index % 6 < 4 && gameManager.field[index + 2] != 0)
            {
                gameManager.DestroyCircle(index + 2);
            }
            if (index - 2 > -1 && index - 2 < 34 && index % 6 > 1) StartCoroutine(ChangeFieldColor(index - 2));
            if (index - 1 > -1 && index - 1 < 35 && index % 6 > 0) StartCoroutine(ChangeFieldColor(index - 1));
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
            if (index + 1 > 0 && index + 1 < 36 && index % 6 < 5) StartCoroutine(ChangeFieldColor(index + 1));
            if (index + 2 > 1 && index + 2 < 36 && index % 6 < 4) StartCoroutine(ChangeFieldColor(index + 2));
        }
    }

    void ShotgunAbility(int location)
    {
        int[] indexs = new int[3];
        for (int i = 0; i < 3; ++i)
        {
            int ranV, ranH;

            if (location < 6) ranV = UnityEngine.Random.Range(0, 2);
            else if (location > 29) ranV = UnityEngine.Random.Range(-1, 1);
            else ranV = UnityEngine.Random.Range(-1, 2);

            if (location % 6 < 1) ranH = UnityEngine.Random.Range(0, 2);
            else if (location % 6 > 4) ranH = UnityEngine.Random.Range(-1, 1);
            else ranH = UnityEngine.Random.Range(-1, 2);

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
                gameManager.DestroyCircle(index);
            }
            if (index > -1 && index < 36) StartCoroutine(ChangeFieldColor(index));
        }
    }

    void ReverseCardAbility(int location)
    {
        if (gameManager.field[location] != 0)
        {
            gameManager.ChangeCircle(location);
        }
    }

    IEnumerator ChangeFieldColor(int index)
    {
        yield return new WaitForEndOfFrame();
        MeshRenderer fieldColor = gameManager.fieldObjects[index].GetComponent<MeshRenderer>();

        fieldColor.material.color = Color.red;
        yield return waitScond;
        fieldColor.material.color = Color.white;
    }
}
