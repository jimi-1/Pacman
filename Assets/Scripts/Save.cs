using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Save{

    public List<int> PacdotsPositions = new List<int>();
    public double x;
    public double y;
    public List<double> PacdotsX = new List<double>();
    public List<double> PacdotsY = new List<double>();
    public int score = 0;
}
