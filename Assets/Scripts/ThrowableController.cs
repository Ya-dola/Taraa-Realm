using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaunchThrowable());
    }

    // Update is called once per frame
    void Update()
    {
        // To work only when the Game has started or has not ended or is not paused
        if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded || GameManager.singleton.GamePaused)
            return;


    }

    void OnCollisionEnter(Collision collision)
    {
        // If the throwable collides with the edge
        if (collision.gameObject.tag == "Edge")
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LaunchThrowable()
    {
        // Direction the throwable will be launched in
        var launchDir = Vector3.Normalize(GameManager.singleton.Player.transform.position -
                                            GameManager.singleton.Enemies[GameManager.singleton.enemiesCounter].transform.position);

        yield return new WaitForSeconds(GameManager.singleton.thrwLaunchDelay);

        // To Launch the throwable in the direction of the player
        gameObject.GetComponent<Rigidbody>().velocity = launchDir * GameManager.singleton.thrwSpeed;

    }
}
