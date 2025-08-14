using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// For setting the ScreenAspectRatio to 16:9
/// Cause i couldn't be bothered to make the gui scaling dynamic
/// </summary>

public class AspectRatioCamera : MonoBehaviour
{
    // Input action for toggling fullscreen when pressing f11
    private InputAction toggle = new InputAction(
            type: InputActionType.Button,
            binding: "<Keyboard>/f11"
        );

    private void Awake()
    {
        toggle.performed += ctx => ToggleFullscreen();
    }

    // Enable/disabble the toggle input alongside the script
    private void OnEnable()
    {
        toggle.Enable();
    }

    private void OnDisable()
    {
        toggle.Disable();
    }

    // Toggle between FullScreenWindow mode and windowed mode
    private void ToggleFullscreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void Update()
    {
        WindowLock();
    }

    // Set the window screen to 16:9 aspect ratio
    // Forces the resolution to width and height
    // --> TODO: needs testing i never testing this
    void WindowLock()
    {
        int height = Screen.height;
        int width = Mathf.RoundToInt(height * _settings.AspectRatio);

        if (Screen.width != width)
        {
            Screen.SetResolution(width, height, Screen.fullScreenMode == FullScreenMode.FullScreenWindow);
        }
    }
}