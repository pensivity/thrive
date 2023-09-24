using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager_script : MonoBehaviour
{
    [Header("Menu elements")]
    public GameObject mainMenu;
    public GameObject instructions;
    public GameObject introScreen;
    public GameObject creditScreen;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        instructions.SetActive(false);
        introScreen.SetActive(false);
        creditScreen.SetActive(false);
    }

    public void StartIntro ()
    {
        mainMenu.SetActive(false);
        introScreen.SetActive(true);
    }

    public void StartGame ()
    {
        SceneManager.LoadScene("plant stages");
    }

    public void HelpMenu ()
    {
        instructions.SetActive(true);
        mainMenu.SetActive(false);
    }


    public void StopHelping ()
    {
        mainMenu.SetActive(true);
        instructions.SetActive(false);
    }

    public void ReturnToStart ()
    {
        SceneManager.LoadScene("menu screens");
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        creditScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
