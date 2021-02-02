﻿using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.GameModes;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.Constructor;
using Kingmaker.UI.Log;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InventoryTweaks.Utilities;

namespace InventoryTweaks.UI
{
    class ContainersUIManager : MonoBehaviour
    {
        public ContainerTypeUIManager ScrollManagerUI { get; private set; }
        public ContainerTypeUIManager PotionManagerUI { get; private set; }
        public ContainerTypeUIManager WandManagerUI { get; private set; }
        private CanvasGroup _canvasGroup;
        private GameObject _togglePanel;
        private ButtonWrapper _buttonScrolls;
        private ButtonWrapper _buttonWands;
        private ButtonWrapper _buttonPotions;
        private ButtonWrapper _buttonTrash;
        private static float _menu_step = .001f;
        private static GameObject hud; 

        public static ContainersUIManager CreateObject()
        {
            Main.Mod.Debug(MethodBase.GetCurrentMethod());
            UICommon uiCommon = Game.Instance.UI.Common;
            hud = uiCommon?.transform.Find("HUDLayout")?.gameObject;
            GameObject tooglePanel = uiCommon?.transform.Find("HUDLayout/CombatLog/TooglePanel")?.gameObject;

            if (!tooglePanel || !hud)
                return null;

            GameObject containers = new GameObject("TweakContainers", typeof(RectTransform), typeof(CanvasGroup));
            containers.transform.SetParent(hud.transform);
            containers.transform.SetSiblingIndex(0);

            RectTransform rectTweakContainer = (RectTransform)containers.transform;

            rectTweakContainer.anchorMin = new Vector2(0.0f, 0.0f);
            rectTweakContainer.anchorMax = new Vector2(0.0f, 0.0f);
            rectTweakContainer.pivot = new Vector2(0.0f, 0.0f);
            rectTweakContainer.rotation = Quaternion.identity;
            rectTweakContainer.localScale = new Vector3(.8f, .8f, .8f);

            //initialize buttons
            GameObject togglePanel = Instantiate(tooglePanel, containers.transform, false);
            togglePanel.name = "TweakTogglePanel";
            RectTransform rectButton = (RectTransform)togglePanel.transform;
            rectButton.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            rectButton.anchorMin = new Vector2(0f, 1f);
            rectButton.anchorMax = new Vector2(0f, 1f);
            rectButton.pivot = new Vector2(0f, 0f);
            rectButton.localPosition = new Vector3(0 - rectButton.rect.width * 1.08f, 0 - rectTweakContainer.rect.yMax * 6.85f, 0);
            rectButton.localScale = new Vector3(1.04f, 1f, 1f);

            rectButton.rotation = Quaternion.identity;
            DestroyImmediate(togglePanel.GetComponent<LogToggleManager>());
            DestroyImmediate(togglePanel.GetComponent<Image>());

            void setToggleButtons(GameObject button, string name)
            {
                button.name = name;
                DestroyImmediate(button.GetComponent<Toggle>());
                button.AddComponent<ButtonPF>();
            }

            GameObject toggleScrolls = togglePanel.transform.Find("ToogleAll").gameObject;
            setToggleButtons(toggleScrolls, "Scrolls");
            GameObject togglePotions = togglePanel.transform.Find("ToogleLifeEvent").gameObject;
            setToggleButtons(togglePotions, "Potions");
            GameObject toggleWands = togglePanel.transform.Find("ToogleCombat").gameObject;
            setToggleButtons(toggleWands, "Wands");
            GameObject toggleTrash = togglePanel.transform.Find("ToogleDialogue").gameObject;
            DestroyImmediate(toggleTrash); //remove in future developement
            //setToggleButtons(toggleTrash, "Trash");

            return containers.AddComponent<ContainersUIManager>();
        }

