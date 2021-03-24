using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageInteract : MonoBehaviour
{
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }
    

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Dust") {
            animator.SetTrigger("Trigger");
        }
    }
}
