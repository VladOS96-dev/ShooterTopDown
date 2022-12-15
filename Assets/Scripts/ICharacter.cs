using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter 
{
    public TypeCharacter typeCharacter { get;  }
    public void Damage();
    public void ResetPosition();
}
