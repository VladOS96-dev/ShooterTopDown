using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    public void StateInit();
    public void StateUpdate();
    public void StateEnd();

}
