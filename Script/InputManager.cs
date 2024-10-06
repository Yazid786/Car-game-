using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public bool throttle;
    public bool brakes;
    public bool handbrake ;
    public float turn;

    public void Update()
    {
            throttle = Input.GetKey(KeyCode.W);
            brakes = Input.GetKey(KeyCode.S);
            handbrake = Input.GetKey(KeyCode.Space);
            turn = Input.GetAxis("Horizontal");
    }
}
