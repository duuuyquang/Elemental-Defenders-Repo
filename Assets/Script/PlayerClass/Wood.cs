using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Element
{
    public Wood()
    {
        this.type = TYPE_WOOD;
    }

    public override int GetTypeAdvantage(int typeToCompare)
    {
        int result;
        switch (typeToCompare)
        {
            case TYPE_FIRE:
            case TYPE_SOUL:
                result = TYPE_WEAKER;
                break;
            case TYPE_WATER:
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

