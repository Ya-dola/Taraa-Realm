using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the Bumper
        if (collider.gameObject.tag == "Player")
        {
            // To Stop the existing Player Movement
            GameManager.singleton.SetPlayerMove(false);

            var dirReflection = transform.position - collider.transform.position;

            StopAllCoroutines();
            GameManager.singleton.SetPlayerModified(false);
            StartCoroutine(ReflectPlayer(dirReflection.normalized * GameManager.singleton.bumperForce));

        }
    }

    private IEnumerator ReflectPlayer(Vector3 targetPos)
    {
        targetPos.y = GameManager.singleton.playerStartingPos.y;

        // To Disable Swipe Movement when the Player is being reflected
        GameManager.singleton.SetPlayerModified(true);

        while (Vector3.Distance(
                        GameManager.singleton.Player.transform.position,
                        targetPos) >=
                            GameManager.singleton.playerPosRotMoveAcptableRange)
        {
            GameManager.singleton.Player.transform.position =
                Vector3.Lerp(GameManager.singleton.Player.transform.position,
                            targetPos,
                            GameManager.singleton.playerPosMoveSpeed * Time.fixedDeltaTime);

            yield return null;
        }

        // To Allow Swipe Movement once the Player has been Reflected
        GameManager.singleton.SetPlayerModified(false);
    }
}