using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    

    public void TakeDamage(int damage) {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage and has " + health + " health left");
    }

}
