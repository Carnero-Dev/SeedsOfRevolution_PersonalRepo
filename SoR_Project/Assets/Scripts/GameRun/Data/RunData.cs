using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RunData {
    public int seed = 0;
    [HideInInspector] public SeedState seedState = new();
    public int randomNum;
}