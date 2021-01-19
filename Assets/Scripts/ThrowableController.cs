using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // To Launch the Throwable after a Delay
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
        // To assign the Material of the Trial Effect for the Throwable
        GetThrwTrailEffectMat();

        // Direction the throwable will be launched in
        var launchDir = Vector3.Normalize(GameManager.singleton.Player.transform.position -
                                            GameManager.singleton.Enemies[GameManager.singleton.enemiesCounter].transform.position);

        // The Delay Between Showing the Throwable to the Player and Launching it
        yield return new WaitForSeconds(GameManager.singleton.thrwLaunchDelay);

        // To Launch the throwable in the direction of the player
        gameObject.GetComponent<Rigidbody>().velocity = launchDir * GameManager.singleton.thrwSpeed;
    }

    // To assign the Material of the Trial Effect for the Throwable
    private void GetThrwTrailEffectMat()
    {
        var matIndex = -1;

        for (int i = 0; i < GameManager.singleton.enemyMaterials.Length; i++)
            if (gameObject.GetComponent<Renderer>().material.name.Replace(" (Instance)", "") ==
                GameManager.singleton.enemyMaterials[i].name.Replace(" (Instance)", ""))
                matIndex = i;

        if (matIndex != -1)
            gameObject.GetComponentInChildren<TrailRenderer>().material = GameManager.singleton.enemyEffectsMaterials[matIndex];
    }
}
