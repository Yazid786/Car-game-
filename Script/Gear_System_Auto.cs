using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Gear_System_Auto : MonoBehaviour
{

    public enum transmissionType
    {
        Automatic,
        Manual
    }

    


    public Rigidbody rb;
    public WheelCollider wheel;
    public WheelCollider wheel1;
    public transmissionType Tt;
    public float velocity;
    public float transition_time = 2f;
    bool undertrans = false;
    public int current_gear;
    public float[] gearRatio = { 3, 2, 1.5f, 1, 0.5f };
    public AnimationCurve powerToRPM;
    private float currentRPM;
    private float tyreRPM;
    public float currentTorque;
    public float Horsepower;
    public float idleRpm = 800;
    public bool IsAutomatic = true;
    public float max_acc = 30.0f;
    private float smoothTime;
    public Text text;
    public Text trans;
    public Text gear;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        current_gear = 0;
    }

    private void FixedUpdate()
    {
        //Debug.Log(current_gear);
        velocity = rb.velocity.magnitude * 3.6f;
        gear.text = (current_gear + 1).ToString();
        tyreRPM = (wheel.rpm + wheel1.rpm) / 2;
        shift();
        calcTorque();
        shift();
        if (IsAutomatic)
        {
            Tt = transmissionType.Automatic;
            trans.text = "Automatic";
        }
        else
        {
            Tt = transmissionType.Manual;
            trans.text = "Manual";
        }

    }
    public void switchType()
    {
        if (IsAutomatic)
        {
            IsAutomatic = false;
        }
        else
        {
            IsAutomatic = true;
        }
    }

    void shift()
    {
       
        if (Tt == transmissionType.Automatic)
        {
            auto();
        }
        else
        {
            manual();
        }
        
    }


    public void calcTorque()
    {
        Debug.Log("Torque applied");
        float Velocity = 0;
        currentTorque = powerToRPM.Evaluate(currentRPM) * gearRatio[current_gear] * 3.6f ;
        currentRPM = Mathf.SmoothDamp(currentRPM, 1000 + (Mathf.Abs(tyreRPM) * 3.6f *gearRatio[current_gear]), ref Velocity, smoothTime: smoothTime);
        //currentRPM = Mathf.Clamp(currentRPM,0,5300);
        
        text.text = currentRPM.ToString();
    }
    public void manual()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (current_gear != 4 && !undertrans)
            {
                StartCoroutine(TransUp());
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (current_gear != 0 && !undertrans)
            {
                StartCoroutine(TransDown());
            }
        }
    }
    public void auto()
    {
        if (currentRPM > 4900)
        {
            if (current_gear != 4 && !undertrans)
            {
                StartCoroutine(TransUp());
            }
        }
        else if (currentRPM < 2000)
        {
            if (current_gear != 0 && !undertrans)
            {
                StartCoroutine(TransDown());
            }
        }

    }
    IEnumerator TransUp()
    {
        undertrans = true;
        yield return new WaitForSeconds(transition_time);
        current_gear++;
        undertrans = false;
    }
    IEnumerator TransDown()
    {
        undertrans = true;
        yield return new WaitForSeconds(transition_time);
        current_gear -= 1;
        undertrans = false;
    }

}
