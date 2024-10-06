using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform Player;
    public Rigidbody rb;
    public Vector3 Offset;
    public float speed;
    public float offset;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 playerForward = (rb.velocity + Player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, Player.position + Player.TransformVector(Offset) + playerForward * (-offset), speed * Time.deltaTime);
        transform.LookAt(Player);
    }
}
