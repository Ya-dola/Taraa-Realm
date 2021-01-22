using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator enemyAnimator;

    void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // To perform Updates on which animation should be playing for the Player
        AnimationUpdates();

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

    // To perform Updates on which animation should be playing for the Enemy
    private void AnimationUpdates()
    {
        if (!enemyAnimator.GetBool("GameStarted"))
            enemyAnimator.SetBool("GameStarted", GameManager.singleton.GameStarted);

        // If the Player Won the Game
        if (GameManager.singleton.GameWon)
            enemyAnimator.Play("Game Won");

        // To Stop the Enemy Throwing Animation
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw Thrw"))
            enemyAnimator.SetBool("CharacterThrw", false);
    }
}
