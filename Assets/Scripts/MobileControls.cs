using UnityEngine;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    public static bool JumpPressed = false;
    public static bool CrouchHeld = false;

    public void OnJumpDown() => JumpPressed = true;
    public void OnJumpUp() => JumpPressed = false;
    public void OnCrouchDown() => CrouchHeld = true;
    public void OnCrouchUp() => CrouchHeld = false;
}