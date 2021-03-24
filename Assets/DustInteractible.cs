using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustInteractible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Interactable_foliage") {
            Animator temp = col.gameObject.GetComponent<Animator>();
            temp.SetTrigger("Trigger");
        }
    }
}
