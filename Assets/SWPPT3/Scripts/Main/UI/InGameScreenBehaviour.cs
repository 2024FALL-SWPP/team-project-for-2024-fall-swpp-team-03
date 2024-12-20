using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.UI
{
    public class InGameScreenBehaviour : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Player _player;
        [SerializeField] private PlayerScript _playerScript;
        [SerializeField] private GameObject _radialUI;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _playTimeTmp;
        [SerializeField] private TextMeshProUGUI _metalNumTmp;
        [SerializeField] private TextMeshProUGUI _rubberNumTmp;
        [SerializeField] private TextMeshProUGUI _stageTmp;
        [SerializeField] private RectTransform _rubberButton;
        [SerializeField] private RectTransform _metalButton;
        [SerializeField] private RectTransform _slimeButton;
        [SerializeField] private RectTransform _rubberHover;
        [SerializeField] private RectTransform _metalHover;
        [SerializeField] private RectTransform _slimeHover;

        [Header("Audio")]
        [SerializeField] private AudioSource _escAudio;
        [SerializeField] private AudioSource _radialAudio;

        [Header("Events")]
        [SerializeField] private UnityEvent<bool> _onTryingPauseStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingLoadStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingFailStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingClearStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingChoiceStatusChanged;

        private float MinRadialDistance;

        private void Start()
        {
            InitializeAudio();
            InitializeUI();
            SubscribeEvents();
            MinRadialDistance = _playerScript.MinRadial;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void InitializeAudio()
        {
            _escAudio.volume = BgmManager.Instance.SFXVolume;
            _radialAudio.volume = BgmManager.Instance.SFXVolume;
        }

        private void InitializeUI()
        {
            Cursor.visible = false;
            UpdateStageText();
            NumUpdate();
        }

        private void SubscribeEvents()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
                InputManager.Instance.OnStartTransform += HandleTransform;
            }

            if (_player != null)
            {
                _player.OnItemChanged += NumUpdate;
            }

            ResetUIEvents();
        }

        private void UnsubscribeEvents()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc -= HandleEsc;
                InputManager.Instance.OnStartTransform -= HandleTransform;
            }

            if (_player != null)
            {
                _player.OnItemChanged -= NumUpdate;
            }
        }

        private void ResetUIEvents()
        {
            _onTryingLoadStatusChanged.Invoke(true);
            _onTryingPauseStatusChanged.Invoke(false);
            _onTryingFailStatusChanged.Invoke(false);
            _onTryingClearStatusChanged.Invoke(false);
            _onTryingChoiceStatusChanged.Invoke(false);
        }

        private void UpdateStageText()
        {
            int stageNumber = GameManager.Instance.StageNumber;
            _stageTmp.text = stageNumber < 2 ? $"T{stageNumber + 1}" : $"S{stageNumber - 1}";
        }

        private void Update()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                _onTryingLoadStatusChanged.Invoke(false);
            }

            if (GameManager.Instance.GameState == GameState.OnChoice)
            {
                UpdateRadialScale();
            }
        }

        private void UpdateRadialScale()
        {
            Vector2 relativePos = Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2f, Screen.height / 2f);

            if (relativePos.magnitude < MinRadialDistance)
            {
                DeactivateHoverUI();
            }
            else
            {
                ActivateHoverUI(relativePos);
            }
        }

        private void DeactivateHoverUI()
        {
            _slimeHover.gameObject.SetActive(false);
            _metalHover.gameObject.SetActive(false);
            _rubberHover.gameObject.SetActive(false);
        }

        private void ActivateHoverUI(Vector2 relativePos)
        {
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            DeactivateHoverUI();

            if (angle <= 90 || angle > 330)
            {
                _rubberHover.gameObject.SetActive(_rubberButton.gameObject.activeSelf);
            }
            else if (angle >= 90 && angle < 210)
            {
                _metalHover.gameObject.SetActive(_metalButton.gameObject.activeSelf);
            }
            else
            {
                _slimeHover.gameObject.SetActive(_slimeButton.gameObject.activeSelf);
            }
        }

        public void PlayTimeUpdate(int time)
        {
            _playTimeTmp.text = $"{time / 60:D2}:{time % 60:D2}";
        }

        private void NumUpdate()
        {
            _metalNumTmp.text = $"{_player.Item[PlayerStates.Metal]}";
            _rubberNumTmp.text = $"{_player.Item[PlayerStates.Rubber]}";
        }

        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                _escAudio.Play();
                Cursor.visible = true;
                GameManager.Instance.GameState = GameState.Paused;
                _onTryingPauseStatusChanged.Invoke(true);
            }
        }

        private void HandleTransform(bool isClick)
        {
            if (GameManager.Instance.GameState == GameState.Playing && isClick)
            {
                EnterRadialUI();
            }
            else if (GameManager.Instance.GameState == GameState.OnChoice && !isClick)
            {
                ExitRadialUI();
            }
        }

        private void EnterRadialUI()
        {
            _radialAudio.Play();
            Cursor.visible = true;

            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));

            GameManager.Instance.GameState = GameState.OnChoice;
            ShowRadialUI();
        }

        private void ExitRadialUI()
        {
            CheckRadial();
            Cursor.visible = false;
            GameManager.Instance.GameState = GameState.Playing;
            _onTryingChoiceStatusChanged.Invoke(false);
        }

        private void ShowRadialUI()
        {
            _radialUI.SetActive(true);
            UpdateRadialButtons();
        }

        private void UpdateRadialButtons()
        {
            _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(_player.Item[PlayerStates.Rubber] > 0);
            _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(_player.Item[PlayerStates.Metal] > 0);
        }

        private void CheckRadial()
        {
            Vector2 relativePos = Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2f, Screen.height / 2f);

            if (relativePos.magnitude < MinRadialDistance) return;

            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            if (angle <= 90 || angle > 330)
                _player.TryChangeState(PlayerStates.Rubber);
            else if (angle >= 90 && angle < 210)
                _player.TryChangeState(PlayerStates.Metal);
            else
                _player.TryChangeState(PlayerStates.Slime);
        }

        public void ShowFail()
        {
            _onTryingFailStatusChanged.Invoke(true);
            Cursor.visible = true;
        }

        public void ShowSuccess()
        {
            _onTryingClearStatusChanged.Invoke(true);
            Cursor.visible = true;
        }

        public void ClickResume()
        {
            Cursor.visible = false;
            GameManager.Instance.GameState = GameState.Playing;
            _onTryingPauseStatusChanged.Invoke(false);
        }

        public void ClickRestart()
        {
            GameManager.Instance.GameState = GameState.Ready;
        }

        public void ClickMainMenu()
        {
            GameManager.Instance.GameState = GameState.BeforeStart;
            GameManager.Instance.StageNumber = 0;
            SceneManager.LoadScene("Start");
        }

        public void ClickNext()
        {
            if (GameManager.Instance.StageNumber != 6)
            {
                GameManager.Instance.StageNumber++;
                GameManager.Instance.GameState = GameState.Ready;
            }
        }
    }
}
