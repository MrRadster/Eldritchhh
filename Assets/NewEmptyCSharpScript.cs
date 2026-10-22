using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void Retry()
    {
        // Very important – unfreeze the game first
        Time.timeScale = 1f;

        // Optional: unlock cursor if you locked it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}