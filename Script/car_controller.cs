using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static car_controller;

public class car_controller : MonoBehaviour
{
    public enum Axel
    {
        front,
        back
    }
    public enum side
    {
        left,
        right
    }
    public InputManager IM;
    public driftController drift;
    [Serializable] 
    
    public struct wheels
    {
        public GameObject wheelmesh;
        public WheelCollider wheel;
        public Axel axel;
        public side side;
    }
    
    

    
    public List<wheels> Wheels;
    private Rigidbody rb;
    private float turnsen = 1.0f;
    
    
    Quaternion rot;
    Vector3 pos;
    public float brakes = 50.0f;
    private Vector3 q;
    public float grip = 0.5f;
    public GameObject com;
    public AnimationCurve turnRadius;
    private float car_l = 1.71255f;
    float angle;
    private Gear_System_Auto gears;
    public AnimationCurve velocityVSsteerangle;
    public float downforce = 50;
    bool isDrifting = false;
    int gear;
    private WheelFrictionCurve forward;
    private WheelFrictionCurve sideway;
    private float CurrentTorque;
    public float handBrakes = 2f;

    private void Start()
    {

        drift = GetComponent<driftController>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com.transform.localPosition;
        gears  = GetComponent<Gear_System_Auto>();

        
    }

    private void Update()
    {
        gear = gears.current_gear;
        
        float q = rb.velocity.magnitude;
        CurrentTorque = gears.currentTorque;
        
        


    }

    private void LateUpdate()
    {
        Brakes();
        accelerating();
        Turning();
        anim();
        drift.drift(isDrifting,rb.velocity.magnitude * 3.6f,IM.turn);
        
    }



    void accelerating()
    {
        foreach (var wheel in Wheels)
        {
            if (!IM.brakes )
            {
                if (wheel.axel == Axel.back)
                {
                    if (IM.throttle)
                    {

                        wheel.wheel.motorTorque = Mathf.Lerp(0, 1, 0.9f) * gears.currentTorque;
                        double p = wheel.wheel.rpm * wheel.wheel.radius * 2 * Math.PI;



                    }

                    else
                    {
                        wheel.wheel.motorTorque = 0;
                        if (Mathf.Round(Mathf.Abs(rb.velocity.magnitude)) > 0)
                        {
                            rb.AddForce(-transform.forward * handBrakes);
                        }



                    }
                }
               
                
            }
            

        }
    }

    float TurnRadius;
    void Turning()
    {
        float left_angle;
        float right_angle;
        if (gears.current_gear == 0)
        {
            TurnRadius = 10;
        }
        else
        {
            TurnRadius = turnRadius.Evaluate(Mathf.Abs(rb.velocity.magnitude)*3.6f);
        }

        foreach (var wheel in Wheels)
        {
            if(wheel.axel == Axel.front)
            {
                if (IM.turn > 0)
                {
                    if (wheel.side == side.right)
                    {
                        right_angle = turnsen * IM.turn * Mathf.Rad2Deg * Mathf.Atan(4.5f / (TurnRadius - car_l / 2));
                        
                        
                        
                            right_angle += Vector3.SignedAngle(transform.forward, transform.forward + rb.velocity, transform.up);
                            right_angle = Mathf.Clamp(right_angle, -75, 75);
                        
                            
                        
                        wheel.wheel.steerAngle = Mathf.Lerp(wheel.wheel.steerAngle, right_angle, 0.3f);
                    }
                    else
                    {
                        left_angle = turnsen * IM.turn * Mathf.Rad2Deg * Mathf.Atan(4.5f / (TurnRadius  + car_l / 2));
                        
                        
                            left_angle += Vector3.SignedAngle(transform.forward, transform.forward + rb.velocity, transform.up);
                            left_angle = Mathf.Clamp(left_angle, -75, 75);
                        
                            
                        
                            
                        wheel.wheel.steerAngle = Mathf.Lerp(wheel.wheel.steerAngle, left_angle, 0.3f);
                    }
                }
                else if (IM.turn < 0)
                {
                    if (wheel.side == side.left)
                    {
                        left_angle = turnsen  * IM.turn * Mathf.Rad2Deg * Mathf.Atan(4.5f / (TurnRadius - car_l / 2));

                           
                            left_angle += Vector3.SignedAngle(transform.forward, transform.forward + rb.velocity, transform.up);
                            left_angle = Mathf.Clamp(left_angle, -75, 75);
                        

                        wheel.wheel.steerAngle = Mathf.Lerp(wheel.wheel.steerAngle,left_angle, 0.3f);
                    }
                    else
                    {
                        right_angle = turnsen * IM.turn * Mathf.Rad2Deg * Mathf.Atan(4.5f / (TurnRadius + car_l / 2));

                        
                            right_angle += Vector3.SignedAngle(transform.forward, transform.forward + rb.velocity, transform.up);
                            right_angle = Mathf.Clamp(right_angle, -75, 75);
                        

                        wheel.wheel.steerAngle = Mathf.Lerp(wheel.wheel.steerAngle,right_angle, 0.3f);
                    }

                }
                else
                {
                    wheel.wheel.steerAngle = 0;
                }

            }
        }
    }
    float tempo;
    void Brakes()
    {
        
        if (Input.GetKey(KeyCode.Space) )//|| move == 0)
        {
            
            foreach(var wheel in Wheels)
            {
                if (IM.handbrake)// && wheel.axel == Axel.front)
                {
                    if (wheel.axel == Axel.back)
                    {
                        isDrifting = true;
                        wheel.wheel.brakeTorque = brakes;
                    }
                    
                        
                }
                //else
                //{
                  //  wheel.wheel.brakeTorque = brakes * 6 * Time.deltaTime;
                //}
                
            }
        }
        else
        {
            foreach(var wheel in Wheels)
            {
                isDrifting = false;
                wheel.wheel.brakeTorque = 0;

            }
        }
    }
    void downForce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }
    void anim()
    {
        foreach (var wheel in Wheels)
        {
            wheel.wheel.GetWorldPose(out pos,out rot);
            wheel.wheelmesh.transform.position = pos;
            wheel.wheelmesh.transform.rotation = rot;
        }
    }

}
