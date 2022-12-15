using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalPoolBullet 
{
    public static List<GameObject> ActiveBullets=new List<GameObject>();
    public static List<GameObject> DisactiveBullets=new List<GameObject>();
    public static void AddActiveBullet(GameObject bullet)
    {
        ActiveBullets.Add(bullet);
        DisactiveBullets.Remove(bullet);
    }
    public static void RemoveActiveBullet(GameObject bullet)
    {
        DisactiveBullets.Add(bullet);
        ActiveBullets.Remove(bullet);
    }
    public static void DisactiveAllActiveBullets()
    {
        foreach (var item in ActiveBullets)
        {
            item.SetActive(false);
            DisactiveBullets.Add(item);
        }
        ActiveBullets.Clear();

    }
}
