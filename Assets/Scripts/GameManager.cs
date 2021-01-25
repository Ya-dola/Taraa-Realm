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
    public bool GameWon { get; private set; }
    public bool GameLost { get; private set; }

    [Header("Player")]
    public Vector3 playerStartingPos;
    [Range(0, 48f)]
    public float playerPosMoveSpeed;
    [Range(0, 48f)]
    public float playerRotMoveSpeed;
    public float playerMoveFactor;
    public float playerPosRotMoveAcptableRange;
    public bool playerMove { get; private set; }
    public bool playerMoveUp { get; private set; }
    public bool playerMoveDown { get; private set; }
    public bool playerMoveRight { get; private set; }
    public bool playerMoveLeft { get; private set; }
    public bool playerMoveAnimPlayed { get; private set; }
    public float[] IdleAnimRange;
    public bool playerHit { get; private set; }
    public bool playerRecovered { get; private set; }
    public float playerRecoveryTime;
    public float playerRecoverySpeed;
    public float playerRecoveryTimeTemp { get; private set; }
    public GameObject Player { get; private set; }

    [Header("Enemy")]
    [Range(0, 48f)]
    public float enemyRotMoveSpeed;
    public List<GameObject> Enemies { get; private set; }
    public int enemiesCounter { get; private set; }

    [Header("Throwables")]
    public GameObject thrwPrefab;
    public GameObject thrwBrokenPrefab;
    public float baseThrwInstantiateDelay;
    public float gameThrwInstantiateDelay { get; private set; }
    public float thrwLaunchDelay;
    public float thrwLaunchLayerDelay;
    public float thrwYPos;
    public float[] thrwSizeScale;
    public float[] thrwSizeRange;
    [Range(0, 48f)]
    public float baseThrwSpeed;
    public float levelThrwSpeed { get; private set; }
    public float gameThrwSpeed { get; private set; }
    public float thrwPlayerHitDelay;
    public float thrwBrokenDelay;
    public float thrwExplosionForce;
    public float thrwExplosionRadius;
    public float thrwExplosionUpwardsMod;
    public AudioClip thrwBrokenSound;

    [Range(0, 1)]
    public float thrwBrokenSoundVolume;

    [Header("Gameplay")]
    public int baseBallCount;
    public int levelBallCount { get; private set; }
    public int gameBallCount { get; private set; }
    public float scoreDetectorRadius;
    public int baseDodgeScoreValue;
    public int gameDodgeScoreValue { get; private set; }
    public int dodgeScoreMultiplier;
    public float successiveDodgeFactor;
    public float successiveDodgeCounter { get; private set; }
    public int hitScoreValue;
    public int baseHitCount;
    public int gameHitCount { get; private set; }
    public int currentScore { get; private set; }

    [Header("Effects")]
    public float cameraShakeAmount;
    public float cameraShakeAmountStep { get; set; }
    public float cameraShakeDuration;
    public float cameraShakeDurationStep { get; set; }
    public float cameraShakeStepFactor;
    public float effectPsDestDelay;
    public GameObject ConfettiPs;
    public GameObject ExplosionPs;
    public float launchPsSpeed;

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
    public Material[] enemyMaterials;
    public Material[] enemyEffectsMaterials;

    [Header("Level Management")]
    public string levelSceneName;
    private int sceneCounter;
    private AsyncOperation sceneLoader;

    [Header("User Interface")]
    public GameObject startScreen;
    public TextMeshProUGUI scoreTMP;
    public GameObject ballCounter;
    public TextMeshProUGUI ballCounterText;
    public GameObject pauseMenu;
    public TextMeshProUGUI pauseLevelTMP;
    public TextMeshProUGUI pauseScoreTMP;
    public GameObject gameWonMenu;
    public TextMeshProUGUI gameWonLevelTMP;
    public TextMeshProUGUI gameWonScoreTMP;
    public GameObject gameLostMenu;
    public TextMeshProUGUI gameLostLevelTMP;
    public TextMeshProUGUI gameLostScoreTMP;
    public GameObject pauseButton;
    public GameObject controlButtons;
    public Button upButton;
    public Button downButton;
    public Button rightButton;
    public Button leftButton;
    public GameObject recoveryScreen;
    public TextMeshProUGUI playerRecoveringTMP;
    public TextMeshProUGUI hitsRemainingTMP;

    [Header("Debug")]
    public GameObject debugCanvas;
    public TextMeshProUGUI debugText;

    void Awake()
    {
        // Creates a Single Instance of the Game Manager through out the entire game
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        // To Start Level 1 as the Base Scene Loads
        SceneManager.LoadScene(levelSceneName, LoadSceneMode.Additive);

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

        Enemies = new List<GameObject>();
        enemiesCounter = 0;

        playerRecovered = true;

        currentScore = 0;

        // To load the next scene
        LoadNextScene();

        // To Lauch a Throwable at the Player
        StartCoroutine(InstantiateThrowable());
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
                            // "playerMove: " + playerMove + "\n" +
                            // "playerHit: " + playerHit + "\n" +
                            "playerRecovered: " + playerRecovered + "\n" +
                            "playerRecoveryTimeTemp: " + playerRecoveryTimeTemp + "\n" +
                            "gamethrwSpeed: " + gameThrwSpeed + "\n" +
                            "gameThrwInstantiateDelay: " + gameThrwInstantiateDelay + "\n"
                            // "playerMoveUp: " + playerMoveUp + "\n" +
                            // "playerMoveDown: " + playerMoveDown + "\n" +
                            // "playerMoveLeft: " + playerMoveLeft + "\n" +
                            // "playerMoveRight: " + playerMoveRight + "\n" +
                            // "Enemies Count: " + Enemies.Count + "\n"
                            // "Scene Counter: " + sceneCounter + "\n" +
                            // "Next Scene Int: " + nextSceneInt + "\n" +
                            // "Temp Next Scene Int: " + tempNextSceneInt + "\n" +
                            // "Current Scene Int: " + currentSceneInt + "\n" +
                            ;

        // To Show the Debug Canvas
        if (Input.GetKeyUp(KeyCode.Keypad0))
            debugCanvas.SetActive(!debugCanvas.activeSelf);

        // To Start the Game when the Screen is Tapped
        if (Input.GetMouseButton(0) || Input.GetKeyUp(KeyCode.Keypad5))
            // Signals the Game has started and only runs it once if the game has already started
            if (!GameStarted)
                CheckCameraTransitions();

        // To perform the Camera Transitions
        PerformCameraTransitions();

        // To Control the Visibility of some UI Elements
        UiElementsVisibility();

        // To Update Gameplay Elements within the Level
        UpdateGameplayElements();

        // To Check if the Current Level Progress
        CurrentGameProgress();

        // TODO - To check if the current level has finished
        // if (!GameEnded)
        //     LevelProgress();
    }

    // Game Management

    public void StartGame()
    {
        GameStarted = true;

        // Assigning the Default Game Values

        // Ball Count for Level
        levelBallCount = baseBallCount + Mathf.RoundToInt(sceneCounter * 4.269f);
        gameBallCount = levelBallCount;

        // Thrw Speed for Level
        levelThrwSpeed = baseThrwSpeed + (sceneCounter * 0.125f);
        gameThrwSpeed = levelThrwSpeed;

        // Delay between Spawning Thrws
        gameThrwInstantiateDelay = baseThrwInstantiateDelay - (sceneCounter * 0.01125f);

        // Base Score Per Dodge
        gameDodgeScoreValue = baseDodgeScoreValue;

        // Hits Player can take per Level
        gameHitCount = baseHitCount + Mathf.RoundToInt(sceneCounter * 0.5f);

        // To Reset Player Position
        playerMove = false;
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

        if (gameWon)
        {
            GameWon = true;
            gameWonMenu.SetActive(true);
            // StopTime();
        }
        else
        {
            GameLost = true;
            gameLostMenu.SetActive(true);
            // StopTime();
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

    public void NextLevel()
    {
        // To Unload the current scene
        SceneManager.UnloadSceneAsync(levelSceneName);

        // To Hide the Game Won Menu
        gameWonMenu.SetActive(false);

        // To Clear the State of the next loaded Game
        GameWon = false;
        GameLost = false;

        // To Reset the Score for the Level
        currentScore = 0;

        // Display Next Loaded Scene
        LoadNextScene();
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

    // To Check if the Current Level Progress
    private void CurrentGameProgress()
    {
        if (GameStarted && gameBallCount <= 0)
            EndGame(true);

        if (GameStarted && gameHitCount <= 0)
            EndGame(false);
    }

    private void UiElementsVisibility()
    {
        // To Hide the Start Screen if not at the Starting Scene
        if (sceneCounter > 1 || GameStarted ||
            camStartFovTransition || camStartPosTransition || camStartRotTransition)
            startScreen.SetActive(false);

        // To Hide the Control Buttons, Pause Button and Level Indicator Text when the Game is Paused, Won or Lost
        if (pauseMenu.activeSelf || gameWonMenu.activeSelf || gameLostMenu.activeSelf)
        {
            controlButtons.SetActive(false);
            pauseButton.SetActive(false);
            scoreTMP.gameObject.SetActive(false);
            ballCounter.gameObject.SetActive(false);
            recoveryScreen.SetActive(false);
        }
        else if (GameStarted)
        {
            pauseButton.SetActive(true);
            scoreTMP.gameObject.SetActive(true);
            ballCounter.gameObject.SetActive(true);

            // To Hide the Control Buttons and Show the Recovery Screen when the Player is Recovering
            if (!playerRecovered)
            {
                controlButtons.SetActive(false);
                recoveryScreen.SetActive(true);
            }
            else
            {
                controlButtons.SetActive(true);
                recoveryScreen.SetActive(false);
            }
        }
    }

    // To Update Gameplay Elements within the Level
    private void UpdateGameplayElements()
    {
        // To Update the Score indicator text with the corresponding score
        scoreTMP.text = "" + currentScore;

        // To Update the Ball Counter text with the game ball count
        ballCounterText.text = "" + gameBallCount;

        // To Update the Pause Menu indicators
        if (GamePaused)
        {
            pauseLevelTMP.text = "Level " + sceneCounter;
            pauseScoreTMP.text = "Score: " + currentScore;
        }

        // To Update the Game Won Menu indicators
        if (GameWon)
        {
            gameWonLevelTMP.text = "Level " + sceneCounter;
            gameWonScoreTMP.text = "Score: " + currentScore;
        }

        // To Update the Game Lost Menu indicators
        if (GameLost)
        {
            gameLostLevelTMP.text = "Level " + sceneCounter;
            gameLostScoreTMP.text = "Score: " + currentScore;
        }

        // To Update the Player Recovery Screen
        if (!playerRecovered)
        {
            // To Update the Hits Remaining text with the game hit count
            hitsRemainingTMP.text = "Hits Remaining \n" + gameHitCount;

            // To Update the Player Recovery Time
            playerRecoveringTMP.text = "Player Recovering \n" + playerRecoveryTimeTemp.ToString("F2") + "s";
        }

        // To increase the speed of the Thrwoables when the Remaining Balls Reduces
        if (gameBallCount > (levelBallCount * 0.5f))
            gameThrwSpeed = baseThrwSpeed;
        else if (gameBallCount > (levelBallCount * 0.25f))
            gameThrwSpeed = levelThrwSpeed * 1.125f;
        else
            gameThrwSpeed = levelThrwSpeed * 1.25f;
    }

    private void LoadNextScene()
    {
        // To Load the Scene that was loaded in the background
        if (sceneCounter != 0)
            sceneLoader.allowSceneActivation = true;

        // To allow the player to play the loaded level/scene
        GameEnded = false;
        GameStarted = false;

        // To Initialise the Game Objects References of Loaded Scene according to their tags
        InitialiseGameObjectsRef();

        // To Initialise the Game Objects Materials according to their tag
        InitialiseGameObjMaterials();

        // To Determine the next scene's number
        sceneCounter++;

        // To load the next scene in the background
        BackgroundLoadNextScene();
    }

    // To load the next scene in the background
    private void BackgroundLoadNextScene()
    {
        sceneLoader = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);
        sceneLoader.allowSceneActivation = false;
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
            // To reposition the Camera Positon as the Game Starts
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
            // To reposition the Camera Rotation as the Game Starts
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,
                                                              Quaternion.Euler(camRotXEnd, 0, 0),
                                                              camRotTransitionSpeed * Time.fixedDeltaTime);

            if (Camera.main.transform.rotation.eulerAngles.x - camRotXEnd < camPosRotAcptableRange)
            {
                camStartRotTransition = false;
                camRotTransitionDone = true;
            }
        }

        // To Start the Game once the transitions have finished
        if (camFovTransitionDone && camPosTransitionDone && camRotTransitionDone)
            if (!GameStarted)
                StartGame();
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

    // Throwable Logic

    private IEnumerator InstantiateThrowable()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameThrwInstantiateDelay);

            // To work only when the Game has started or has not ended or is not paused
            if (!GameStarted || GameEnded || GamePaused)
                continue;

            // To launch only when Player is not Hit
            if (playerRecovered)
            {
                // To determine which enemy will lauch the determined throwable
                enemiesCounter = Random.Range(0, Enemies.Count);

                GameObject throwable = Instantiate(thrwPrefab,
                                                   new Vector3(Enemies[enemiesCounter].transform.position.x,
                                                   thrwYPos,
                                                   Enemies[enemiesCounter].transform.position.z),
                                                   Quaternion.identity);

                // To assign the Material of the Chosen Enemy to the Throwable
                throwable.GetComponent<Renderer>().material = Enemies[enemiesCounter].GetComponentInChildren<Renderer>().material;
            }
        }
    }

    public void AddThrwDodgeScore()
    {
        currentScore += gameDodgeScoreValue;
    }

    public void SubThrwHitScore()
    {
        var newScore = currentScore - hitScoreValue;

        if (newScore < 0)
            currentScore = 0;
        else
            currentScore = newScore;
    }

    public void AddSuccessiveDodgeCounter()
    {
        successiveDodgeCounter += 1;
    }

    public void ResetSuccessiveDodgeCounter()
    {
        successiveDodgeCounter = 0;
        gameDodgeScoreValue = baseDodgeScoreValue;
    }

    public void ApplyDodgeValueMultiplier()
    {
        gameDodgeScoreValue *= dodgeScoreMultiplier;
    }

    public void ReduceHitCount()
    {
        gameHitCount -= 1;
    }

    // Game Play Objects

    private void InitialiseGameObjectsRef()
    {
        // To ensure player and enemies are being reset for each level when progressing from level to another
        Player = null;
        Enemies.Clear();

        // Setting Up the Player
        if (GameObject.FindGameObjectWithTag("Player") != null)
            SetPlayer(GameObject.FindGameObjectWithTag("Player"));

        Player.transform.position = playerStartingPos;

        // Setting Up the Enemies
        foreach (GameObject enemyGo in GameObject.FindGameObjectsWithTag("Enemy"))
            Enemies.Add(enemyGo);
    }

    private void InitialiseGameObjMaterials()
    {
        // Setting Up the Player Material
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Renderer>().material = playerMaterial;

        // Setting Up the Enemies Materials
        foreach (GameObject enemyGo in Enemies)
            enemyGo.GetComponentInChildren<Renderer>().material = enemyMaterials[Random.Range(0, enemyMaterials.Length)];
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

    public void SetPlayerMoveAnimPlayed(bool status)
    {
        playerMoveAnimPlayed = status;
    }

    public void SetPlayerHit(bool status)
    {
        playerHit = status;
    }

    public void SetPlayerRecovered(bool status)
    {
        playerRecovered = status;
    }

    public void SetPlayerRecoveryTimeTemp(float amount)
    {
        playerRecoveryTimeTemp = amount;
    }

    public void SetGameBallCount(int amount)
    {
        gameBallCount = amount;
    }

    public void SetPlayer(GameObject gameObject)
    {
        Player = gameObject;
    }
}
