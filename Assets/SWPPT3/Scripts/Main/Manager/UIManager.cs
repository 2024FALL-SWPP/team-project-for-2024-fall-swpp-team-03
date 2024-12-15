using UnityEngine;
using UnityEngine.UI;
using SWPPT3.Main.Manager;
using SWPPT3.Main.Utility.Singleton;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    private GameManager _gameManager;
    [Header("Canvases")]
    [SerializeField] private Canvas beforeCanvas;
    [SerializeField] private Canvas playingCanvas;
    [SerializeField] private Canvas pausedCanvas;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Canvas stageClearedCanvas;
    [SerializeField] private Canvas beforeOptionCanvas;
    [SerializeField] private Canvas startIntroCanvas;
    [SerializeField] private Canvas clearIntroCanvas;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button finishStageButton;

    [Header("Stage Buttons")]
    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;
    [SerializeField] private Button stage4Button;
    [SerializeField] private Button stage5Button;
    [SerializeField] private Button tutorial1Button;
    [SerializeField] private Button tutorial2Button;

    private float introDuration = 1.0f;

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;

        _gameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        InitializeButtons();
        HideAllCanvases();
    }

    private void InitializeButtons()
    {
        pauseButton.onClick.AddListener(() => OnButtonClicked("Pause"));
        resumeButton.onClick.AddListener(() => OnButtonClicked("Resume"));
        resetButton.onClick.AddListener(() => OnButtonClicked("Reset"));
        nextStageButton.onClick.AddListener(() => OnButtonClicked("NextStage"));
        finishStageButton.onClick.AddListener(() => OnButtonClicked("Finish"));
        stage1Button.onClick.AddListener(() => OnButtonClicked(1));
        stage2Button.onClick.AddListener(() => OnButtonClicked(2));
        stage3Button.onClick.AddListener(() => OnButtonClicked(3));
        stage4Button.onClick.AddListener(() => OnButtonClicked(4));
        stage5Button.onClick.AddListener(() => OnButtonClicked(5));
        tutorial1Button.onClick.AddListener(() => OnButtonClicked(6));
        tutorial2Button.onClick.AddListener(() => OnButtonClicked(7));
    }

    public void ShowCanvas(string canvasName)
    {
        HideAllCanvases();

        switch (canvasName)
        {
            case "BeforeStart":
                beforeCanvas.gameObject.SetActive(true);
                break;
            case "Playing":
                playingCanvas.gameObject.SetActive(true);
                break;
            case "Paused":
                pausedCanvas.gameObject.SetActive(true);
                break;
            case "GameOver":
                gameOverCanvas.gameObject.SetActive(true);
                break;
            case "StageCleared":
                stageClearedCanvas.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown canvas name: " + canvasName);
                break;
        }
    }
    private void HideAllCanvases()
    {
        beforeCanvas.gameObject.SetActive(false);
        playingCanvas.gameObject.SetActive(false);
        pausedCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        stageClearedCanvas.gameObject.SetActive(false);
        beforeOptionCanvas.gameObject.SetActive(false);
    }

    // private void OnEnable()
    // {
    //     GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    // }
    //
    // private void OnDisable()
    // {
    //     GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    // }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnButtonClicked(string buttonName)
    {
        GameManager.Instance.OnUIButtonClicked(buttonName);
    }

    public void OnButtonClicked(int stageNum)
    {
        //Debug.Log("UIManager button clicked");
        GameManager.Instance.OnUIButtonClicked(stageNum);
    }

    private void OnGameStateChanged(GameState newState)
    {
        StopAllCoroutines();
        switch (newState)
        {
            case GameState.BeforeStart:
                ShowCanvas("BeforeStart");
                break;
            case GameState.Playing:
                StartCoroutine(ShowIntroAndMainCanvas(startIntroCanvas, "Playing"));
                break;
            case GameState.Paused:
                ShowCanvas("Paused");
                break;
            case GameState.GameOver:
                ShowCanvas("GameOver");
                break;
            case GameState.StageCleared:
                StartCoroutine(ShowIntroAndMainCanvas(clearIntroCanvas, "Clear"));
                break;
            default:
                break;
        }

    }
    private IEnumerator ShowIntroAndMainCanvas(Canvas introCanvas, string mainCanvasName)
    {
        introCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(introDuration);
        introCanvas.gameObject.SetActive(false);
        ShowCanvas(mainCanvasName);
    }
}
