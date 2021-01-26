using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem whiteLaunchPs;
    public ParticleSystem charLaunchPs;

    void Awake()
    {
        // To determine the size of the throwable
        SetThrwScaleSize();
    }

    // Start is called before the first frame update
    void Start()
    {
        // To Launch the Throwable after a Delay
        StartCoroutine(LaunchThrowable());

        // To Ensure all Throwables eventually get destroyed
        Destroy(gameObject, GameManager.singleton.thrwLifespan);
    }

    // Update is called once per frame
    void Update()
    {
        // To work only when the Game has started or has not ended or is not paused
        // if (!GameManager.singleton.GameStarted || GameManager.singleton.GameEnded || GameManager.singleton.GamePaused)
        //     return;

        // To destroy the Throwable if the player is recovering or the game has ended
        if (!GameManager.singleton.playerRecovered || GameManager.singleton.GameEnded)
        {
            // To Stop and Disable the Throwable
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;

            Destroy(gameObject, GameManager.singleton.thrwPlayerHitDelay);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the Throwable collides with the Edge
        if (collision.gameObject.tag == "Edge")
        {
            GameObject destEffect = Instantiate(GameManager.singleton.ConfettiPs, transform.position, Quaternion.identity);

            Destroy(gameObject);

            Destroy(destEffect, GameManager.singleton.effectPsDestDelay);

            GameManager.singleton.thrwEdgeCollideSoundCounter++;

            if (GameManager.singleton.thrwEdgeCollideSoundCounter % 4 == 0)
            {
                // Plays the sound between the Camera's position and the Throwable's position
                AudioSource.PlayClipAtPoint(GameManager.singleton.thrwEdgeCollideSound,
                                            0.9f * Camera.main.transform.position + 0.1f * transform.position,
                                            GameManager.singleton.thrwEdgeCollideSoundVolume);
            }

            // To Indicate that a Throwable was launched
            GameManager.singleton.SetGameBallCount(GameManager.singleton.gameBallCount - 1);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // If the Throwable Collides with the Player
        if (collider.gameObject.tag == "Player")
        {
            // To Shake the Camera
            ShakeCamera(GameManager.singleton.cameraShakeDuration, GameManager.singleton.cameraShakeAmount);

            // To Play the Explosion Effect 
            GameObject destEffect = Instantiate(GameManager.singleton.ExplosionPs, transform.position, Quaternion.identity);

            // To Reduce the Score when the Player is hit
            GameManager.singleton.SubThrwHitScore();

            // To Reduce the Number of Times the Player can be hit
            GameManager.singleton.ReduceHitCount();

            // To reset the Score Multiplier
            GameManager.singleton.ResetSuccessiveDodgeCounter();

            // To Stop and Disable the Throwable
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;

            // To show the Broken Throwable
            GameObject thrwBroken = Instantiate(GameManager.singleton.thrwBrokenPrefab, transform.position, Quaternion.identity);
            thrwBroken.transform.localScale = gameObject.transform.localScale;

            for (int i = 0; i < thrwBroken.transform.childCount; i++)
            {
                // Assigning the Destructable's Material to the Broken Parts of the Destructable
                thrwBroken.transform.GetChild(i).GetComponent<Renderer>().material = GetComponent<Renderer>().material;

                // Giving an Upwards Explosive force to the Broken Parts
                thrwBroken.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(GameManager.singleton.thrwExplosionForce,
                                                                                             transform.position,
                                                                                             GameManager.singleton.thrwExplosionRadius,
                                                                                             GameManager.singleton.thrwExplosionUpwardsMod);
            }

            // Plays the sound between the Camera's position and the Throwable's position
            AudioSource.PlayClipAtPoint(GameManager.singleton.thrwBrokenSound,
                                        0.9f * Camera.main.transform.position + 0.1f * transform.position,
                                        GameManager.singleton.thrwBrokenSoundVolume);

            // Destroy the Game Objects
            Destroy(gameObject, GameManager.singleton.thrwPlayerHitDelay);
            Destroy(destEffect, GameManager.singleton.effectPsDestDelay);
            Destroy(thrwBroken, GameManager.singleton.thrwBrokenDelay);
        }
    }

    private IEnumerator LaunchThrowable()
    {
        // To assign the Material of the Trial Effect for the Throwable
        GetThrwTrailEffectMat();

        // To be able to identify the enemy which will throw the throwable
        var thrwEnemy = GameManager.singleton.Enemies[GameManager.singleton.enemiesCounter];

        // Direction the throwable will be launched in
        var launchDir = Vector3.Normalize(
                               GameManager.singleton.Player.transform.position - thrwEnemy.transform.position);

        // To Randomize the Direction of the Throwable
        if (Random.Range(0f, 1f) < 0.25f)
        {
            launchDir = new Vector3(launchDir.x + Random.Range(-GameManager.singleton.thrwLaunchVariance,
                                                                GameManager.singleton.thrwLaunchVariance),
                                    launchDir.y,
                                    launchDir.z + Random.Range(-GameManager.singleton.thrwLaunchVariance,
                                                                GameManager.singleton.thrwLaunchVariance)).normalized;
        }

        // To move the Particles in the direction that the disc is launched in
        var whiteLaunchPsForce = whiteLaunchPs.forceOverLifetime;
        var charLaunchPsForce = charLaunchPs.forceOverLifetime;

        whiteLaunchPsForce.x = launchDir.x * GameManager.singleton.launchPsSpeed;
        whiteLaunchPsForce.z = launchDir.z * GameManager.singleton.launchPsSpeed;

        charLaunchPsForce.x = launchDir.x * GameManager.singleton.launchPsSpeed;
        charLaunchPsForce.z = launchDir.z * GameManager.singleton.launchPsSpeed;

        // The Delay Between Showing the Throwable to the Player and Launching it
        yield return new WaitForSeconds(GameManager.singleton.thrwLaunchDelay);

        // To Play the Enemy Throwing Animation
        thrwEnemy.GetComponent<Animator>().Play("Throw Thrw");
        thrwEnemy.GetComponent<Animator>().SetBool("CharacterThrw", true);

        // To Launch the throwable in the direction of the player
        gameObject.GetComponent<Rigidbody>().velocity = launchDir * GameManager.singleton.gameThrwSpeed;

        // To Launch the Particle Systems when the Throwable is launched
        whiteLaunchPs.Play();
        charLaunchPs.Play();

        // The Delay Between Switching the Layer the Throwable belongs to to one that collides with the Edges
        yield return new WaitForSeconds(GameManager.singleton.thrwLaunchLayerDelay);

        gameObject.layer = LayerMask.NameToLayer("Thrw");
    }

    // To assign the Material of the Trial Effect for the Throwable
    private void GetThrwTrailEffectMat()
    {
        var matIndex = -1;
        var charLaunchPsColor = charLaunchPs.main;

        for (int i = 0; i < GameManager.singleton.enemyMaterials.Length; i++)
            if (gameObject.GetComponent<Renderer>().material.name.Replace(" (Instance)", "") ==
                GameManager.singleton.enemyMaterials[i].name.Replace(" (Instance)", ""))
                matIndex = i;

        if (matIndex != -1)
        {
            gameObject.GetComponentInChildren<TrailRenderer>().material = GameManager.singleton.enemyEffectsMaterials[matIndex];

            // To set the color of the trail for the Character Launch Particles
            charLaunchPs.GetComponent<ParticleSystemRenderer>().trailMaterial = GameManager.singleton.enemyMaterials[matIndex];
            charLaunchPsColor.startColor = GameManager.singleton.enemyMaterials[matIndex].color;
        }
    }

    // To determine the size of the throwable
    private void SetThrwScaleSize()
    {
        var chance = Random.Range(0f, 1f);

        if (chance >= GameManager.singleton.thrwSizeRange[1])
            gameObject.transform.localScale = new Vector3(GameManager.singleton.thrwSizeScale[2],
                                                          GameManager.singleton.thrwSizeScale[2],
                                                          GameManager.singleton.thrwSizeScale[2]);
        else if (chance >= GameManager.singleton.thrwSizeRange[0])
            gameObject.transform.localScale = new Vector3(GameManager.singleton.thrwSizeScale[1],
                                                          GameManager.singleton.thrwSizeScale[1],
                                                          GameManager.singleton.thrwSizeScale[1]);
        else
            gameObject.transform.localScale = new Vector3(GameManager.singleton.thrwSizeScale[0],
                                                          GameManager.singleton.thrwSizeScale[0],
                                                          GameManager.singleton.thrwSizeScale[0]);
    }

    // To Shake the Camera
    public void ShakeCamera(float camShakeDuration, float camShakeAmt)
    {
        // To Stop Any Existing Camera Shakes
        StopAllCoroutines();
        StartCoroutine(ShakeCameraIEnum(camShakeDuration, camShakeAmt));
    }

    private IEnumerator ShakeCameraIEnum(float camShakeDuration, float camShakeAmt)
    {
        while (camShakeDuration > 0)
        {
            Camera.main.transform.position = GameManager.singleton.camPosEnd +
                                                (Random.insideUnitSphere * camShakeAmt);

            camShakeDuration -= GameManager.singleton.cameraShakeDurationStep;
            camShakeAmt -= GameManager.singleton.cameraShakeAmountStep;

            yield return null;
        }

        // To reset the Camera Position after Shaking the Camera
        Camera.main.transform.position = GameManager.singleton.camPosEnd;
    }
}
