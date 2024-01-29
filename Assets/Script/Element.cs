using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element
{
    public const int TYPE_FIRE      = 1;
    public const int TYPE_WATER     = 2;
    public const int TYPE_WOOD      = 3;
    public const int TYPE_SOUL      = 4;
    public const int TYPE_HEAVEN    = 5;
    public const int TYPE_MAX       = 5;

    public const int TYPE_STRONGER = 1;
    public const int TYPE_WEAKER = 0;
    public const int TYPE_DRAW = -1;

    protected int type;

    [SerializeField]
    protected float speed;

    public int Type {
        get {
            return type;
        }
        set {
            type = Math.Min(Math.Max(type, 0), TYPE_MAX);
        }
    }

    public void Main(int type)
    {
        this.type = type;
    }

    public abstract int GetTypeAdvantage(int typeToCompare);
}