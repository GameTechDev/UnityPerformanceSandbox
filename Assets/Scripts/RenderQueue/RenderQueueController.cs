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

public class RenderQueueController : MonoBehaviour
{

    public MeshRenderer FloorDownRenderer = null;
    public MeshRenderer FloorUpRenderer = null;
    public MeshRenderer LeftCubeRenderer = null;
    public MeshRenderer RightCubeRenderer = null;

    [HideInInspector]
    public GameObject DescriptionBox = null;
    Text DescriptionText = null;

    enum OBJECT
    {
        FLOOR_UP,
        FLOOR_DOWN,
        CUBE_LEFT,
        CUBE_RIGHT
    }

    Dictionary<OBJECT, int> OriginalRenderQueueNumbers = new Dictionary<OBJECT, int>();

    enum RENDER_QUEUE_STATE
    {
        DEFAULT_ORDERING,
        SMART_ORDERING,
        MAX_STATES
    }

    RENDER_QUEUE_STATE CurrentRenderQueueState = RENDER_QUEUE_STATE.DEFAULT_ORDERING;

    class RenderQueueData
    {
        public string ButtonDisplayText;
        public static string Description;
        public static readonly string RenderQueueLabel = "Render Sorting: ";
        public static UIController.ButtonData ButtonData = null;

        public RenderQueueData(string displayText)
        {
            ButtonDisplayText = displayText;
        }
    };

    Dictionary<RENDER_QUEUE_STATE, RenderQueueData> renderDict = new Dictionary<RENDER_QUEUE_STATE, RenderQueueData>();

    void Awake()
    {
        var perfMgr = PerfSceneManager.GetInstance();
        //perfMgr.LoadScene(PerfSceneManager.SCENE.BUILDINGS);
        perfMgr.LoadSceneAdditive(SCENE.UI);
    }

	// Use this for initialization
    void Start()
    {
        var canvasRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
        Assert.AreNotEqual(null, canvasRoot);
        var uiControllerScript = canvasRoot.GetComponent<UIController>();
        Assert.AreNotEqual(null, uiControllerScript);

        var renderQueueButton = uiControllerScript.GetAvailableButton();
        Assert.AreNotEqual(null, renderQueueButton);
        renderQueueButton.PointerEnteredButton += PointerEnterCascadeButton;
        renderQueueButton.PointerExitedButton += PointerExitCascadeButton;

        // Maybe make a script for description box that contains Text field + accessor to string so I can search on canvas root.
        DescriptionBox = GameObject.FindGameObjectWithTag("DescriptionBox");
        DescriptionText = DescriptionBox.GetComponentInChildren<Text>();
        DescriptionText.text = "";
        DescriptionBox.SetActive(false);

        renderQueueButton.ButtonComponent.onClick.AddListener(() => RenderQueueClicked());

        // Build Dictionaries
        renderDict[RENDER_QUEUE_STATE.DEFAULT_ORDERING] = new RenderQueueData("Default");
        renderDict[RENDER_QUEUE_STATE.SMART_ORDERING] = new RenderQueueData("Smart Ordering");

        RenderQueueData.ButtonData = renderQueueButton;
        RenderQueueData.Description = "Toggles between the default ordering mode and the manual ordering mode.  Default will do Z ordering based on the central point of a mesh.  Sometimes developers will use a plane as their floor and scale it up.  This can lead to a lot of overdrawn pixels.";
        renderQueueButton.SetText(RenderQueueData.RenderQueueLabel + renderDict[RENDER_QUEUE_STATE.DEFAULT_ORDERING].ButtonDisplayText);

        OriginalRenderQueueNumbers[OBJECT.FLOOR_UP] = FloorUpRenderer.material.renderQueue;
        OriginalRenderQueueNumbers[OBJECT.FLOOR_DOWN] = FloorDownRenderer.material.renderQueue;
        OriginalRenderQueueNumbers[OBJECT.CUBE_LEFT] = RightCubeRenderer.material.renderQueue;
        OriginalRenderQueueNumbers[OBJECT.CUBE_RIGHT] = LeftCubeRenderer.material.renderQueue;
    }

    void RenderQueueClicked()
    {
        CurrentRenderQueueState = (RENDER_QUEUE_STATE)(((((int)CurrentRenderQueueState) + 1)) % ((int)RENDER_QUEUE_STATE.MAX_STATES));
        if(CurrentRenderQueueState == RENDER_QUEUE_STATE.DEFAULT_ORDERING)
        {
            // default ordering algo
            // 1) On load, get the renderqueue place of each renderer in scene.
            // 2) On switch back, set the renderqueue of everything to be what initial value was

            FloorUpRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.FLOOR_UP];
            FloorDownRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.FLOOR_DOWN];
            RightCubeRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.CUBE_LEFT];
            LeftCubeRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.CUBE_RIGHT];
        }
        else if(CurrentRenderQueueState == RENDER_QUEUE_STATE.SMART_ORDERING)
        {
            // smart ordering algo
            RightCubeRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.CUBE_LEFT] + 0;
            LeftCubeRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.CUBE_RIGHT] + 1;
            FloorUpRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.FLOOR_UP] + 2;
            FloorDownRenderer.material.renderQueue = OriginalRenderQueueNumbers[OBJECT.FLOOR_DOWN] + 3;
        }
        RenderQueueData.ButtonData.SetText(RenderQueueData.RenderQueueLabel + renderDict[CurrentRenderQueueState].ButtonDisplayText);
    }

    public void PointerEnterCascadeButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(true);
        DescriptionText.text = RenderQueueData.Description;
    }

    public void PointerExitCascadeButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(false);
    }
}