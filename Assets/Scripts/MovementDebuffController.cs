using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDebuffController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the DeBuff Modifier
        if (collider.gameObject.tag == "Player")
            GameManager.singleton.DeBuffPlayerMoveFactor();
    }
}
