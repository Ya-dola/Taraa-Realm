using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDebuffController : MonoBehaviour
{
    public GameObject deBuffEffectPrefab;
    public float deBuffEffectDelay;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, GameManager.singleton.moveModifierLifespan);
    }

    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the DeBuff Modifier
        if (collider.gameObject.tag == "Player")
        {
            ShowDeBuffEffect();
            GameManager.singleton.DeBuffPlayerMoveFactor();

            // To Destroy the DeBuff Modifier once used
            Destroy(gameObject);
        }
    }

    // To show the DeBuff Effect
    private void ShowDeBuffEffect()
    {
        GameObject deBuffEffect = Instantiate(deBuffEffectPrefab, transform.position, transform.rotation);

        Destroy(deBuffEffect, deBuffEffectDelay);
    }
}
