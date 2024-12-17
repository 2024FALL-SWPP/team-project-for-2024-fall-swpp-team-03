using System;
using SWPPT3.Main.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace SWPPT3.Main.UI
{
    public enum ButtonClickType
    {
        Tutorial1 = 0,
        Tutorial2,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Option,
    }

    public class TitleScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> _onTryingExitStatusChanged;

        public void OnButtonClick(int type)
        {
            OnButtonClick((ButtonClickType) type);
        }

        public void OnButtonClick(ButtonClickType type)
        {
            switch (type)
            {
                case ButtonClickType.Tutorial1:
                    UIManager.Instance.();
                    break;
                case ButtonClickType.Tutorial2:
                    break;
                case ButtonClickType.Stage1:
                    break;
                case ButtonClickType.Stage2:
                    break;
                case ButtonClickType.Stage3:
                    break;
                case ButtonClickType.Stage4:
                    break;
                case ButtonClickType.Stage5:
                    break;
                case ButtonClickType.Option:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void Start()
        {
            _onTryingExitStatusChanged.Invoke(false);
        }

        private void OnDestroy()
        {
        }
    }
}
