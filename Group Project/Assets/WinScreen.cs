using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI win;
    public TextMeshProUGUI first;
    public TextMeshProUGUI second;
    public TextMeshProUGUI third;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(0);
        }

        win.text = Data.data.didWin ? "YOU WON" : "YOU LOST";

        switch(Data.data.firstYear)
        {
            case -1:
                first.text = "";
                break;
            case 0:
                first.text = "In first year you got a pass!";
                break;
            case 1:
                first.text = "In first year you got a 2:2!";
                break;
            case 2:
                first.text = "In first year you got a 2:1!";
                break;
            case 3:
                first.text = "In first year you got a 1st!";
                break;
        }

        switch (Data.data.secondYear)
        {
            case -1:
                second.text = "";
                break;
            case 0:
                second.text = "In second year you got a pass!";
                break;
            case 1:
                second.text = "In second year you got a 2:2!";
                break;
            case 2:
                second.text = "In second year you got a 2:1!";
                break;
            case 3:
                second.text = "In second year you got a 1st!";
                break;
        }

        switch (Data.data.thirdYear)
        {
            case -1:
                third.text = "";
                break;
            case 0:
                third.text = "In third year you got a pass!";
                break;
            case 1:
                third.text = "In third year you got a 2:2!";
                break;
            case 2:
                third.text = "In third year you got a 2:1!";
                break;
            case 3:
                third.text = "In third year you got a 1st!";
                break;
        }
    }
}
