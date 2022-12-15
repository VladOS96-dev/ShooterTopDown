using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public  static class GlobalEventsUI 
{
    public static Action<TypeCharacter> OnHit;

    public static void InvokeOnHit(TypeCharacter typeCharacter)
    {
        OnHit?.Invoke(typeCharacter);
    }
}
