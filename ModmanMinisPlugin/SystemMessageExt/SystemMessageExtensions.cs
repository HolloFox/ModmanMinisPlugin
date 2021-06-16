using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderMan.SystemMessageExt
{
    [SingletonResource("_SystemMessageExtension")]
    public class SystemMessageExtensions : SingletonBehaviour<SystemMessageExtensions>
    {
        [SerializeField]
        private SystemMessageTextField _textFieldMessage;
        [SerializeField]
        private SystemMessagePending _pendingMessage;
        [SerializeField]
        private OnScreenMessage _onscreenMessage;
        [SerializeField]
        private GameObject _stopAllInputObject;

        public static void AskForTextInput(
            string title,
            string message,
            string acceptBtnText,
            Action<string> onSubmit,
            Action onAccept,
            string cancelBtnText = "",
            Action onCancel = null,
            string inputString = "")
        {
            SingletonBehaviour<SystemMessageExtensions>.Instance.Open();
            SingletonBehaviour<SystemMessageExtensions>.Instance._textFieldMessage.SetupMessage(title, message, acceptBtnText, onSubmit, onAccept, cancelBtnText, onCancel, inputString);
        }

        protected override void Awake()
        {
            base.Awake();
            this._stopAllInputObject.SetActive(false);
        }

        public void Open()
        {
            this._stopAllInputObject.SetActive(true);
            BoardToolManager.EnableTools(false);
        }

        public void Close()
        {
            this._stopAllInputObject.SetActive(false);
            BoardToolManager.EnableTools(true);
        }
        public static void ClosePendingMessage() => SingletonBehaviour<SystemMessageExtensions>.Instance._pendingMessage.Close();
        public bool IsOpen => SingletonBehaviour<SystemMessageExtensions>.HasInstance && SingletonBehaviour<SystemMessageExtensions>.Instance.IsOpen;

        private void OnEnable()
        {
            GameSettings.OnUIScaleChange -= new Action<float>(this.UIScaleChange);
            GameSettings.OnUIScaleChange += new Action<float>(this.UIScaleChange);
            this.UIScaleChange(GameSettings.UIScale);
        }

        private void OnDisable() => GameSettings.OnUIScaleChange -= new Action<float>(this.UIScaleChange);

        private void UIScaleChange(float scale) => this.GetComponent<CanvasScaler>().scaleFactor = scale;

    }
}
