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
using Assets.Scripts.Managers;
using Assets.Scripts.Types;

public class RenderingPathUI : MonoBehaviour {

    private Camera LoadedMainCamera = null;

    [HideInInspector]
    public GameObject DescriptionBox = null;
    Text DescriptionText = null;

    private const string s_RenderingBaseLabel = "Mode: ";
    enum RENDERING_TYPE {
        LEGACY_DEFERRED,
        DEFERRED,
        FORWARD,
        VERTEX_LIT,
        NUM_TYPES
    };

    class RenderingData {
        public string ButtonDisplayText;

        public static readonly string RenderPathLabel = "Mode: ";
        public static UIController.ButtonData ButtonData = null;
        public RenderingPath UnityRenderingPath;
        public string Description;
        public RenderingData(string displayText, RenderingPath unityRenderingPath)
        {
            ButtonDisplayText = displayText;
            UnityRenderingPath = unityRenderingPath;
        }
    };

    private Dictionary<RENDERING_TYPE, RenderingData> renderDict = new Dictionary<RENDERING_TYPE, RenderingData>();

    private RENDERING_TYPE CurrentRenderingMode = RENDERING_TYPE.FORWARD;
    

    // Necessary as tagged MainCamera exists in another scene
    void Awake()
    {
        var perfMgr = PerfSceneManager.GetInstance();
        perfMgr.LoadSceneAdditive(SCENE.TOWER_LOW_COUNT);
        perfMgr.LoadSceneAdditive(SCENE.UI);

        RenderingData deferredLightingInf = new RenderingData("Legacy Deferred", RenderingPath.DeferredLighting);
        deferredLightingInf.Description = "Legacy version of the deferred renderer.";
        renderDict[RENDERING_TYPE.LEGACY_DEFERRED] = deferredLightingInf;
        RenderingData deferredShadingRef = new RenderingData("Deferred", RenderingPath.DeferredShading);
        deferredShadingRef.Description = "Lighting performance unrelated to scene geometry complexity.  Trade heavy lighting computation for more memory usage leading to a higher chance of being memory bound.  Real-time shadows and per-pixel effects supported.  Semi-transparent rendering done in additional forwarded pass.  No support for anti-aliasing and Mesh Renderer's Receive Shadows flag.";
        renderDict[RENDERING_TYPE.DEFERRED] = deferredShadingRef;
        RenderingData forwardInf = new RenderingData("Forward", RenderingPath.Forward);
        forwardInf.Description = "Lighting done with a combination of per-pixel, per-vertex, and spherical harmonic techniques.  Real-time shadows and other per-pixel effects supported.  Does not incur memory cost required to build the g-buffer as in deferred.  Can lead to many draw calls covering the same pixel(s) if care isn't taken.";
        renderDict[RENDERING_TYPE.FORWARD] = forwardInf;
        RenderingData vertexLitInf = new RenderingData("Vertex Lit", RenderingPath.VertexLit);
        vertexLitInf.Description = "Does calculations per-vertex, and not per-pixel.  Can improve mobile performance.  Real-time shadows and other per-pixel effects not supported.  Low quality lighting.";
        renderDict[RENDERING_TYPE.VERTEX_LIT] = vertexLitInf;
    }

    void Start()
    {
        var CamObj = GameObject.FindGameObjectWithTag("MainCamera");
        Assert.AreNotEqual(null, CamObj);
        LoadedMainCamera = CamObj.GetComponent<Camera>();
        Assert.AreNotEqual(null, LoadedMainCamera);

        var canvasRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
        Assert.AreNotEqual(null, canvasRoot);
        var uiControllerScript = canvasRoot.GetComponent<UIController>();
        Assert.AreNotEqual(null, uiControllerScript);

        UIController.ButtonData renderQueueButton = uiControllerScript.GetAvailableButton();
        Assert.AreNotEqual(null, renderQueueButton);
        renderQueueButton.PointerEnteredButton += PointerEnterPathButton;
        renderQueueButton.PointerExitedButton += PointerExitPathButton;

        DescriptionBox = GameObject.FindGameObjectWithTag("DescriptionBox");
        DescriptionText = DescriptionBox.GetComponentInChildren<Text>();
        DescriptionText.text = "";
        DescriptionBox.SetActive(false);

        renderQueueButton.ButtonComponent.onClick.AddListener(() => SwitchRenderMode());


        RenderingData.ButtonData = renderQueueButton;
        renderQueueButton.SetText(RenderingData.RenderPathLabel + ": " + renderDict[CurrentRenderingMode].ButtonDisplayText);

        // Always start with forward rendering mode
        SetRenderingMode(CurrentRenderingMode);
    }

    void SetRenderingMode(RENDERING_TYPE type)
    {
        var renderInf = renderDict[type];
        Assert.AreNotEqual(null, renderInf);
        LoadedMainCamera.renderingPath = renderInf.UnityRenderingPath;
        RenderingData.ButtonData.SetText(RenderingData.RenderPathLabel + " " + renderDict[type].ButtonDisplayText);
        SetDescriptionBoxToCurrentRender();
    }

    void SwitchRenderMode()
    {
        CurrentRenderingMode = (RENDERING_TYPE) (((((int) CurrentRenderingMode) + 1)) % ((int)RENDERING_TYPE.NUM_TYPES));
        SetRenderingMode(CurrentRenderingMode);
    }

    void SetDescriptionBoxToCurrentRender()
    {
        if(!DescriptionBox.activeInHierarchy)
        {
            DescriptionBox.SetActive(true);
        }
        var renderInf = renderDict[CurrentRenderingMode];
        DescriptionText.text = renderInf.Description;
    }

    public void PointerEnterPathButton(object source, EventArgs e)
    {
        SetDescriptionBoxToCurrentRender();
    }

    public void PointerExitPathButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(false);
    }
}
