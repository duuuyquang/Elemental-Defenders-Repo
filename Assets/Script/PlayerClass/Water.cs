using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Element
{
    public Water()
    {
        this.type = TYPE_WATER;
    }

    public override int GetTypeAdvantage(int typeToCompare)
    {
        int result;
        switch (typeToCompare)
        {
            case TYPE_WOOD:
            case TYPE_SOUL:
                result = TYPE_WEAKER;
                break;
            case TYPE_FIRE:
            case TYPE_HEAVEN:
                result = TYPE_STRONGER;
                break;
            default:
                result = TYPE_DRAW;
                break;

        }

        return result;
    }
}
