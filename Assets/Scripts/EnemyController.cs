using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // To work only when the Game has started or has not ended or is not paused
        if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded || GameManager.singleton.GamePaused)
            return;

        // To Rotate the Enemy towards the Player
        PerformEnemyRotation();
    }

    // To Rotate the Enemy towards the Player
    private void PerformEnemyRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(
                                                    GameManager.singleton.Player.transform.position -
                                                            transform.position),
                                              GameManager.singleton.enemyRotMoveSpeed * Time.fixedDeltaTime);
    }
}
