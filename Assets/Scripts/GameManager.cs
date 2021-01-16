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
    public GameObject Player { get; set; }

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
    public float camFovTransitionSpeed;
    private bool camStartTransition;
    private Vector3 cameraPosition;

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

    // [Header("Level Management")]
    // public string baseSceneName;
    // private int sceneCounter;
    // private int nextSceneInt;
    // private int tempNextSceneInt;
    // private int currentSceneInt;
    // private AsyncOperation sceneLoader;

    [Header("Menus")]
    public GameObject tapToStyleTMP;
    public TextMeshProUGUI levelText;
    public GameObject pauseMenu;
    public GameObject gameWonMenu;
    public GameObject gameLostMenu;
    public GameObject pauseButton;
    public GameObject controlButtons;

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

    }

    // Update is called once per frame
    void Update()
    {

    }
}