        void Awake()
        {
            Main.Mod.Debug(MethodBase.GetCurrentMethod());

            gameObject.transform.position = SetMenuPosition(SettingsWrapper.Menu_X, SettingsWrapper.Menu_Y, hud.transform.position);
            gameObject.transform.position -= gameObject.transform.forward;
            //scroll ui
            ScrollManagerUI = ContainerTypeUIManager.CreateObject();
            ScrollManagerUI.UseableType = UsableItemType.Scroll;
            var rectScroll = (RectTransform)ScrollManagerUI.transform;
            rectScroll.SetParent(gameObject.transform);
            rectScroll.SetSiblingIndex(0);
            rectScroll.gameObject.SetActive(false);

            //Potion ui
            PotionManagerUI = ContainerTypeUIManager.CreateObject();
            PotionManagerUI.UseableType = UsableItemType.Potion;
            var rectPotion = (RectTransform)PotionManagerUI.transform;
            rectPotion.SetParent(gameObject.transform);
            rectPotion.SetSiblingIndex(0);
            rectPotion.gameObject.SetActive(false);

            //Wand ui
            WandManagerUI = ContainerTypeUIManager.CreateObject();
            WandManagerUI.UseableType = UsableItemType.Wand;
            var rectWand = (RectTransform)WandManagerUI.transform;
            rectWand.SetParent(gameObject.transform);
            rectWand.SetSiblingIndex(0);
            rectWand.gameObject.SetActive(false);

            _togglePanel = gameObject.transform.Find("TweakTogglePanel").gameObject;
            _togglePanel.SetActive(true);

            _buttonScrolls = new ButtonWrapper((RectTransform)_togglePanel.transform.Find("Scrolls"), "Scrolls", HandleToggleScrolls);
            _buttonWands = new ButtonWrapper((RectTransform)_togglePanel.transform.Find("Wands"), "Wands", HandleToggleWands);
            _buttonPotions = new ButtonWrapper((RectTransform)_togglePanel.transform.Find("Potions"), "Potions", HandleTogglePotions);
            //_buttonTrash = new ButtonWrapper((RectTransform)_togglePanel.transform.Find("Trash"), "Trash", HandleToggleTrash);
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f;
        }

        private static Vector3 SetMenuPosition(float x, float y, Vector3 z)
        {
            float ascpectRatio = (float)Screen.width / (float)Screen.height;
            return Camera.current.ScreenToWorldPoint
                (new Vector3(Screen.width * x, (Screen.height * y) * (ascpectRatio * 0.5625f), Camera.current.WorldToScreenPoint(z).z));
        }

        private void HandleToggleScrolls()
        {
            ScrollManagerUI.transform.gameObject.SetActive(_buttonScrolls.ButtonToggle = !_buttonScrolls.ButtonToggle);
            WandManagerUI.transform.gameObject.SetActive(_buttonWands.ButtonToggle = false);
            PotionManagerUI.transform.gameObject.SetActive(_buttonPotions.ButtonToggle = false);
            //TrashManagerUI.transform.gameObject.SetActive(false);
        }

        private void HandleToggleWands()
        {
            ScrollManagerUI.transform.gameObject.SetActive(_buttonScrolls.ButtonToggle = false);
            WandManagerUI.transform.gameObject.SetActive(_buttonWands.ButtonToggle = !_buttonWands.ButtonToggle);
            PotionManagerUI.transform.gameObject.SetActive(_buttonPotions.ButtonToggle = false);
            //TrashManagerUI.transform.gameObject.SetActive(false);
        }
        private void HandleTogglePotions()
        {
            ScrollManagerUI.transform.gameObject.SetActive(_buttonScrolls.ButtonToggle = false);
            WandManagerUI.transform.gameObject.SetActive(_buttonWands.ButtonToggle = false);
            PotionManagerUI.transform.gameObject.SetActive(_buttonPotions.ButtonToggle = !_buttonPotions.ButtonToggle);
            //TrashManagerUI.transform.gameObject.SetActive(false);
        }
        private void HandleToggleTrash()
        {
            ScrollManagerUI.transform.gameObject.SetActive(false);
            WandManagerUI.transform.gameObject.SetActive(false);
            PotionManagerUI.transform.gameObject.SetActive(false);
            //TrashManagerUI.transform.gameObject.SetActive(true);
        }


