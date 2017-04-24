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
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Types;

public class ShadowUIController : MonoBehaviour
{
    [HideInInspector]
    public GameObject DescriptionBox = null;
    Text DescriptionText = null;

    enum SHADOW_CASCADE_COUNT
    {
        NONE,
        TWO,
        FOUR,
        MAX_CASCADES
    }

    enum SHADOW_PROJECTION_TYPE
    {
        CLOSE_FIT,
        STABLE_FIT,
        MAX_TYPES
    }

    SHADOW_CASCADE_COUNT CurrentCascadeLevel = SHADOW_CASCADE_COUNT.NONE;
    SHADOW_PROJECTION_TYPE CurrentProjectionMode = SHADOW_PROJECTION_TYPE.CLOSE_FIT;

    class ShadowCascadeData
    {
        public string ButtonDisplayText;
        public int NumCascades;
        public static string Description;
        public static readonly string ShadowCascadeLabel = "Shadow Cascades: ";
        public static UIController.ButtonData ButtonData = null;

        public ShadowCascadeData(string displayText, int numCascades)
        {
            ButtonDisplayText = displayText;
            NumCascades = numCascades;
        }
    };

    class ShadowProjectionData
    {
        public string ButtonDisplayText;
        public ShadowProjection ProjectionType;
        public static string Description;
        public static readonly string ShadowProjectionLabel = "Shadow Projection: ";
        public static UIController.ButtonData ButtonData = null;

        public ShadowProjectionData(string displayText, ShadowProjection projectionType)
        {
            ButtonDisplayText = displayText;
            ProjectionType = projectionType;
        }
    };

    Dictionary<SHADOW_CASCADE_COUNT, ShadowCascadeData> cascadeDict = new Dictionary<SHADOW_CASCADE_COUNT, ShadowCascadeData>();
    Dictionary<SHADOW_PROJECTION_TYPE, ShadowProjectionData> projectionDict = new Dictionary<SHADOW_PROJECTION_TYPE, ShadowProjectionData>();

    void Awake()
    {
        var perfMgr = PerfSceneManager.GetInstance();
        perfMgr.LoadSceneAdditive(SCENE.TOWER_LOW_COUNT);
        perfMgr.LoadSceneAdditive(SCENE.UI);
    }

	// Use this for initialization
    void Start()
    {

        var canvasRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
        Assert.AreNotEqual(null, canvasRoot);
        var uiControllerScript = canvasRoot.GetComponent<UIController>();
        Assert.AreNotEqual(null, uiControllerScript);

        var cascadeButton = uiControllerScript.GetAvailableButton();
        Assert.AreNotEqual(null, cascadeButton);
        cascadeButton.PointerEnteredButton += PointerEnterCascadeButton;
        cascadeButton.PointerExitedButton += PointerExitCascadeButton;

        var projectionButton = uiControllerScript.GetAvailableButton();
        Assert.AreNotEqual(null, projectionButton);
        projectionButton.PointerEnteredButton += PointerEnterProjectionButton;
        projectionButton.PointerExitedButton += PointerExitProjectionButton;

        // Maybe make a script for description box that contains Text field + accessor to string so I can search on canvas root.
        DescriptionBox = GameObject.FindGameObjectWithTag("DescriptionBox");
        DescriptionText = DescriptionBox.GetComponentInChildren<Text>();
        DescriptionText.text = "";
        DescriptionBox.SetActive(false);

        cascadeButton.ButtonComponent.onClick.AddListener(() => ShadowCascadeClicked());
        projectionButton.ButtonComponent.onClick.AddListener(() => ShadowProjectionClicked());

        // Build Dictionaries
        cascadeDict[SHADOW_CASCADE_COUNT.NONE] = new ShadowCascadeData("None", 0);
        cascadeDict[SHADOW_CASCADE_COUNT.TWO] = new ShadowCascadeData("Two", 2);
        cascadeDict[SHADOW_CASCADE_COUNT.FOUR] = new ShadowCascadeData("Four", 4);
        ShadowCascadeData.ButtonData = cascadeButton;
        ShadowCascadeData.Description = "The number of shadow cascades can be set to zero, two or four. A higher number of cascades gives better quality but at the expense of processing overhead.";
        projectionDict[SHADOW_PROJECTION_TYPE.CLOSE_FIT] = new ShadowProjectionData("Close Fit", ShadowProjection.CloseFit);
        projectionDict[SHADOW_PROJECTION_TYPE.STABLE_FIT] = new ShadowProjectionData("Stable Fit", ShadowProjection.StableFit);
        ShadowProjectionData.ButtonData = projectionButton;
        ShadowProjectionData.Description = "There are two different methods for projecting shadows from a directional light. Close Fit renders higher resolution shadows but they can sometimes wobble slightly if the camera moves. Stable Fit renders lower resolution shadows but they don’t wobble with camera movements.";

        // Set initial settings
        UpdateCascadeCount(CurrentCascadeLevel);
        UpdateProjectionType(SHADOW_PROJECTION_TYPE.CLOSE_FIT);

	}

    void UpdateCascadeCount(SHADOW_CASCADE_COUNT countToSwitchTo)
    {
        var renderInf = cascadeDict[countToSwitchTo];
        QualitySettings.shadowCascades = renderInf.NumCascades;
        // put label in obj?  put button as static?
        ShadowCascadeData.ButtonData.SetText(ShadowCascadeData.ShadowCascadeLabel + renderInf.ButtonDisplayText);
    }

    void UpdateProjectionType(SHADOW_PROJECTION_TYPE projectionType)
    {
        var projectionInf = projectionDict[projectionType];
        QualitySettings.shadowProjection = projectionInf.ProjectionType;
        ShadowProjectionData.ButtonData.SetText(ShadowProjectionData.ShadowProjectionLabel + projectionInf.ButtonDisplayText);
    }

    void ShadowCascadeClicked()
    {
        CurrentCascadeLevel = (SHADOW_CASCADE_COUNT)(((((int)CurrentCascadeLevel) + 1)) % ((int)SHADOW_CASCADE_COUNT.MAX_CASCADES));
        UpdateCascadeCount(CurrentCascadeLevel);
    }

    void ShadowProjectionClicked()
    {
        CurrentProjectionMode = (SHADOW_PROJECTION_TYPE)(((((int)CurrentProjectionMode) + 1)) % ((int)SHADOW_PROJECTION_TYPE.MAX_TYPES));
        UpdateProjectionType(CurrentProjectionMode);
    }

    public void PointerEnterCascadeButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(true);
        DescriptionText.text = ShadowCascadeData.Description;
    }

    public void PointerExitCascadeButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(false);
    }
    public void PointerEnterProjectionButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(true);
        DescriptionText.text = ShadowProjectionData.Description;
    }

    public void PointerExitProjectionButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(false);
    }
}