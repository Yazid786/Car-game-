using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friction : MonoBehaviour
{
    public car_controller cont;
    public Rigidbody rb;
    public Text speed;


    void Update()
    {
        friction();
    }
    

    void friction()
    {
        foreach (var wheel in cont.Wheels )
        {
            if (wheel.axel == car_controller.Axel.back)
            {
                
                if (wheel.side == car_controller.side.left)
                {
                    double p = wheel.wheel.rpm * wheel.wheel.radius;
                    int k = Mathf.RoundToInt((float)p);
                    int q = Mathf.RoundToInt(rb.velocity.magnitude * 3.6f);
                    speed.text = k + " " + q;
                    //if(p != q)
                    //  {
                    // Debug.Log("Slipping!");
                    //}
                }


            }


        }
    }
}