        void Update()
        {
            if (Game.Instance.CurrentMode == GameModeType.Default || 
                Game.Instance.CurrentMode == GameModeType.EscMode ||
                Game.Instance.CurrentMode == GameModeType.Pause)
            {
                if (Menus.MainMenu.MoveToggle)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                        SettingsWrapper.Menu_X -= _menu_step;
                    else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                        SettingsWrapper.Menu_X += _menu_step;
                    else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                        SettingsWrapper.Menu_Y += _menu_step;
                    else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                        SettingsWrapper.Menu_Y -= _menu_step;
                    else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                        _menu_step *= 2f;
                    else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) && _menu_step > .001f)
                        _menu_step /= 2f;

                    gameObject.transform.position = SetMenuPosition(SettingsWrapper.Menu_X, SettingsWrapper.Menu_Y, hud.transform.position); 
                }
                PotionManagerUI.transform.gameObject.SetActive(_buttonPotions.ButtonToggle);
                WandManagerUI.transform.gameObject.SetActive(_buttonWands.ButtonToggle);
                ScrollManagerUI.transform.gameObject.SetActive(_buttonScrolls.ButtonToggle);
                _togglePanel.transform.gameObject.SetActive(true);

            }
            else
            {
                ScrollManagerUI.transform.gameObject.SetActive(false);
                WandManagerUI.transform.gameObject.SetActive(false);
                PotionManagerUI.transform.gameObject.SetActive(false);
                _togglePanel.transform.gameObject.SetActive(false);
            }
        }

        private class ButtonWrapper
        {
            private bool _isPressed;
            public bool ButtonToggle { get; set; }
            private readonly Color _enableColor = Color.white;
            private readonly Color _disableColor = new Color(0.7f, 0.8f, 1f);
            private readonly RectTransform _button;
            private readonly ButtonPF _toggle;
            private readonly TextMeshProUGUI _textMesh;
            private readonly Image _image;
            private readonly Sprite _defaultSprite;
            private readonly SpriteState _defaultSpriteState;
            private readonly SpriteState _pressedSpriteState;



            //public bool IsInteractable
            //{
            //    get => _button.interactable;
            //    set
            //    {
            //        if (_button.interactable != value)
            //        {
            //            _button.interactable = value;
            //            _textMesh.color = value ? _enableColor : _disableColor;
            //        }
            //    }
            //}



            //public bool IsPressed
            //{
            //    get => _isPressed;
            //    set
            //    {
            //        if (_isPressed != value)
            //        {
            //            _isPressed = value;
            //            if (value)
            //            {
            //                _button.spriteState = _pressedSpriteState;
            //                _image.sprite = _pressedSpriteState.pressedSprite;
            //            }
            //            else
            //            {
            //                _button.spriteState = _defaultSpriteState;
            //                _image.sprite = _defaultSprite;
            //            }
            //        }
            //    }
            //}

            public ButtonWrapper(RectTransform button, string text, UnityAction OnToggle)
            {
                Main.Mod.Debug(MethodBase.GetCurrentMethod());
                this.ButtonToggle = false;
                _button = button;
                _toggle = _button.GetComponent<ButtonPF>();
                _toggle.onClick.AddListener(new UnityAction(OnToggle));
                //_button = new Button.ButtonClickedEvent();
                //_button.onClick.AddListener(new UnityAction(onClick));
                _textMesh = _button.GetComponentInChildren<TextMeshProUGUI>();
                //_textMesh.fontSize = 20;
                //_textMesh.fontSizeMax = 72;
                //_textMesh.fontSizeMin = 18;
                _textMesh.text = text;
                //_textMesh.color = _button.interactable ? _enableColor : _disableColor;
                //_image = _button.gameObject.GetComponent<Image>();
                //_defaultSprite = _image.sprite;
                //_defaultSpriteState = _button.spriteState;
                //_pressedSpriteState = _defaultSpriteState;
                //_pressedSpriteState.disabledSprite = _pressedSpriteState.pressedSprite;
                //_pressedSpriteState.highlightedSprite = _pressedSpriteState.pressedSprite;
            }
        }
    }
}
