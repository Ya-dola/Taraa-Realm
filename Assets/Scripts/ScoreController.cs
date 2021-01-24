using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private SphereCollider scoreCollider;

    void Awake()
    {
        scoreCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreCollider.radius = GameManager.singleton.scoreDetectorRadius;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Throwable")
        {
            GameManager.singleton.AddThrwDodgeScore();
            GameManager.singleton.AddSuccessiveDodgeCounter();
        }

        // To Add Multiplier to the score every X successive dodges
        if (GameManager.singleton.successiveDodgeCounter != 0 &&
            GameManager.singleton.successiveDodgeCounter % GameManager.singleton.successiveDodgeFactor == 0)
            GameManager.singleton.ApplyDodgeValueMultiplier();
    }
}
