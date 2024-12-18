using System.Collections;
using System.Collections.Generic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SWPPT3.Main.UI
{
    public class InGameScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [SerializeField] private UnityEvent<bool> _onTryingPauseStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingLoadStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingFailStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingClearStatusChanged;
        [SerializeField] private UnityEvent<bool> _onTryingChoiceStatusChanged;

        [SerializeField] private TextMeshProUGUI _playTimeTmp;
        [SerializeField] private TextMeshProUGUI _metalNumTmp;
        [SerializeField] private TextMeshProUGUI _rubberNumTmp;

        [SerializeField] private GameObject _radialUI;
        [SerializeField] private RectTransform _rubberButton;
        [SerializeField] private RectTransform _metalButton;
        [SerializeField] private RectTransform _slimeButton;
        private Vector2 _screenCenter;


        public void ClickResume()
        {
            Cursor.visible = false;
            Debug.Log("click reusme");
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

        public void ChangeSlime()
        {
            _player.TryChangeState(PlayerStates.Slime);
        }

        public void ChangeMetal()
        {
            _player.TryChangeState(PlayerStates.Metal);
        }

        public void ChangeRubber()
        {
            _player.TryChangeState(PlayerStates.Rubber);
        }

        public void ShowRadialUI()
        {
            _radialUI.SetActive(true);
            if (_player.Item[PlayerStates.Rubber] == 0)
            {
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("RightButton/RightActive").gameObject.SetActive(true);
            }

            if (_player.Item[PlayerStates.Metal] == 0)
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(true);
            }
        }
        private void UpdateRadialScale()
        {
            UpdateButtonScale();
        }

        private void UpdateButtonScale()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 relativePos = Mouse.current.position.ReadValue() - screenCenter;

            if (relativePos.magnitude < 50f || relativePos.magnitude > 100f)
            {
                SetButtonScale(_slimeButton, 1.0f);
                SetButtonScale(_metalButton, 1.0f);
                SetButtonScale(_rubberButton, 1.0f);
            }
            else
            {
                float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;


                if (angle <=90 || angle > 330)
                {
                    SetButtonScale(_slimeButton, 1.5f);
                    SetButtonScale(_metalButton, 1.0f);
                    SetButtonScale(_rubberButton, 1.0f);
                }
                else if (angle >= 90 && angle < 210)
                {
                    SetButtonScale(_metalButton, 1.5f);
                    SetButtonScale(_rubberButton, 1.0f);
                    SetButtonScale(_slimeButton, 1.0f);
                }
                else
                {
                    SetButtonScale(_rubberButton, 1.5f);
                    SetButtonScale(_metalButton, 1.0f);
                    SetButtonScale(_slimeButton, 1.0f);
                }
            }
        }

        private void SetButtonScale(RectTransform button, float scale)
        {
            if (button.gameObject.activeSelf)
            {
                button.localScale = new Vector3(scale, scale, 1.0f);
            }
        }


        public void PlayTimeUpdate(int time){
            int min = time/60;
            int sec = time%60;
            _playTimeTmp.text = $"{min:D2}:{sec:D2}";
        }

        private void NumUpdate()
        {
            _metalNumTmp.text = $"{_player.Item[PlayerStates.Metal]}";
            _rubberNumTmp.text = $"{_player.Item[PlayerStates.Rubber]}";
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


        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnEsc += HandleEsc;
                InputManager.Instance.OnStartTransform += HandleTransform;
            }
            else
            {
                Debug.LogError("InputManager is null");
            }

            _player.OnItemChanged += NumUpdate;
            NumUpdate();

            _onTryingLoadStatusChanged.Invoke(true);

            _onTryingPauseStatusChanged.Invoke(false);
            _onTryingFailStatusChanged.Invoke(false);
            _onTryingClearStatusChanged.Invoke(false);
            _onTryingChoiceStatusChanged.Invoke(false);
        }

        // Update is called once per frame
        void Update()
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
        private void HandleEsc()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                Cursor.visible = true;
                GameManager.Instance.GameState = GameState.Paused;
                _onTryingPauseStatusChanged.Invoke(true);
            }
        }

        private void HandleTransform(bool isClick)
        {
            if (GameManager.Instance.GameState == GameState.Playing && isClick)
            {
                Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;

                // 커서를 화면 중앙으로 이동
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Mouse.current.WarpCursorPosition(screenCenter);

                GameManager.Instance.GameState = GameState.OnChoice;
                ShowRadialUI();
            }
            else if (GameManager.Instance.GameState == GameState.OnChoice && !isClick)
            {
                CheckRadial();
                Cursor.visible = false;
                GameManager.Instance.GameState = GameState.Playing;
                _onTryingChoiceStatusChanged.Invoke(false);
            }
        }

        private void CheckRadial()
        {
            Vector2 cursorPos = Mouse.current.position.ReadValue();
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 relativePos = cursorPos - screenCenter;
            if (relativePos.magnitude < 50f || relativePos.magnitude > 100f)
            {
                return;
            }
            else
            {

                float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
                if (angle < 0)
                {
                    angle += 360f;
                }
                Debug.Log(angle);

                if (angle <=90 || angle >330 )
                {
                    Debug.Log("atRubber");
                    _player.TryChangeState(PlayerStates.Rubber);
                }
                else if (angle >= 90 && angle < 210)
                {
                    Debug.Log("atMetal");
                    _player.TryChangeState(PlayerStates.Metal);
                }
                else
                {
                    Debug.Log("Slime");
                    _player.TryChangeState(PlayerStates.Slime);
                }
                Debug.Log("finish!");
            }

        }



        private void OnDestroy()
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
            Cursor.visible = true;
        }

    }
}
