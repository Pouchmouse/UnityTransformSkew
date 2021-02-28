using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The Draggable class is placed on the Eye, and listens for mouse clicks. When you click, it stores the Plane made by the eye's current position and the forward vector of
// the camera. Then it moves the eye around that plane to match the mouse position until you release the mouse button.
//
// When you aren't dragging, it lets the set of three damped springs (see the SimpleSpring class) determine the eye's position.
//

public class Draggable : MonoBehaviour
{
    bool  m_IsDragging;
    Plane m_DraggingPlane;

    Vector3 m_InitialPosition;

    public float m_FloorHeight;

    public SkewAxisHandler m_Skew;
    public Transform       m_TowerContent;

    public InputHandler m_InputHandler;

    public SimpleSpring m_SpringX;
    public SimpleSpring m_SpringY;
    public SimpleSpring m_SpringZ;

    public void Start()
    {
        // We store our initial position when we start, so we know what resting position
        // to return to when we are released from dragging.

        m_InitialPosition = transform.position;
    }

    public void OnMouseDown()
    {
        // Okay, the player just clicked on us. That means we're now being dragged. So...

        // ...we set m_IsDragging to true, to tell us to follow the mouse position in our Update function.
        m_IsDragging = true;

        // ...we save the current camera plane. This is the plane we'll be dragged around in.
        m_DraggingPlane = new Plane(Camera.main.transform.forward, transform.position);

        // ...we tell the InputHandler to ignore mouse movement, so we don't also rotate the camera while being dragged.
        m_InputHandler.IgnoreInput = true;
    }

    void Update()
    {
        // m_IsDragging will become true if the player clicks on us.
        if(m_IsDragging)
        {
            // Okay, m_IsDragging is true. The first thing we should check is: does the player still have their finger on the mouse button?

            if (!Input.GetMouseButton(0))
            {
                // The player has released the mouse button, so we exit dragging mode:
                m_IsDragging = false;

                // ...and tell the input handler that the mouse is once again able to rotate the camera.
                m_InputHandler.IgnoreInput = false;

                // We inform our three springs about how far away we are from our 'rest' position on each axis.
                m_SpringX.m_Value = transform.position.x - m_InitialPosition.x;
                m_SpringY.m_Value = transform.position.y - m_InitialPosition.y;
                m_SpringZ.m_Value = transform.position.z - m_InitialPosition.z;

                // I'm not tracking the speed of the dragged object currently. But you could! If you track how much we move each frame and divide
                // by the timestep, you'd have our velocity, which you could plug in here in order to have momentum carry over when the eye is released.
                m_SpringX.m_Speed = 0.0f;
                m_SpringY.m_Speed = 0.0f;
                m_SpringZ.m_Speed = 0.0f;
            }
            else
            {
                // If we're here, then the player is still holding down the mouse button and dragging us. We need to stick to the position in
                // m_DraggingPlane that's under the mouse.

                // Fortunately, turning the mouse position into a ray that extends from the camera is pretty simple:
                float intersect_distance;
                Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // ...and so is getting the intersection between a ray and a plane:
                if(m_DraggingPlane.Raycast(mouse_ray, out intersect_distance))
                {
                    //This raycase returns a distance, not an intersection point. but passing that distance into ray.GetPoint() tells us
                    //where the ray crossed the plane.
                    Vector3 new_pos = mouse_ray.GetPoint(intersect_distance);

                    // ...but I don't feel like letting you drag us under the floor. So if you do that, I'm going to snap to the floor height.
                    new_pos.y = Mathf.Max(new_pos.y, m_InitialPosition.y + m_FloorHeight);

                    // And we're done! Snap to this position, we're now under the mouse (as long as the mouse isn't under the floor).
                    transform.position = new_pos;
                }
            }
        }
        else
        {
            // If we're here, it means the player isn't dragging us. That means we use springs to determine our position.

            // Before we start, there's one extra rule...
            // The 'Y-axis' spring isn't allowed to drop us below the floor, so clamp it if it does.
            if(m_SpringY.m_Value < m_FloorHeight)
            {
                m_SpringY.m_Value = m_FloorHeight;
                m_SpringY.m_Speed = Mathf.Abs(m_SpringY.m_Speed);
            }

            // All of our springs are working relative to our initial position.
            // So we start by assuming we're at that point...
            Vector3 new_position = m_InitialPosition;

            // ...and then we add each spring's value to one axis.
            new_position.x += m_SpringX.m_Value;
            new_position.y += m_SpringY.m_Value;
            new_position.z += m_SpringZ.m_Value;

            // That's it! We know where we are now.
            transform.position = new_position;
        }

        // Great, so whether we're being dragged or not, we've now decided where we (the eyeball) are in space.
        // Now we need to talk to the SkewAxisHandler to get it to warp the tower to match our position.
        // We assume that if we're at our initial position, then we'd need to apply no skew or stretching.
        // Any skew or stretching we do apply is thus based on how far away we are from our initial position,
        // and in which direction.

        Vector3 target_pos = transform.position;

        // Calculate how far have we been pulled away along the XZ plane? This will come in handy later.
        float horizontal_magnitude = Mathf.Sqrt(target_pos.x * target_pos.x + target_pos.z * target_pos.z);

        // Calculate the y-axis angle that we need to skew in. This is the angle that we've been pulled in.
        float facing_angle = Mathf.Rad2Deg * Mathf.Atan2(transform.position.x, transform.position.z) + 90.0f;

        // Calculate how much we need to skew the y-axis of the tower to lean correctly.
        float skew_angle   = Mathf.Rad2Deg * Mathf.Atan2(horizontal_magnitude, transform.position.y) + 90.0f;

        // Also, how much would the tower need to stretch to reach us at our new position?
        float length = transform.position.magnitude / m_InitialPosition.magnitude;

        // Okay, apply the stretch to the local y-axis of the tower. It's now the correct length to reach the eye.
        m_TowerContent.localScale = new Vector3(1.0f, length, 1.0f);

        // Hand the angle and skew to the SkewAxisController. It will handle the rest.
        m_Skew.m_FacingAngle = facing_angle;
        m_Skew.m_SkewAngle   = skew_angle;
    }
}
