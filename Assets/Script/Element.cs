using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element
{
    public const int TYPE_FIRE  = 1;
    public const int TYPE_WATER = 2;
    public const int TYPE_WOOD  = 3;
    public const int TYPE_MAX   = 3;

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

    public int GetTypeAdvantage(int typeToCompare)
    {
        if(type == typeToCompare)
        {
            return -1;
        }

        switch (type)
        {
            case TYPE_FIRE:
                return typeToCompare == TYPE_WATER ? 0 : 1;
            case TYPE_WATER:
                return typeToCompare == TYPE_WOOD ? 0 : 1;
            case TYPE_WOOD:
                return typeToCompare == TYPE_FIRE ? 0 : 1;
            default:
                return 0;
        }
    }
}