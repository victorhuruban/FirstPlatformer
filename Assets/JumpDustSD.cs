using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDustSD : MonoBehaviour
{
    void Awake() {
        Animator temp = GetComponent<Animator>();
        if ((int)Random.Range(0,2) == 0) {
            temp.SetTrigger("Trigger1");
            Debug.Log("TRUE VAL FOR JUMP DUST");
        } else {
            temp.SetTrigger("Trigger2");
            Debug.Log("FALSE VAL FOR JUMP DUST");
        }
    }
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
