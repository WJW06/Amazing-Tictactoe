using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    public static BattleAI battleAI;

    int index;

    void Awake()
    {
        battleAI = this;
    }
}
