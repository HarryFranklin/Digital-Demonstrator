using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Function for the Play button
    public void OnPlayButtonClicked()
    {
        // Load the GameScene (Scene 1)
        SceneManager.LoadScene("MainScene");
    }

    // Function for the Configure button
    public void OnConfigureButtonClicked()
    {
        // Load the ConfigureScene (Scene 3)
        SceneManager.LoadScene("ConfigureScene");
    }

    // Function for the Back button
    public void OnBackButtonClicked()
    {
        // Load the Title scene
        SceneManager.LoadScene("TitleScene");
    }

    // Function for the Exit button
    public void OnExitButtonClicked()
    {
        // Close the application
        Application.Quit();

        // If running in the editor, stop play mode
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
