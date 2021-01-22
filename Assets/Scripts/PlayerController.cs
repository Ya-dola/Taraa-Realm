using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimator;
    private Vector3 playerPosOld;
    private Vector3 playerPosLast;
    private Vector3 playerPosMoveTo;
    private Quaternion playerRotMoveTo;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Fixed Update used mainly for Physics Calculations
    void FixedUpdate()
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

        // To Move and Rotate the Player
        PerformPlayerMovement();

        // To Move the Player Using Keyboard Keys
        PerformPlayerKeyboardMovement();

        // To have a delay for the player to recover
        PlayerRecovery();
    }

    // Late Update used mainly for Camera Calculations and Calculations that need to occur after movement has occured
    // Occurs after physics is applied 
    void LateUpdate()
    {
        // To Update the Position of the Player with the Constraints
        ConstraintPlayerPosition();
    }

    // To Ensure the Character will not leave the bounds of the Playable Area
    private void ConstraintPlayerPosition()
    {
        playerPosOld = transform.position;

        // To make the Character not fall from the Realm through the Left Side
        if (transform.position.x < GameManager.singleton.leftSideXEdgePos)
            playerPosOld.x = GameManager.singleton.leftSideXEdgePos;

        // To make the Character not fall from the Realm through the Right Side
        if (transform.position.x > GameManager.singleton.rightSideXEdgePos)
            playerPosOld.x = GameManager.singleton.rightSideXEdgePos;

        // To make the Character not be able to leave the Realm through the Bottom
        if (transform.position.z < -GameManager.singleton.topBotZEdgePos)
            playerPosOld.z = -GameManager.singleton.topBotZEdgePos;

        // To make the Character not be able to leave the Realm through the Top
        if (transform.position.z > GameManager.singleton.topBotZEdgePos)
            playerPosOld.z = GameManager.singleton.topBotZEdgePos;

        // To Constraint the Position of the Character within the Realm
        transform.position = playerPosOld;
    }

    // To perform Updates on which animation should be playing for the Player
    private void AnimationUpdates()
    {
        if (!playerAnimator.GetBool("GameStarted"))
            playerAnimator.SetBool("GameStarted", GameManager.singleton.GameStarted);

        // If the Player Won the Game
        if (GameManager.singleton.GameWon)
            playerAnimator.Play("Game Won");

        // To Play an Idle Animation
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            var idleAnimChance = Random.Range(0f, 1f);

            if (idleAnimChance >= GameManager.singleton.IdleAnimRange[2])
                playerAnimator.Play("Battle Cry");
            else if (idleAnimChance >= GameManager.singleton.IdleAnimRange[1])
                playerAnimator.Play("Hip Hop");
            else if (idleAnimChance >= GameManager.singleton.IdleAnimRange[0])
                playerAnimator.Play("Gangnam Style");
            else
                playerAnimator.Play("Shuffling");
        }

        // To Play the Player Movement Animation
        if (GameManager.singleton.playerMove && !GameManager.singleton.playerMoveAnimPlayed)
        {
            playerAnimator.Play("Character Moving");
            playerAnimator.SetBool("CharacterMoving", true);
            GameManager.singleton.SetPlayerMoveAnimPlayed(true);
        }
        else
            playerAnimator.SetBool("CharacterMoving", false);

        // To Ensure the Movement Animation does not Play Twice
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Character Moving"))
            playerAnimator.SetBool("CharacterMoving", false);

        // To Play the Player Falling on Hit Animation
        if (GameManager.singleton.playerHit)
        {
            playerAnimator.Play("Player Hit");
            playerAnimator.SetBool("CharacterHit", true);
        }

        // To Ensure the Movement Animation does not Play Twice
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player Hit"))
            GameManager.singleton.SetPlayerHit(false);

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Standing Up"))
            playerAnimator.SetBool("CharacterHit", false);
    }

    // To Move and Rotate the Player
    private void PerformPlayerMovement()
    {
        // To Move and Rotate the Player Up
        if (GameManager.singleton.playerMoveUp)
        {
            playerPosLast = transform.position;
            playerPosMoveTo = playerPosLast;
            playerPosMoveTo.z += GameManager.singleton.playerMoveFactor;

            playerRotMoveTo = Quaternion.Euler(0, 0, 0);

            GameManager.singleton.SetPlayerMoveUp(false);
            GameManager.singleton.SetPlayerMove(true);
        }

        // To Move and Rotate the Player Down
        if (GameManager.singleton.playerMoveDown)
        {
            playerPosLast = transform.position;
            playerPosMoveTo = playerPosLast;
            playerPosMoveTo.z -= GameManager.singleton.playerMoveFactor;

            playerRotMoveTo = Quaternion.Euler(0, 180f, 0);

            GameManager.singleton.SetPlayerMoveDown(false);
            GameManager.singleton.SetPlayerMove(true);
        }

        // To Move and Rotate the Player Right
        if (GameManager.singleton.playerMoveRight)
        {
            playerPosLast = transform.position;
            playerPosMoveTo = playerPosLast;
            playerPosMoveTo.x += GameManager.singleton.playerMoveFactor;

            playerRotMoveTo = Quaternion.Euler(0, 90f, 0);

            GameManager.singleton.SetPlayerMoveRight(false);
            GameManager.singleton.SetPlayerMove(true);
        }

        // To Move and Rotate the Player Left
        if (GameManager.singleton.playerMoveLeft)
        {
            playerPosLast = transform.position;
            playerPosMoveTo = playerPosLast;
            playerPosMoveTo.x -= GameManager.singleton.playerMoveFactor;

            playerRotMoveTo = Quaternion.Euler(0, -90f, 0);

            GameManager.singleton.SetPlayerMoveLeft(false);
            GameManager.singleton.SetPlayerMove(true);
        }

        if (GameManager.singleton.playerMove)
        {
            // To move the Player Accordingly to the Button Pressed
            transform.position = Vector3.Lerp(transform.position,
                                              playerPosMoveTo,
                                              GameManager.singleton.playerPosMoveSpeed * Time.fixedDeltaTime);

            // To rotate the Player Accordingly to the Button Pressed
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  playerRotMoveTo,
                                                  GameManager.singleton.playerRotMoveSpeed * Time.fixedDeltaTime);

            // To Indicate that the Player has finished moving
            if (Vector3.Distance(transform.position, playerPosMoveTo) <
                                    GameManager.singleton.playerPosRotMoveAcptableRange &&
                transform.rotation.eulerAngles.y - playerRotMoveTo.eulerAngles.y <
                                    GameManager.singleton.playerPosRotMoveAcptableRange)
            {
                GameManager.singleton.SetPlayerMove(false);
                GameManager.singleton.SetPlayerMoveAnimPlayed(false);
            }
        }
    }

    // To Move the Player Using Keyboard Keys
    private void PerformPlayerKeyboardMovement()
    {
        if (Input.GetKeyUp(KeyCode.Keypad8))
            GameManager.singleton.upButton.onClick.Invoke();
        // GameManager.singleton.MovePlayerUp();

        if (Input.GetKeyUp(KeyCode.Keypad2))
            GameManager.singleton.downButton.onClick.Invoke();
        // GameManager.singleton.MovePlayerDown();

        if (Input.GetKeyUp(KeyCode.Keypad6))
            GameManager.singleton.rightButton.onClick.Invoke();
        // GameManager.singleton.MovePlayerRight();

        if (Input.GetKeyUp(KeyCode.Keypad4))
            GameManager.singleton.leftButton.onClick.Invoke();
        // GameManager.singleton.MovePlayerLeft();
    }

    // To have a delay for the player to recover
    public void PlayerRecovery()
    {
        // To only add the recovery delay if it has not started
        if (!GameManager.singleton.playerRecovered &&
            GameManager.singleton.playerRecoveryTimeTemp == 0)
            GameManager.singleton.SetPlayerRecoveryTimeTemp(GameManager.singleton.playerRecoveryTime);

        // To delay the time between Player being hit and continuing the game
        if (!GameManager.singleton.playerRecovered)
        {
            if (GameManager.singleton.playerRecoveryTimeTemp > 0)
                GameManager.singleton.SetPlayerRecoveryTimeTemp(
                        GameManager.singleton.playerRecoveryTimeTemp -
                            (GameManager.singleton.playerRecoverySpeed * Time.fixedDeltaTime));
            else
            {
                GameManager.singleton.SetPlayerRecovered(true);
                GameManager.singleton.SetPlayerRecoveryTimeTemp(0f);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // If the Throwable Collides with the Player
        if (collider.gameObject.tag == "Throwable")
        {
            // To Play the Player Falling on Hit Animation
            GameManager.singleton.SetPlayerHit(true);

            // To Indicate that the Player needs to recover
            GameManager.singleton.SetPlayerRecovered(false);
        }
    }
}
