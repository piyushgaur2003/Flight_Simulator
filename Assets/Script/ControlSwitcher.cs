using UnityEngine;
using UnityEngine.UI;

public class ControlSwitcher : MonoBehaviour
{
    public Button keyboardButton;
    public Button mouseKeyboardButton;

    private void Start()
    {
        // Assign button listeners
        keyboardButton.onClick.AddListener(SetKeyboardControl);
        mouseKeyboardButton.onClick.AddListener(SetMouseKeyboardControl);
    }

    public void SetKeyboardControl()
    {
        MFlight.Demo.Plane.SetControlMode(false); // Keyboard only
        Debug.Log("Switched to Keyboard Control");
    }

    public void SetMouseKeyboardControl()
    {
        MFlight.Demo.Plane.SetControlMode(true); // Mouse + Keyboard
        Debug.Log("Switched to Mouse + Keyboard Control");
    }
}
