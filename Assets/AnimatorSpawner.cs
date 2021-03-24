using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject jumpDust;
    [SerializeField] private GameObject stopDust;
    [SerializeField] private GameObject fallDust;
    [SerializeField] private GameObject stompDust;
    public static AnimatorSpawner instance;
    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void SpawnAnimation(string spawnAnimationObject) {
        if (spawnAnimationObject == "jumpDust") {
            Instantiate(jumpDust, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        } else if (spawnAnimationObject == "fallDust") {
            Instantiate(fallDust, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        } else if (spawnAnimationObject == "stopDust") {
            GameObject stopDustObj = Instantiate(stopDust, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            Transform temp = GetComponent<Transform>();
            if (temp.rotation.y == -1) {
                stopDustObj.transform.Rotate(new Vector3(0,0,0));
            } else if (temp.rotation.y == 0) {
                stopDustObj.transform.Rotate(new Vector3(0,180,0));
            }
        } else if (spawnAnimationObject == "stompDust") {
            Instantiate(stompDust, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        }
    }
}
