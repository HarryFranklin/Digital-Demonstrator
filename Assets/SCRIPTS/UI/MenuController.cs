using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    [Header("Gameplay UI Elements")]
    [SerializeField] private GameObject attacksPanel;
    [SerializeField] private GameObject moneyUIPanel;
    [SerializeField] private GameObject controlPanelPanel;
    
    // Track pause state
    private bool isPaused = false;
    private float previousTimeScale;

    private void Start()
    {
        // Initialise pause menu as hidden
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        // Initialise gameplay UI elements as visible
        if (attacksPanel != null)
            attacksPanel.SetActive(true);
            
        if (moneyUIPanel != null)
            moneyUIPanel.SetActive(true);

        if (controlPanelPanel != null)
            controlPanelPanel.SetActive(true);
    }

    private void Update()
    {
        // Check for pause key input
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // Function to toggle pause state
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // Function to pause the game
    public void PauseGame()
    {
        // Store current time scale
        previousTimeScale = Time.timeScale;
        
        // Set time scale to 0 (freeze gameplay)
        Time.timeScale = 0f;
        
        // Show pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
            
        // Hide gameplay UI elements - known bug - this doesn't work
        if (attacksPanel != null)
            attacksPanel.SetActive(false);
            
        if (moneyUIPanel != null)
            moneyUIPanel.SetActive(false);
        
        if (controlPanelPanel != null)
            controlPanelPanel.SetActive(false);


        // Update state
        isPaused = true;
        
        // Optional: Show cursor if it was hidden during gameplay
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Function to resume the game
    public void ResumeGame()
    {
        // Restore previous time scale
        Time.timeScale = previousTimeScale;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        // Show gameplay UI elements
        if (attacksPanel != null)
            attacksPanel.SetActive(true);
            
        if (moneyUIPanel != null)
            moneyUIPanel.SetActive(true);

        if (controlPanelPanel != null)
            controlPanelPanel.SetActive(true);
        
        // Update state
        isPaused = false;
    }

    // Function for the Play button
    public void OnPlayButtonClicked()
    {
        // Ensure we reset time scale when changing scenes
        Time.timeScale = 1f;
        
        // Load the GameScene (Scene 1)
        SceneManager.LoadScene("MainScene");
    }

    // Function for the Configure button
    public void OnConfigureButtonClicked()
    {
        // Ensure we reset time scale when changing scenes
        Time.timeScale = 1f;
        
        // Load the ConfigureScene (Scene 3)
        SceneManager.LoadScene("ConfigureScene");
    }

    // Function for the Back button
    public void OnBackButtonClicked()
    {
        // Ensure we reset time scale when changing scenes
        Time.timeScale = 1f;
        
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
    
    // Function to return to main menu from pause menu
    public void OnMainMenuButtonClicked()
    {
        // Ensure we reset time scale when changing scenes
        Time.timeScale = 1f;
        
        // Load the Title scene
        SceneManager.LoadScene("TitleScene");
    }
    
    // Check if game is paused (can be called from other scripts)
    public bool IsPaused()
    {
        return isPaused;
    }
    
    // Public method to toggle gameplay UI visibility
    public void SetGameplayUIVisibility(bool visible)
    {
        if (attacksPanel != null)
            attacksPanel.SetActive(visible);
            
        if (moneyUIPanel != null)
            moneyUIPanel.SetActive(visible);

        if (controlPanelPanel != null)
            controlPanelPanel.SetActive(visible);
    }
}