using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDown : MonoBehaviour
{
    float originalY;
    public float upDownFloating;

    void Start() {
        originalY = this.transform.position.y;
    }

    void FixedUpdate() {
        transform.position = new Vector2(transform.position.x,
        originalY + ((float) Mathf.Sin(Time.time) * upDownFloating));
    }
}
