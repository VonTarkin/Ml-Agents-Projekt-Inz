using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPositionData : MonoBehaviour
{ 
    public string name;
    public int checkPointIndex;

    public CarPositionData(string name, int checkPointIndex)
    {
        this.name = name;
        this.checkPointIndex = checkPointIndex;
    }
}
