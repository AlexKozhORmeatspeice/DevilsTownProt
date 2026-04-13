using System.Collections.Generic;
using UnityEngine;

public class UsableLogicFactory
{
    private static UsableLogicFactory instance;
    public static UsableLogicFactory Instance 
    { 
        get
        {
            if(instance == null)
            {
                instance = new();
            }

            return instance;
        }
    }

    public IUsableLogic GetLogic(UsableType usableType, ItemData itemData)
    {
        IUsableLogic newLogic = null;

        switch(usableType)
        {
            case UsableType.Placeble:
                newLogic = new PlacebleLogic();
                break;
            case UsableType.Kill:
                newLogic = new KillLogic();
                break;
        }

        if(newLogic == null)
        {
            return null;
        }

        newLogic.Init(itemData);

        return newLogic;
    }
}
