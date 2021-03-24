using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompDustSD : MonoBehaviour
{
    void Awake() {
        Animator temp = GetComponent<Animator>();
        temp.SetTrigger("Trigger");
    }
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
