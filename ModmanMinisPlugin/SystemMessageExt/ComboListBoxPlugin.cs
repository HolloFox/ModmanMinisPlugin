using System;
using System.Reflection;
using BepInEx;
using SRF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ThunderMan.SystemMessageExt
{
    public enum ComboType
    {
        List,
        DropDown,
        CategoryList
    }


    [BepInPlugin(Guid, Name, Version)]

    public class ComboListBoxPlugin : BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.ComboListBoxPlugin";
        private const string Version = "1.0.0.0";
        private const string Name = "HolloFoxes' Combo List Box Plug-In";

        // Actions
        private static Action<string[]> _onSubmit;
        private static Action<string[]> _onAccept;
        private static Action<string[]> _onCancel;
        private static Action<string[]> _onPreview;
        private static Action _removePreview;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log($"{Name} is Active.");
        }


        private bool _first = true;
        private static GameObject _stopInput;
        private static Transform _drop;
        private static Dropdown _categories;

        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {
            if (_first && SystemMessage.HasInstance)
            {
                try
                {
                    var instance = SystemMessage.Instance;
                    Debug.Log(instance.gameObject.name);
                    _stopInput = instance.transform.GetChild(0).gameObject;
                    var text = _stopInput.transform.GetChild(1);

                    _drop = GameObject.Instantiate(text);
                    _drop.name = "DropDownPanel";
                    _drop.transform.parent = _stopInput.transform;
                    _drop.localPosition = Vector3.zero;

                    // Adjust anchor
                    var rect = _drop.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.25f, 0.25f);
                    rect.anchorMax = new Vector2(0.3f, 0.75f);

                    // Work on Drop Down
                    var inputText = _drop.transform.GetChild(3);
                    
                    var dropdown = GameObject.Instantiate(inputText.gameObject);
                    dropdown.name = "TextMeshPro - DropdownField";
                    dropdown.transform.parent = _drop.transform;
                    dropdown.transform.localPosition = inputText.transform.localPosition;
                    dropdown.GetComponent<RectTransform>().offsetMin = new Vector2(10, -130);
                    dropdown.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -100);
                    dropdown.transform.DestroyChildren();
                    _categories = dropdown.AddComponent<Dropdown>();

                    var x = new ScrollView(ScrollViewMode.Vertical);
                    //x.contentViewport.cont

                    _categories.options.AddRange(new Dropdown.OptionData[]
                    {
                        new Dropdown.OptionData("1"),
                        new Dropdown.OptionData("2"),
                        new Dropdown.OptionData("3"),
                    });

                    // Define callbacks for Buttons
                    SetCallbacks();
                }
                catch (Exception e)
                {
                    Debug.Log("Failed to instantiate modified");
                    Debug.LogError($"Error: {e}");
                }

                _first = false;
            }
        }

        private static void SetTitle(string title)
        {
            var text = _drop.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            text.SetText(title);
        }

        private static void SetMessage(string message)
        {
            var text = _drop.GetChild(1).GetComponent<TextMeshProUGUI>();
            text.SetText(message);
        }

        private static void SetButtonsText(string acceptBtnText, string cancelBtnText)
        {
            // Find
            var buttonPanel = _drop.GetChild(2);
            var btnOk = buttonPanel.GetChild(0);
            var btnCancel = buttonPanel.GetChild(1);

            // Set Text
            btnOk.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(acceptBtnText);
            btnCancel.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(cancelBtnText);
        }

        // Adjust callbacks so we can use string[] Instead
        private static void SetCallbacks()
        {
            // Find
            var buttonPanel = _drop.GetChild(2);
            var btnOk = buttonPanel.GetChild(0);
            var btnCancel = buttonPanel.GetChild(1);

            var btnOkComponent = btnOk.GetComponent<UnityEngine.UI.Button>();
            var btnCancelComponent = btnCancel.GetComponent<UnityEngine.UI.Button>();

            // Probably need to make this static on load and not dynamic
            btnOkComponent.onClick.RemoveAllListeners();
            btnOkComponent.onClick.AddListener(BtnOkClicked);

            // Don't need to remove the existing feature that allows it to auto close
            btnCancelComponent.onClick.RemoveAllListeners();
            btnCancelComponent.onClick.AddListener(BtnCancelClicked);
        }

        private static void BtnOkClicked()
        {
            Debug.Log("BtnOkClicked Called");
            ClosePrompt();
            
            // Callback
            var selected = Selected();
            _removePreview.Invoke();
            _onSubmit?.Invoke(selected);
            _onAccept?.Invoke(selected);

            CleanUp();
        }

        private static void BtnCancelClicked()
        {
            Debug.Log("BtnCancelClicked Called");
            ClosePrompt();

            // Callback
            var selected = Selected();
            _removePreview.Invoke();
            _onSubmit?.Invoke(selected);
            _onCancel?.Invoke(selected);

            CleanUp();
        }

        private static void CleanUp()
        {
            // Remove Binds
            _onAccept = null;
            _onCancel = null;
            _onSubmit = null;
            _onPreview = null;
            _removePreview = null;

            // Revert to original size
            var rect = _drop.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.25f, 0.25f);
            rect.anchorMax = new Vector2(0.3f, 0.75f);
        }

        private static string[] Selected()
        {
            return new string[] { };
        }

        private static void ClosePrompt()
        {
            _drop.gameObject.SetActive(false);
            _stopInput.SetActive(false);
        }

        public static void AskForSelectedInputs(
            string title,
            string message,
            string acceptBtnText,
            Action<string[]> onAccept,
            string[] options,
            ComboType promptType = ComboType.CategoryList,
            string cancelBtnText = "Cancel",
            Action<string[]> onCancel = null,
            Action<string[]> onSubmit = null,
            Action<string[]> onPreview = null,
            Action removePreview = null,
            RectTransform customSize = null
        )
        {
            // Configure
            SetTitle(title);
            SetMessage(message);
            SetButtonsText(acceptBtnText,cancelBtnText);

            // Adjust anchor
            if (customSize != null)
            {
                var rect = _drop.GetComponent<RectTransform>();
                rect.anchorMin = customSize.anchorMin;
                rect.anchorMax = customSize.anchorMax;
            }

            // Load Options
            foreach (var o in options)
            {

            }

            // Bind Events
            _onSubmit = onSubmit;
            _onAccept = onAccept;
            _onCancel = onCancel;
            _onPreview = onPreview;
            _removePreview = removePreview;

            // Render
            _stopInput.SetActive(true);
            _drop.gameObject.SetActive(true);
        }
    }
}
