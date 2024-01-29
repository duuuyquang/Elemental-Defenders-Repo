using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Element
{
    public Fire()
    {
        this.type = TYPE_FIRE;
    }

    public override int GetTypeAdvantage(int typeToCompare)
    {
        int result;

        switch(typeToCompare)
        {
            case TYPE_WATER:
            case TYPE_SOUL:
                result = TYPE_WEAKER;
                break;
            case TYPE_WOOD:
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
