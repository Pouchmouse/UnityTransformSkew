using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// A very simple spring simulation. 
// Turning up the SpringConstant will make it vibrate faster.
// Turning up the Dampening will decrease the amount it wiggles before coming to a rest.
// Both values should be NEGATIVE if you want the spring to return to the origin instead flying off into outer space.
//

public class SimpleSpring : MonoBehaviour
{
    public float m_SpringConstant;
    public float m_Dampening;
    public float m_Value;
    public float m_Speed;

    void Update()
    {
        // Springs accelerate towards their resting position.
        // The further from their resting position, the strong the acceleration.
        float accel = m_SpringConstant * m_Value;

        // Apply that acceleration to the current speed.
        m_Speed += accel * Time.deltaTime;

        // The faster the spring is going, the more dampening friction it experiences.
        // Slow the speed by the dampening value.
        m_Speed += m_Dampening * m_Speed * Time.deltaTime;

        // Okay, now apply the speed to the position and we're done.
        m_Value += m_Speed * Time.deltaTime;
    }
}
