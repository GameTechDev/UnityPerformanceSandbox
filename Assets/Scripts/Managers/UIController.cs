/////////////////////////////////////////////////////////////////////////////////////////////
// Copyright 2017 Intel Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Assets.Scripts.Types;

namespace Assets.Scripts.Managers
{
    public class UIController : MonoBehaviour
    {

        public GameObject ButtonPrefab = null;
        public GameObject DescriptionBox = null;
        public GameObject ButtonsSection = null;

        static List<ButtonData> AvailableButtonList = new List<ButtonData>();
        static List<ButtonData> UsedButtonList = new List<ButtonData>();

        public int MaxButtons;

        public Button ToggleUIButton = null;
        Text ToggleUIText = null;

        public Button ToggleCamMovementButton = null;
        Text ToggleCamMovementText = null;

        public Button BackToSceneIndexButton = null;

        public Camera MainCamera = null;
        CameraMovement MainCameraController = null;

        GameObject UISceneRoot = null;

        enum UI_STATE
        {
            VISIBLE,
            INVISIBLE,
            STATE_COUNT
        }

        UI_STATE CurrentUIState = UI_STATE.VISIBLE;

        public enum UI_MODE
        {
            WITH_UI,
            WITHOUT_UI
        }

        UI_MODE CurrentUIMode = UI_MODE.WITH_UI;

        public class ButtonData
        {
            private GameObject ButtonObjRef;
            private EventTrigger HoverEventTrigger;
            public Button ButtonComponent;
            private Text TextComponent;

            public event EventHandler PointerEnteredButton;
            public event EventHandler PointerExitedButton;

            protected virtual void OnPointerEnteredButton()
            {
                if (PointerEnteredButton != null)
                {
                    PointerEnteredButton(this, EventArgs.Empty);
                }
            }

            protected virtual void OnPointerExitedButton()
            {
                if (PointerExitedButton != null)
                {
                    PointerExitedButton(this, EventArgs.Empty);
                }
            }

            public void SetText(string newText)
            {
                if(TextComponent != null)
                    TextComponent.text = newText;
            }

            public void SetVisibility(bool visibility)
            {
                if(ButtonObjRef != null)
                    ButtonObjRef.SetActive(visibility);
            }

            public ButtonData(GameObject buttonObjRef)
            {
                ButtonObjRef = buttonObjRef;
                Assert.AreNotEqual(null, ButtonObjRef);
                HoverEventTrigger = ButtonObjRef.GetComponent<EventTrigger>();
                Assert.AreNotEqual(null, HoverEventTrigger);

                // Create handler for pointer entered
                EventTrigger.Entry pointerEnteredEntry = new EventTrigger.Entry();
                pointerEnteredEntry.callback = new EventTrigger.TriggerEvent();
                pointerEnteredEntry.callback.AddListener((x) => OnPointerEnteredButton());
                pointerEnteredEntry.eventID = EventTriggerType.PointerEnter;
                HoverEventTrigger.triggers.Add(pointerEnteredEntry);

                // Create handler for pointer exited
                EventTrigger.Entry pointerExitedEntry = new EventTrigger.Entry();
                pointerExitedEntry.callback = new EventTrigger.TriggerEvent();
                pointerExitedEntry.callback.AddListener((x) => OnPointerExitedButton());
                pointerExitedEntry.eventID = EventTriggerType.PointerExit;
                HoverEventTrigger.triggers.Add(pointerExitedEntry);


                // Get component references
                TextComponent = ButtonObjRef.GetComponentInChildren<Text>();
                Assert.AreNotEqual(null, TextComponent);

                ButtonComponent = ButtonObjRef.GetComponent<Button>();
                Assert.AreNotEqual(null, ButtonComponent);
            }
        }

        void ReleaseAllResources()
        {
            UsedButtonList.Clear();
            AvailableButtonList.Clear();
        }

        void SwitchUIText()
        {
            CurrentUIState = (UI_STATE)(((((int)CurrentUIState) + 1)) % ((int)UI_STATE.STATE_COUNT));
            if (CurrentUIState == UI_STATE.INVISIBLE)
            {
                // display all UI elements
                foreach (ButtonData element in UsedButtonList)
                {
                    element.ButtonComponent.gameObject.SetActive(false);
                }
                if(DescriptionBox.activeInHierarchy == true)
                {
                    DescriptionBox.SetActive(false);
                }
                ToggleCamMovementButton.gameObject.SetActive(false);
                BackToSceneIndexButton.gameObject.SetActive(false);
                ToggleUIText.text = "UI: Off";
            }
            else
            {
                // turn off all UI elements
                foreach (ButtonData element in UsedButtonList)
                {
                    element.ButtonComponent.gameObject.SetActive(true);
                }
                ToggleCamMovementButton.gameObject.SetActive(true);
                BackToSceneIndexButton.gameObject.SetActive(true);
                ToggleUIText.text = "UI: On";
            }
        }

