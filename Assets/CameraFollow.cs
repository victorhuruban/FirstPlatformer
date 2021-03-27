using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float timeWHeld;
    public float timeSHeld;
    public float timeWNEEDHeld;
    public float timeSNEEDHeld;

    public float smoothSpeed = .125f;

    void Start() {
        timeWHeld = 0;
        timeSHeld = 0;
    }

    void FixedUpdate() {
        
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void Update() {
        if (target.rotation.y == -1) {
            offset.x = -3;
        } else offset.x = 3;

        if (Input.GetKey(KeyCode.W)) {
            if (timeWHeld < timeWNEEDHeld) {
                timeWHeld += Time.deltaTime;
            } else {
                offset.y = 2;
            }
        }

        if (Input.GetKey(KeyCode.S)) {
            if (timeSHeld < timeSNEEDHeld) {
                timeSHeld += Time.deltaTime;
            } else {
                offset.y = -2;
            }
        }

        if (Input.GetKeyUp(KeyCode.W)) {
            timeWHeld = 0;
            offset.y = 0;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            timeSHeld = 0;
            offset.y = 0;
        }
    }
}
