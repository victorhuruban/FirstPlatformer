using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDustSD : MonoBehaviour
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
