using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// InputHandler is a very simple camera controller. It listens for mouse clicks and rotates the camera around the origin
// when you click and drag. Using the mouse wheel also allows you to zoom in and out, within limits.
//

public class InputHandler : MonoBehaviour
{
    private bool m_IgnoreInput = false;
    public bool IgnoreInput { set { m_IgnoreInput = value; } }

    float m_CameraAngle;
    float m_CameraElevation = 0.2f;
    float m_CameraDistance = 0.5f;
    public float m_CameraDistanceMin;
    public float m_CameraDistanceMax;
    public Vector3 m_CameraCenter;

    bool m_IsDragging;
    Vector3 m_LastMousePosition;

    void Update()
    {
        // We listen to the mouse scroll wheel and apply changes to m_CameraDistance, which is the distance that the camera
        // will be from the origin.

        m_CameraDistance += Input.mouseScrollDelta.y * -0.1f;
        m_CameraDistance = Mathf.Clamp01(m_CameraDistance);

        // Check input to decide if the player is dragging the mouse or not.
        // We think of the player as dragging the camera any time that they aren't dragging the eye.
        // ...and the eye tells if it thinks it's being dragged. So, if the mouse is down and the
        // eye hasn't told us to stop, let's drag the camera!

        // But don't store our current dragging state in m_IsDragging just yet. Use a local variable instead.
        bool is_dragging = Input.GetMouseButton(0) && !m_IgnoreInput;

        if(is_dragging && !m_IsDragging)
        {
            // If we get here it's because we are now dragging, but we weren't on the previous frame.
            // (because m_IsDragging is still false. This means that we just started dragging and our
            // m_LastMousePosition won't be valid. So we snap it to the current mouse position.
            m_LastMousePosition = Input.mousePosition;
        }

        // Okay, now m_IsDragging can be the real value. =)
        m_IsDragging = is_dragging;

        // So, are we dragging the camera?
        if(m_IsDragging)
        {
            // If we are, then the distance we dragged the mouse is applied to the camera's angle
            // and elevation.

            Vector3 movement = Input.mousePosition - m_LastMousePosition;
            m_LastMousePosition = Input.mousePosition;

            m_CameraAngle     += movement.x / Screen.width * 2.0f;
            m_CameraElevation -= movement.y / Screen.height * 2.0f;
        }

        // Elevation needs to be clamped so you can't take the camera through the floor, or flip it upside down.
        m_CameraElevation = Mathf.Clamp01(m_CameraElevation);

        // Angle should wrap around so that it stays within a consistent range.
        m_CameraAngle = m_CameraAngle - Mathf.Floor(m_CameraElevation);

        // We've been storing angle and elevation as parameters that go from 0.0f to 1.0f. Here we convert them into degrees and 
        // apply them to the real camera transform.
        Camera.main.transform.rotation = Quaternion.Euler(m_CameraElevation * 80.0f + 5.0f, m_CameraAngle * 360.0f, 0.0f);

        // The camera always looks at the origin, so to position it we just place it at the origin and pull it backwards by the correct distance.
        Camera.main.transform.position = m_CameraCenter + Camera.main.transform.forward * -Mathf.Lerp(m_CameraDistanceMin, m_CameraDistanceMax, m_CameraDistance);
    }
}
