using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationInteract : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("Entered on foilage");
    }
}
