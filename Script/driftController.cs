using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class driftController : MonoBehaviour
{

    public car_controller car;
    public WheelFrictionCurve forward,sideways,a_forward,a_sideways;
    public float friction_multiplier = 0.7f;
    public float handbrakes = 2f;
    private float driftFactor;

    
    public void Start()
    {
        car = GetComponent<car_controller>();
        foreach (var wheel in car.Wheels)
        {
            a_forward = wheel.wheel.forwardFriction;
            forward = wheel.wheel.forwardFriction;
            a_sideways = wheel.wheel.sidewaysFriction;
            sideways = wheel.wheel.sidewaysFriction;
        }
        
    }
    public void drift(bool isDrifting,float velocity,float Turn)
    {
        if (isDrifting)
        {
            float Velocity = 0;
            forward.extremumValue = forward.asymptoteValue = Mathf.SmoothDamp(forward.extremumValue, driftFactor * handbrakes, ref Velocity, 0.05f * Time.deltaTime);
            sideways.extremumValue = sideways.asymptoteValue = Mathf.SmoothDamp(sideways.extremumValue, driftFactor * handbrakes, ref Velocity, 0.05f * Time.deltaTime);
           
            foreach (var wheel in car.Wheels)
            {
                wheel.wheel.forwardFriction = forward;
                wheel.wheel.sidewaysFriction = sideways;

            }
            forward.extremumValue = forward.asymptoteValue = sideways.extremumValue = sideways.asymptoteValue = 1f;
            foreach (var wheel in car.Wheels)
            {
                if (wheel.axel == car_controller.Axel.front)
                {
                    wheel.wheel.forwardFriction = forward;
                    wheel.wheel.sidewaysFriction = sideways;
                }
            }

        }
        else if (!isDrifting)
        {
            forward.extremumValue = forward.asymptoteValue = 1f;  
            sideways.extremumValue = sideways.asymptoteValue = 1f;

            foreach (var wheel in car.Wheels)
            {
                wheel.wheel.forwardFriction = forward;
                wheel.wheel.sidewaysFriction = sideways;

            }
        }
        WheelHit wheelHit;

        foreach (var wheel in car.Wheels)
        {
            if (wheel.axel == car_controller.Axel.back)
            {
                wheel.wheel.GetGroundHit(out wheelHit);
                if (wheelHit.sidewaysSlip < 0)
                {
                    driftFactor = (1 + -Turn) * Mathf.Abs(wheelHit.sidewaysSlip);
                    if (driftFactor < 0.5) driftFactor = 0.5f;
                }
                

                if (wheelHit.sidewaysSlip > 0)
                {
                    driftFactor = (1 + Turn) * Mathf.Abs(wheelHit.sidewaysSlip);
                    if (driftFactor < 0.5) driftFactor = 0.5f;
                }
                    
            }
            
        }

       


    }

}


