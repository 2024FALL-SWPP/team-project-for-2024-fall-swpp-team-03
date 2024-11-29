using UnityEngine;
using UnityEngine.UI;
using SWPPT3.Main.Manager;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button nextStageButton;

    private void Start()
    {
        pauseButton.onClick.AddListener(() => OnButtonClicked("Pause"));
        resumeButton.onClick.AddListener(() => OnButtonClicked("Resume"));
        resetButton.onClick.AddListener(() => OnButtonClicked("Reset"));
        nextStageButton.onClick.AddListener(() => OnButtonClicked("NextStage"));
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnButtonClicked(string buttonName)
    {
        GameManager.Instance.OnUIButtonClicked(buttonName);
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
            case GameState.StageCleared:
                break;
            default:
                break;
        }
    }
}
