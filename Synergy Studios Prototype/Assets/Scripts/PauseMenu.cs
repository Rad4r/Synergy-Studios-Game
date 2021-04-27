using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static GameManager GM;
    private int currentButton;
    private float timerDelay;

    public SpriteRenderer[] buttons;

    private void Start()
    {
        timerDelay = 1.2f;
        GM = FindObjectOfType<GameManager>();
        InvokeRepeating("ChangeButton", timerDelay, timerDelay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(GM.primary) && gameObject.activeSelf)
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
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
                break;
            case 1:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case 2:
                gameObject.SetActive(false);
                break;
        }
        gameObject.SetActive(false);
    }

    private void ChangeButton()
    {
        if (currentButton < buttons.Length - 1)
            currentButton++;
        else
            currentButton = 0;
    }
}
