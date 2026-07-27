using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelController : MonoBehaviour
{
    // This function will be called when you click the Next Level button.
    public void LoadNextLevel()
    {
        // Get the index of the current scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene    
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next level in the build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // If there is, load the next level
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // If it's the last level, return to the main menu (Scene index 0)
            Debug.Log("Đã hết Level! Quay lại Menu chính.");
            SceneManager.LoadScene(0);
        }
    }
}
