using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The SkewAxisHandler assumes you have three transforms in a hierarchy. It wants to be a component of the top transform.
// And it wants 'm_ChildTransform' to be the transform directly under it.
// And it wants 'm_ContentTransform' to be the transform directly under m_ChildTransform.
//
// You're free to put as much content inside m_ContentTransform as you like. You could have the entire scene in there and
// it should end up being skewed correctly.
//

[ExecuteInEditMode]
public class SkewAxisHandler : MonoBehaviour
{
    [Range(10.0f, 170.0f)]
    public float m_SkewAngle = 90.0f;  // m_SkewAngle is the angle you want to have between the X-Axis and the Y-Axis.
    public float m_FacingAngle = 0.0f; // m_FacingAngle is the angle you want the X-Axis to be facing in.

    public Transform m_ChildTransform;    // This is a transform immediately below the transform this component is on.
    public Transform m_ContentTransform;  // This is a transform immediately below m_ChildTransform. You put content you want skewed under this one.

    void Update()
    {
        // We start the skew process by extending the x-axis of our own transform by the amount necessary to
        // get the x and y axis of m_ChildTransform to be at the desired SkewAngle.
        // You're going to have to trust me that this is the right formula because I can't draw pictures in the comments...
        float desired_scale = 1.0f / Mathf.Tan(Mathf.Deg2Rad * m_SkewAngle * 0.5f);

        // Okay, set our own x-axis to that scale.
        transform.localScale = new Vector3(desired_scale, 1.0f, 1.0f);

        // Doing that warped the child transform into having the correct skew, but it also changed the length of its x and y axes.
        // We want to correct for that so the axes are just skewed not lengthed or shortened.
        // Again, sorry for the lack of diagrams: This is how much you need to scale the child transform axes back so that the end
        // result is normalized again:
        float child_scale = 1.0f / Mathf.Sqrt(0.5f + desired_scale * desired_scale * 0.5f);

        // Apply that scale to the child transform's x- and y-axis. 
        m_ChildTransform.localScale = new Vector3(child_scale, child_scale, 1.0f);

        // Now, we want to be able to determine which world-space angle we're skewing in. Our formula just skew's m_ChildTransform's 
        // y-axis towards its x-axis, so we rotate ourselves to make m_ChildTransform's x-axis point the correct direction. Then we
        // rotate m_ContentTransform the opposite way, so the content is now skewed along the desired axis.

        // First, what facing direction do we want, expressed as a rotation around the y-axis?
        Quaternion y_rotation = Quaternion.AngleAxis(m_FacingAngle, Vector3.up);

        // Great, apply that rotation to us.
        transform.localRotation = y_rotation * Quaternion.AngleAxis(m_SkewAngle * 0.5f, new Vector3(0.0f, 0.0f, 1.0f));

        // And apply the opposite to the content.
        m_ContentTransform.localRotation = Quaternion.Inverse(y_rotation);

        // Done! We have constructed a transform that's skewed by the correct angle, in the correct direction, with the local scale
        // of the content left unaffected.
    }
}
