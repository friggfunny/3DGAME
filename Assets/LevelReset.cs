using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script reloads the current active scene when the 'R' key is pressed.
/// This effectively resets the level and transports the player to the starting position.
/// </summary>
public class LevelReset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if the 'R' key is pressed down
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload the currently active scene.
            // This will reset all GameObjects to their initial state.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
