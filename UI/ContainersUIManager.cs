﻿using Kingmaker.UI;
using Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.UI.Constructor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using Kingmaker.UI.Log;

namespace InventoryTweaks.UI
{
    class ContainersUIManager : MonoBehaviour
    {
        public  ContainerTypeUIManager scrollManagerUI { get; private set; }
        private CanvasGroup _canvasGroup;
        private GameObject _togglePanel;
        public static ContainersUIManager CreateObject()
        {
            UICommon uiCommon = Game.Instance.UI.Common;
            GameObject hud = uiCommon?.transform.Find("HUDLayout")?.gameObject;
            GameObject tooglePanel = uiCommon?.transform.Find("HUDLayout/CombatLog/TooglePanel")?.gameObject;

            if (!tooglePanel || !hud)
                return null;

            GameObject containers = new GameObject("TweakContainers", typeof(RectTransform), typeof(CanvasGroup));
            containers.transform.SetParent(hud.transform);
            containers.transform.SetSiblingIndex(0);
            
            RectTransform rectTweakContainer = (RectTransform)containers.transform;
            rectTweakContainer.anchorMin = new Vector2(0f, 1f);
            rectTweakContainer.anchorMax = new Vector2(0f, 1f);
            rectTweakContainer.pivot = new Vector2(0f, 0f);
            rectTweakContainer.position = Camera.current.ScreenToWorldPoint
                (new Vector3(Screen.width * 0.285f, Screen.height * 0.7f, Camera.current.WorldToScreenPoint(hud.transform.position).z));
            rectTweakContainer.position -= rectTweakContainer.forward;
            rectTweakContainer.rotation = Quaternion.identity;
            rectTweakContainer.localScale = new Vector3(.9f, .9f, .9f);

            //initialize buttons
            GameObject togglePanel = Instantiate(tooglePanel, containers.transform, false);
            togglePanel.name = "TweakTogglePanel";
            RectTransform rectButton = (RectTransform)togglePanel.transform;
            rectButton.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            rectButton.anchorMin = new Vector2(0f, 1f);
            rectButton.anchorMax = new Vector2(0f, 1f);
            rectButton.pivot = new Vector2(0f, 0f);
            rectButton.localPosition = new Vector3(0 - rectButton.rect.width * 1.08f, 0 - rectTweakContainer.rect.yMax * 6.85f, 0);
            
            rectButton.rotation = Quaternion.identity;
            DestroyImmediate(togglePanel.GetComponent<LogToggleManager>());
            DestroyImmediate(togglePanel.GetComponent<Image>());

            void setToggleButtons(GameObject button, string name)
            {
                button.name = name;

            }

            GameObject toggleScrolls = togglePanel.transform.Find("ToogleAll").gameObject;
            setToggleButtons(toggleScrolls, "Scrolls");
            GameObject togglePotions = togglePanel.transform.Find("ToogleLifeEvent").gameObject;
            setToggleButtons(togglePotions, "Potions");
            GameObject toggleWands = togglePanel.transform.Find("ToogleCombat").gameObject;
            setToggleButtons(toggleWands, "Wands");
            GameObject toggleTrash = togglePanel.transform.Find("ToogleDialogue").gameObject;
            setToggleButtons(toggleTrash, "Trash");

            return containers.AddComponent<ContainersUIManager>();
        }

        void Awake()
        {
            Main.Mod.Debug(MethodBase.GetCurrentMethod());

            //scroll ui
            scrollManagerUI = ContainerTypeUIManager.CreateObject();
            scrollManagerUI.UseableType = UsableItemType.Scroll;
            var rectScroll = (RectTransform)scrollManagerUI.transform;
            rectScroll.SetParent(gameObject.transform);
            rectScroll.SetSiblingIndex(0);
            rectScroll.gameObject.SetActive(true);

            _togglePanel = gameObject.transform.Find("TweakTogglePanel").gameObject;
            _togglePanel.SetActive(true);

            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f;
        }

        void Update()
        {
        }

        private class ButtonWrapper
        {
            private bool _isPressed;

            private readonly Color _enableColor = Color.white;
            private readonly Color _disableColor = new Color(0.7f, 0.8f, 1f);

            private readonly ButtonPF _button;
            private readonly TextMeshProUGUI _textMesh;
            private readonly Image _image;
            private readonly Sprite _defaultSprite;
            private readonly SpriteState _defaultSpriteState;
            private readonly SpriteState _pressedSpriteState;

            public bool IsInteractable
            {
                get => _button.interactable;
                set
                {
                    if (_button.interactable != value)
                    {
                        _button.interactable = value;
                        _textMesh.color = value ? _enableColor : _disableColor;
                    }
                }
            }



            public bool IsPressed
            {
                get => _isPressed;
                set
                {
                    if (_isPressed != value)
                    {
                        _isPressed = value;
                        if (value)
                        {
                            _button.spriteState = _pressedSpriteState;
                            _image.sprite = _pressedSpriteState.pressedSprite;
                        }
                        else
                        {
                            _button.spriteState = _defaultSpriteState;
                            _image.sprite = _defaultSprite;
                        }
                    }
                }
            }

            public ButtonWrapper(ButtonPF button, string text, Action onClick)
            {
                _button = button;
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(new UnityAction(onClick));
                _textMesh = _button.GetComponentInChildren<TextMeshProUGUI>();
                //_textMesh.fontSize = 20;
                //_textMesh.fontSizeMax = 72;
                //_textMesh.fontSizeMin = 18;
                //_textMesh.text = text;
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
