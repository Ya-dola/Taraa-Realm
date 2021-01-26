using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBuffController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the Buff Modifier
        if (collider.gameObject.tag == "Player")
            GameManager.singleton.BuffPlayerMoveFactor();
    }
}