using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class GlobalEventsGame 
{
    public static Action OnResetPositionCharacter;
    public static void InvokeResetPositionCharacter()
    {
        OnResetPositionCharacter?.Invoke();
    }
}
