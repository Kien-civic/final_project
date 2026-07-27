using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Tang2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Tang3");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("Tang4");
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene("Tang5");
    }

    public void LoadLevel6()
    {
        SceneManager.LoadScene("Tang6");
    }

    public void LoadLevel7()
    {
        SceneManager.LoadScene("Tang7");
    }

    public void LoadLevel8()
    {
        SceneManager.LoadScene("Tang8");
    }

    public void LoadLevel9()
    {
        SceneManager.LoadScene("Tang9");
    }

    public void BackToMainMenu()
    {
        // Restore normal running time to prevent the game from being paused.
        Time.timeScale = 1f;

        // Load the MainMenu scene. You can either use the scene index (usually 0) or the exact name of the scene.
        SceneManager.LoadScene("MainMenu");

        // Or if your MainMenu is the first scene in the Build Settings:
        // SceneManager.LoadScene(0);
    }
}