        void SwitchCamMovement()
        {
            MainCameraController.CurrentMovement = (CameraMovement.MOVEMENT)(((((int)MainCameraController.CurrentMovement) + 1)) % ((int)CameraMovement.MOVEMENT.STATE_COUNT));
            if (MainCameraController.CurrentMovement == CameraMovement.MOVEMENT.RAILS)
            {
                ToggleCamMovementText.text = "Cam: Rails";
            }
            else
            {
                ToggleCamMovementText.text = "Cam: Static";
            }
        }

        void Start()
        {
            Assert.AreNotEqual(null, ButtonsSection);
            Assert.AreNotEqual(null, DescriptionBox);
            Assert.AreNotEqual(null, ButtonPrefab);

            Assert.AreNotEqual(null, ToggleUIButton);
            Assert.AreNotEqual(null, ToggleCamMovementButton);

            ToggleUIText = ToggleUIButton.GetComponentInChildren<Text>();
            ToggleCamMovementText = ToggleCamMovementButton.GetComponentInChildren<Text>();
            Assert.AreNotEqual(null, ToggleUIText);
            Assert.AreNotEqual(null, ToggleCamMovementText);

            ToggleUIButton.onClick.AddListener(() => SwitchUIText());
            ToggleCamMovementButton.onClick.AddListener(() => SwitchCamMovement());
            BackToSceneIndexButton.onClick.AddListener(() => SwitchToIndexScene());

            MainCameraController = MainCamera.GetComponent<CameraMovement>();
            Assert.AreNotEqual(null, MainCameraController);

            UISceneRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
            Assert.AreNotEqual(null, UISceneRoot);
            if (DescriptionBox.activeInHierarchy == true)
            {
                DescriptionBox.SetActive(false);
            }
        }

        void SwitchToIndexScene()
        {
            ReleaseAllResources();
            var sceneMgr = PerfSceneManager.GetInstance();
            Destroy(UISceneRoot);
            sceneMgr.LoadScene(SCENE.SCENE_INDEX);
            
        }

        public ButtonData GetAvailableButton()
        {
            if ((AvailableButtonList.Count + UsedButtonList.Count) == 0)
            {
                GenerateButtons();
            }
            Assert.AreNotEqual(0, AvailableButtonList.Count);

            int indexToPullFrom = 0;

            var button = AvailableButtonList[indexToPullFrom];
            Assert.AreNotEqual(null, button);
            AvailableButtonList.RemoveAt(indexToPullFrom);
            UsedButtonList.Add(button);
            button.SetVisibility(true);
            return button;
        }

        public void SetUIMode(UI_MODE mode)
        {
            if (mode == UI_MODE.WITH_UI && CurrentUIMode != UI_MODE.WITH_UI)
            {
                if (!DescriptionBox.activeInHierarchy)
                {
                    DescriptionBox.SetActive(true);
                }
                if (!ToggleUIButton.gameObject.activeInHierarchy)
                {
                    ToggleUIButton.gameObject.SetActive(true);
                }
                if (!ToggleCamMovementButton.gameObject.activeInHierarchy)
                {
                    ToggleCamMovementButton.gameObject.SetActive(true);
                }
            }
            else if (mode == UI_MODE.WITHOUT_UI && CurrentUIMode != UI_MODE.WITHOUT_UI)
            {
                if (DescriptionBox.activeInHierarchy)
                {
                    DescriptionBox.SetActive(false);
                }
                if (ToggleUIButton.gameObject.activeInHierarchy)
                {
                    ToggleUIButton.gameObject.SetActive(false);
                }
                if (ToggleCamMovementButton.gameObject.activeInHierarchy)
                {
                    ToggleCamMovementButton.gameObject.SetActive(false);
                }
                
            }
            CurrentUIMode = mode;
        }

        void GenerateButtons()
        {
            float yForEachButton = 1.0f / (float)MaxButtons;

            for (int i = 0; i < MaxButtons; i++)
            {
                var newButton = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newButton.transform.SetParent(ButtonsSection.transform);
                var rect = newButton.GetComponent<RectTransform>();

                float diff = yForEachButton * i;

                rect.anchorMin = new Vector2(0.0f, 1.0f - diff - yForEachButton);
                rect.anchorMax = new Vector2(1.0f, 1.0f - diff);

                // post anchor adjustment
                rect.offsetMax = new Vector2(0.0f, 0.0f);
                rect.offsetMin = new Vector2(0.0f, 0.0f);
                
                var newButtonData = new ButtonData(newButton);

                AvailableButtonList.Add(newButtonData);
                newButtonData.SetVisibility(false);
            }
        }
    }
}