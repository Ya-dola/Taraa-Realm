using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    public GameObject bumperFadePrefab;
    public float bumperFadeDelay;

    void OnTriggerEnter(Collider collider)
    {
        // If the Player Collides with the Bumper
        if (collider.gameObject.tag == "Player" && !GameManager.singleton.playerModified)
        {
            // To Stop the existing Player Movement
            GameManager.singleton.SetPlayerMove(false);

            ShowBumperFadeEffect();

            // To Reflect the Player
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
                            GameManager.singleton.playerPosRotMoveAcptableRange * 4f)
        {
            GameManager.singleton.Player.transform.position =
                Vector3.Lerp(GameManager.singleton.Player.transform.position,
                            targetPos,
                            GameManager.singleton.playerPosMoveSpeed * 1.5f * Time.fixedDeltaTime);

            yield return null;
        }

        // To Allow Swipe Movement once the Player has been Reflected
        GameManager.singleton.SetPlayerModified(false);
    }

    // To show the Bumper Fade Effect
    private void ShowBumperFadeEffect()
    {
        GameObject bumperFade = Instantiate(bumperFadePrefab, transform.position, transform.rotation);

        bumperFade.GetComponent<Animator>().Play("Bumper Fade Animation");

        Destroy(bumperFade, bumperFadeDelay);
    }
}