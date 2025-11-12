using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MobileBootstrap : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // NEW INPUT SYSTEM (works on Android back button)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ExitGame();
        }
#else
        // LEGACY INPUT fallback (Editor or old projects)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
#endif
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // stop Play Mode
#else
        Application.Quit();                                // quit on device
#endif
    }
}
