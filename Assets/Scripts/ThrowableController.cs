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
        // If the Throwable collides with the Edge
        if (collision.gameObject.tag == "Edge")
        {
            GameObject destEffect = Instantiate(GameManager.singleton.ConfettiPs, transform.position, Quaternion.identity);

            Destroy(gameObject);

            Destroy(destEffect, GameManager.singleton.effectPsDestDelay);
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

            // To Stop and Disable the Throwable
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;

            // To show the Broken Throwable
            GameObject thrwBroken = Instantiate(GameManager.singleton.thrwBrokenPrefab, transform.position, Quaternion.identity);

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

        // Direction the throwable will be launched in
        var launchDir = Vector3.Normalize(GameManager.singleton.Player.transform.position -
                                            GameManager.singleton.Enemies[GameManager.singleton.enemiesCounter].transform.position);

        // To move the Particles in the direction that the disc is launched in
        var whiteLaunchPsForce = whiteLaunchPs.forceOverLifetime;
        var charLaunchPsForce = charLaunchPs.forceOverLifetime;

        whiteLaunchPsForce.x = launchDir.x * GameManager.singleton.launchPsSpeed;
        whiteLaunchPsForce.z = launchDir.z * GameManager.singleton.launchPsSpeed;

        charLaunchPsForce.x = launchDir.x * GameManager.singleton.launchPsSpeed;
        charLaunchPsForce.z = launchDir.z * GameManager.singleton.launchPsSpeed;

        // The Delay Between Showing the Throwable to the Player and Launching it
        yield return new WaitForSeconds(GameManager.singleton.thrwLaunchDelay);

        // To Launch the throwable in the direction of the player
        gameObject.GetComponent<Rigidbody>().velocity = launchDir * GameManager.singleton.thrwSpeed;

        // To Launch the Particle Systems when the Throwable is launched
        whiteLaunchPs.Play();
        charLaunchPs.Play();
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
