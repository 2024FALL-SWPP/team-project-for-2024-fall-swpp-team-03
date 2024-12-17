using System.Collections;
using System.Collections.Generic;
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
            Debug.Log($"{_player.Item[PlayerStates.Rubber]} {_player.Item[PlayerStates.Metal]}");
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
                Debug.Log(GameManager.Instance.GameState+"hi");
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
                Debug.Log(GameManager.Instance.GameState+"bi");
                CheckRadial();
                Cursor.visible = false;
                // Cursor.lockState = CursorLockMode.Locked;
                GameManager.Instance.GameState = GameState.Playing;
                _onTryingChoiceStatusChanged.Invoke(false);
            }
        }

        private void CheckRadial()
        {
            Vector2 cursorPos = Mouse.current.position.ReadValue();
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 relativePos = cursorPos - screenCenter;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 360f;
            }

            if (angle >= 0 && angle < 120)
            {
                _player.TryChangeState(PlayerStates.Rubber);
            }
            else if (angle >= 120 && angle < 240)
            {
                _player.TryChangeState(PlayerStates.Metal);
            }
            else
            {
                _player.TryChangeState(PlayerStates.Slime);
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
