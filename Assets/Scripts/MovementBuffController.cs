using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBuffController : MonoBehaviour
{
    public GameObject buffEffectPrefab;
    public float buffEffectDelay;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, GameManager.singleton.moveModifierLifespan);
    }

    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the Buff Modifier
        if (collider.gameObject.tag == "Player")
        {
            ShowBuffEffect();
            GameManager.singleton.BuffPlayerMoveFactor();

            // To Destroy the Buff Modifier once used
            Destroy(gameObject);
        }
    }

    // To show the Buff Effect
    private void ShowBuffEffect()
    {
        GameObject buffEffect = Instantiate(buffEffectPrefab, transform.position, transform.rotation);

        Destroy(buffEffect, buffEffectDelay);
    }
}