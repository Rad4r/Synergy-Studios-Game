using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingObjectsUI : MonoBehaviour
{
    public GameObject utensils;
    private int remaining;

    private List<GameObject> myForkList;
    

    public void UIRemainsSetup()
    {
        remaining = 30;

        if (myForkList != null && myForkList.Count > 0)
        {
            for (int i = 0; i < myForkList.Count; i++)
            {
                Destroy(myForkList[i]);
            }
        }
            
        myForkList = new List<GameObject>();
        
        for (int i = 0; i < remaining; i++)
        {
            myForkList.Add(Instantiate(utensils, transform.position  + Vector3.left * i * .2f, Quaternion.identity));
        }
    }

    public void UpdateUI()
    {
        if (myForkList.Count > 0)
        {
            Destroy(myForkList[remaining - 1]);
            remaining--;
        }
    }
}
