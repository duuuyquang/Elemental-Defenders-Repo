using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heaven : Element
{
    public Heaven()
    {
        this.type = TYPE_HEAVEN;
    }

    public override int GetTypeAdvantage(int typeToCompare)
    {
        int result;

        switch (typeToCompare)
        {
            case TYPE_WATER:
            case TYPE_WOOD:
            case TYPE_FIRE:
                result = TYPE_WEAKER;
                break;
            case TYPE_SOUL:
                result = TYPE_STRONGER;
                break;
            default:
                result = TYPE_DRAW;
                break;
        }

        return result;
    }
}

