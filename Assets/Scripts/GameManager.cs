using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }
    public bool GamePaused { get; private set; }

    [Header("Player")]
    public Vector3 playerStartingPos;
    [Range(0, 48f)]
    public float playerPosMoveSpeed;
    [Range(0, 48f)]
    public float playerRotMoveSpeed;
    public float playerPosRotMoveAcptableRange;
    public bool playerMove { get; private set; }
    public bool playerMoveUp { get; private set; }
    public bool playerMoveDown { get; private set; }
    public bool playerMoveRight { get; private set; }
    public bool playerMoveLeft { get; private set; }
    public GameObject Player { get; private set; }

    [Header("Throwables")]
    public GameObject[] thrwPrefabs;
    [Range(0, 48f)]
    public float thrwSpeed;
    public GameObject[] thrwBrokenPrefabs;
    public float thrwBrokenDelay;
    public float thrwExplosionForce;
    public float thrwExplosionRadius;
    public float thrwExplosionUpwardsMod;
    public AudioClip thrwBrokenSound;

    [Range(0, 1)]
    public float thrwBrokenSoundVolume;

    [Header("Effects")]
    public GameObject thrwTrailPrefab;
    public float cameraShakeAmount;
    public float cameraShakeAmountStep { get; set; }
    public float cameraShakeDuration;
    public float cameraShakeDurationStep { get; set; }
    public float cameraShakeStepFactor;

    [Header("Camera")]
    public Vector3 camPosStart;
    public Vector3 camPosEnd;
    public float camRotXStart;
    public float camRotXEnd;
    public float camFovStart;
    public float camFovEnd;
    public float camPosTransitionSpeed;
    public float camRotTransitionSpeed;
    public float camFovTransitionSpeed;
    public float camPosRotAcptableRange;
    private bool camStartPosTransition;
    private bool camPosTransitionDone;
    private bool camStartRotTransition;
    private bool camRotTransitionDone;
    private bool camStartFovTransition;
    private bool camFovTransitionDone;

    [Header("Realm")]
    public float topBotZEdgePos;
    public float rightSideXEdgePos;
    public float leftSideXEdgePos;

    [Header("Materials")]
    public Material playerMaterial;
    public Material playerEffectsMaterial;
    public Color playerColor { get; set; }
    public Material[] enemyMaterials;
    public Material[] enemyEffectsMaterials;
    public Material enemyMaterial { get; set; }
    public Material enemyEffectsMaterial { get; set; }
    public Color enemyColor { get; set; }

    [Header("Level Management")]
    public string baseSceneName;
    private int sceneCounter;
    private int nextSceneInt;
    private int currentSceneInt;
    private AsyncOperation sceneLoader;

    [Header("User Interface")]
    public GameObject tapToStyleTMP;
    public TextMeshProUGUI levelText;
    public GameObject pauseMenu;
    public GameObject gameWonMenu;
    public GameObject gameLostMenu;
    public GameObject pauseButton;
    public GameObject controlButtons;
    public Button upButton;
    public Button downButton;
    public Button rightButton;
    public Button leftButton;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    void Awake()
    {
        // Creates a Single Instance of the Game Manager through out the entire game
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        // To cap the Unity Game View Frame Rate when Maximized
#if UNITY_EDITOR
        // VSync must be disabled
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting Default Values
        camStartFovTransition = false;
        camStartPosTransition = false;
        camStartRotTransition = false;
        camPosTransitionDone = false;
        camFovTransitionDone = false;
        camRotTransitionDone = false;
        sceneCounter = 0;
        cameraShakeAmountStep = cameraShakeAmount / cameraShakeStepFactor;
        cameraShakeDurationStep = cameraShakeDuration / cameraShakeStepFactor;

        // To load the next scene
        // LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text =
                            "Game Started: " + GameStarted + "\n" +
                            // "Game Ended: " + GameEnded + "\n" +
                            // "Game Paused: " + GamePaused + "\n" +
                            // "camStartPosTransition: " + camStartPosTransition + "\n" +
                            // "camStartFovTransition: " + camStartFovTransition + "\n" +
                            // "Cam Rot X: " + Camera.main.transform.rotation.eulerAngles.x + "\n" +
                            // "Time Scale: " + Time.timeScale + "\n" +
                            "playerMove: " + playerMove + "\n" +
                            "playerMoveUp: " + playerMoveUp + "\n" +
                            "playerMoveDown: " + playerMoveDown + "\n" +
                            "playerMoveLeft: " + playerMoveLeft + "\n" +
                            "playerMoveRight: " + playerMoveRight + "\n"
                            // "Scene Counter: " + sceneCounter + "\n" +
                            // "Next Scene Int: " + nextSceneInt + "\n" +
                            // "Temp Next Scene Int: " + tempNextSceneInt + "\n" +
                            // "Current Scene Int: " + currentSceneInt + "\n" +
                            ;

        // To Start the Game when the Screen is Tapped
        if (Input.GetMouseButton(0) || Input.GetKeyUp(KeyCode.Keypad5))
            // Signals the Game has started and only runs it once if the game has already started
            if (!GameManager.singleton.GameStarted)
                GameManager.singleton.CheckCameraTransitions();

        // To perform the Camera Transitions
        PerformCameraTransitions();

        // To Control the Visibility of some UI Elements
        UiElementsVisibility();

        // To Update the level indicator text with the corresponding level
        levelText.text = "Level " + sceneCounter;

        // TODO - To check if the current level has finished
        // if (!GameEnded)
        //     LevelProgress();
    }

    // Game Management

    public void StartGame()
    {
        GameStarted = true;
    }

    public void PauseOrResumeGame()
    {
        GamePaused = !GamePaused;

        pauseMenu.SetActive(GamePaused);

        if (GamePaused)
            StopTime();
        else
            StartTime();
    }

    private void EndGame(bool gameWon)
    {
        GameEnded = true;
        GameStarted = false;

        // TODO - Play Player Won Animation

        if (gameWon)
        {
            gameWonMenu.SetActive(true);

            // // To unload the current scene if the game has ended
            // if (GameEnded)
            //     SceneManager.UnloadSceneAsync(string.Concat(baseSceneName + " ", currentSceneInt + 1));

            // // Display Next Loaded Scene
            // LoadNextScene();
        }
        else
        {
            gameLostMenu.SetActive(true);
            StopTime();
        }
    }

    private void StopTime()
    {
        Time.timeScale = 0f;
    }

    private void StartTime()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Base Scene");

        StartTime();
    }

    public void QuitGame()
    {
        // Written to show as Application.Quit doesnt do anything in Editor
        // Debug.Log("Quit the Game !!!");

        Application.Quit();
    }

    private void UiElementsVisibility()
    {
        // To Hide the Slide to Move Text if not at the Starting Scene
        if (sceneCounter > 1 || GameStarted ||
            camStartFovTransition || camStartPosTransition || camStartRotTransition)
            tapToStyleTMP.SetActive(false);

        // To Hide the Control Buttons, Pause Button and Level Indicator Text when the Game is Paused, Won or Lost
        if (pauseMenu.activeSelf || gameWonMenu.activeSelf || gameLostMenu.activeSelf)
        {
            controlButtons.SetActive(false);
            pauseButton.SetActive(false);
            levelText.gameObject.SetActive(false);
        }
        else if (GameStarted)
        {
            controlButtons.SetActive(true);
            pauseButton.SetActive(true);
            levelText.gameObject.SetActive(true);
        }
    }

    // Effects

    private void CheckCameraTransitions()
    {
        // Camera FOV Transition before Starting the Game
        if (Camera.main.fieldOfView == camFovStart)
            camStartFovTransition = true;

        // Camera POS Transition before Starting the Game
        if (Camera.main.transform.position == camPosStart)
            camStartPosTransition = true;

        // Camera ROT Transition before Starting the Game
        if (Camera.main.transform.rotation.eulerAngles.x == camRotXStart)
            camStartRotTransition = true;

        // Starting the Game if the Camera has transitioned already
        if (camFovTransitionDone && camPosTransitionDone && camRotTransitionDone)
            if (!GameStarted)
                StartGame();
    }

    private void PerformCameraTransitions()
    {
        // Camera FOV Transition before Starting the Game
        if (camStartFovTransition)
        {
            if (Camera.main.fieldOfView > camFovEnd)
                Camera.main.fieldOfView -= camFovTransitionSpeed * Time.fixedDeltaTime;
            else
            {
                Camera.main.fieldOfView = camFovEnd;
                camStartFovTransition = false;
                camFovTransitionDone = true;
            }
        }

        // Camera POS Transition before Starting the Game
        if (camStartPosTransition)
        {
            // To reposition the Disc behind the Player when Caught
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                                                          camPosEnd,
                                                          camPosTransitionSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(Camera.main.transform.position, camPosEnd) < camPosRotAcptableRange)
            {
                camStartPosTransition = false;
                camPosTransitionDone = true;
            }
        }

        // Camera ROT Transition before Starting the Game
        if (camStartRotTransition)
        {
            // To reposition the Disc behind the Player when Caught
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,
                                                              Quaternion.Euler(camRotXEnd, 0, 0),
                                                              camRotTransitionSpeed * Time.fixedDeltaTime);

            if (Camera.main.transform.rotation.eulerAngles.x - camRotXEnd < camPosRotAcptableRange)
            {
                camStartRotTransition = false;
                camRotTransitionDone = true;
            }
        }

        if (camFovTransitionDone && camPosTransitionDone && camRotTransitionDone)
            if (!GameStarted)
                StartGame();
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
            Camera.main.transform.position = camPosEnd + (Random.insideUnitSphere * camShakeAmt);

            camShakeDuration -= cameraShakeDurationStep;
            camShakeAmt -= cameraShakeAmountStep;

            yield return null;
        }

        // To reset the Camera Position after Shaking the Camera
        Camera.main.transform.position = camPosEnd;
    }

    // Player Movement
    public void MovePlayerUp()
    {
        SetPlayerMoveUp(true);
    }

    public void MovePlayerDown()
    {
        SetPlayerMoveDown(true);
    }

    public void MovePlayerRight()
    {
        SetPlayerMoveRight(true);
    }

    public void MovePlayerLeft()
    {
        SetPlayerMoveLeft(true);
    }

    // Setters

    public void SetPlayerMove(bool status)
    {
        playerMove = status;
    }

    public void SetPlayerMoveUp(bool status)
    {
        playerMoveUp = status;
    }

    public void SetPlayerMoveDown(bool status)
    {
        playerMoveDown = status;
    }

    public void SetPlayerMoveRight(bool status)
    {
        playerMoveRight = status;
    }

    public void SetPlayerMoveLeft(bool status)
    {
        playerMoveLeft = status;
    }

    public void SetPlayer(GameObject gameObject)
    {
        Player = gameObject;
    }
}
