using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : Element
{
    public Soul()
    {
        this.type = TYPE_SOUL;
    }

    public override int GetTypeAdvantage(int typeToCompare)
    {
        int result;

        switch (typeToCompare)
        {
            case TYPE_WATER:
            case TYPE_WOOD:
            case TYPE_FIRE:
                result = TYPE_STRONGER;
                break;
            case TYPE_HEAVEN:
                result = TYPE_WEAKER;
                break;
            default:
                result = TYPE_DRAW;
                break;
        }

        return result;
    }
}

