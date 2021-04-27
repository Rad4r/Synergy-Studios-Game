using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private static GameManager GM;
    private int currentButton;
    private float timerDelay;

    public SpriteRenderer[] buttons;

    private void Start()
    {
        timerDelay = 2f;
        GM = FindObjectOfType<GameManager>();
        InvokeRepeating("ChangeButton", timerDelay, timerDelay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(GM.primary) && GM.ControlSetup == false)
            ButtonSelect();

        UpdateUI();
    }

    private void UpdateUI()
    {
        buttons[currentButton].color = Color.green;

        foreach (var button in buttons)
        {
            if(button != buttons[currentButton])
                button.color = Color.white;
        }
    }

    private void ButtonSelect()
    {
        switch (currentButton)
        {
            case 0: 
                SceneManager.LoadScene("ArtisticStackGame", LoadSceneMode.Single);
                break;
            case 1:
                SceneManager.LoadScene("Freecell", LoadSceneMode.Single);
                break;
        }
    }

    private void ChangeButton()
    {
        if (currentButton >= 1)
            currentButton--;
        else
            currentButton++;
    }
}
