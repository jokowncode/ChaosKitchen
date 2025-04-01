
using System;
using UnityEngine;

public class CutOperation : NextStatusCookOp {

    [Header("Slice")] 
    [field : SerializeField] public int CutCount { get; private set; } = 3;
    
    public override CookOP GetCookOP() {
        return CookOP.Cut;
    }
}