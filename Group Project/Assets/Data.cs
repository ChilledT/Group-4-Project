using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data data;

    public bool didWin;
    public int firstYear;
    public int secondYear;
    public int thirdYear;

    private void Awake()
    {
        if (data == null)
        {
            data = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
