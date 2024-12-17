using System.Collections;
using System.Collections.Generic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingScreenBehavior : MonoBehaviour
{
    public Player _player;
    public GameObject _radialUI;

    private void Awake()
    {
        Cursor.visible = false;
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnStartTransform += HandleTransform;
        }
    }

    private void HandleTransform(bool isClick)
    {
        if (GameManager.Instance.GameState == GameState.Playing && isClick)
        {
            Debug.Log(GameManager.Instance.GameState);
            _radialUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 커서를 화면 중앙으로 이동
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Mouse.current.WarpCursorPosition(screenCenter);

            GameManager.Instance.GameState = GameState.OnChoice;
            ShowRadialUI();
        }
        else if (GameManager.Instance.GameState == GameState.OnChoice && !isClick)
        {
            checkRadial();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.Instance.GameState = GameState.Playing;
            HideRadialUI();
        }
    }

    private void checkRadial()
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

    public void HideRadialUI()
    {
        _radialUI.SetActive(false);
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

            if (_player.Item[PlayerStates.Metal] == 0)
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(false);
            }
            else
            {
                _radialUI.transform.Find("LeftButton/LeftActive").gameObject.SetActive(true);
            }
        }
    }
}
