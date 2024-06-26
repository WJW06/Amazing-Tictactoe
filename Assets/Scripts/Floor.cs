using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Circle circle;
    public int floorIndex;
    bool isSet = false;

    public void SetCircle(int curPlayer)
    {
        circle.gameObject.SetActive(true);
        circle.SetCircleType((Circle.CircleType)curPlayer);
        isSet = true;
    }

    public int UnSetCircle()
    {
        circle.gameObject.SetActive(false);
        isSet = false;
        return (int)circle.circleType;
    }
}
