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

public class TransparentTexController : MonoBehaviour {

    public GameObject TransparentObjectRoot = null;
    MeshRenderer[] TransparentRenderers;
    float[] TransparentRenderersStartingTransp;

    [HideInInspector]
    public GameObject DescriptionBox = null;
    Text DescriptionText = null;

    enum TRANSPARENCY_STATE
    {
        SEMI_TRANSPARENT,
        TRANSPARENT,
        MAX_STATES
    }

    TRANSPARENCY_STATE CurrentTransparencyState = TRANSPARENCY_STATE.SEMI_TRANSPARENT;

    class TransparencyData
    {
        public string ButtonDisplayText;
        public static string Description;
        public static readonly string TransparencyLabel = "Transparency: ";
        public static UIController.ButtonData ButtonData = null;

        public TransparencyData(string displayText)
        {
            ButtonDisplayText = displayText;
        }
    };

    Dictionary<TRANSPARENCY_STATE, TransparencyData> transparencySceneDict = new Dictionary<TRANSPARENCY_STATE, TransparencyData>();

    void Awake()
    {
        var perfMgr = PerfSceneManager.GetInstance();
        perfMgr.LoadSceneAdditive(SCENE.UI);
    }

	void Start () {
        TransparentRenderers = TransparentObjectRoot.GetComponentsInChildren<MeshRenderer>();
        Assert.AreNotEqual(0, TransparentRenderers.Length);
	    TransparentRenderersStartingTransp = new float[TransparentRenderers.Length];
        for (int i = 0; i < TransparentRenderers.Length; i++ )
        {
            TransparentRenderersStartingTransp[i] = TransparentRenderers[i].material.color.a;
        }
     
        /* Get ref to description box and reset it - Put this in UI controller later */
        DescriptionBox = GameObject.FindGameObjectWithTag("DescriptionBox");
        DescriptionText = DescriptionBox.GetComponentInChildren<Text>();
        DescriptionBox.SetActive(false);
        Assert.AreNotEqual(null, DescriptionBox);
        

        /* Set Up UI Stuff */
        var canvasRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
        Assert.AreNotEqual(null, canvasRoot);
        var uiControllerScript = canvasRoot.GetComponent<UIController>();
        Assert.AreNotEqual(null, uiControllerScript);

        var ToggleTransparencyButton = uiControllerScript.GetAvailableButton();
        Assert.AreNotEqual(null, ToggleTransparencyButton);
        ToggleTransparencyButton.PointerEnteredButton += PointerEnterTransparencyButton;
        ToggleTransparencyButton.PointerExitedButton += PointerExitTransparencyButton;
        ToggleTransparencyButton.ButtonComponent.onClick.AddListener(() => TransparencyClicked());

        transparencySceneDict[TRANSPARENCY_STATE.SEMI_TRANSPARENT] = new TransparencyData("Semi-Transparent");
        transparencySceneDict[TRANSPARENCY_STATE.TRANSPARENT] = new TransparencyData("Transparent");

        TransparencyData.ButtonData = ToggleTransparencyButton;
        TransparencyData.Description = "Toggles between a semi-transparent and fully transparent state for a few mesh renderers in the scene.  Taking a framecapture of the scene with the objects fully transparent will show that they still get sent to draw.  Developers need to take care and set fully transparent objects to inactive state to save frame time.  This is common in fade effects.";

        DescriptionText.text = TransparencyData.Description; 
        ToggleTransparencyButton.SetText(TransparencyData.TransparencyLabel + transparencySceneDict[CurrentTransparencyState].ButtonDisplayText);

        SwitchRenderers(TRANSPARENCY_STATE.TRANSPARENT);
        SwitchRenderers(TRANSPARENCY_STATE.SEMI_TRANSPARENT);
    }

    void SwitchRenderers(TRANSPARENCY_STATE newState)
    {
        Assert.AreEqual(TransparentRenderers.Length, TransparentRenderersStartingTransp.Length);
        for (int i = 0; i < TransparentRenderers.Length; i++)
        {
            Color oldColor = TransparentRenderers[i].material.color;

            oldColor.a = newState == TRANSPARENCY_STATE.SEMI_TRANSPARENT ? TransparentRenderersStartingTransp[i] : 0.0f;
            TransparentRenderers[i].material.color = oldColor;
        }
    }

    public MeshRenderer myMeshRenderer;

    void UpdateMaterialColor(Color newColor)
    {
        // If fully transparent (alpha == 0), disable renderer
        if(newColor.a == 0.0f)
        {
            myMeshRenderer.enabled = false;
        }
        else
        {
            myMeshRenderer.enabled = true;
        }
        myMeshRenderer.material.color = newColor;
    }

    void EfficientSwitchRenderers(TRANSPARENCY_STATE newState)
    {
        Assert.AreEqual(TransparentRenderers.Length, TransparentRenderersStartingTransp.Length);
        for (int i = 0; i < TransparentRenderers.Length; i++)
        {
            Color oldColor = TransparentRenderers[i].material.color;

            if(newState == TRANSPARENCY_STATE.TRANSPARENT)
            {

            }
            else if(newState == TRANSPARENCY_STATE.SEMI_TRANSPARENT)
            {

            }

            oldColor.a = newState == TRANSPARENCY_STATE.SEMI_TRANSPARENT ? TransparentRenderersStartingTransp[i] : 0.0f;
            TransparentRenderers[i].material.color = oldColor;
        }
    }

    void PointerEnterTransparencyButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(true);
    }

    void PointerExitTransparencyButton(object source, EventArgs e)
    {
        DescriptionBox.SetActive(false);
    }

    void TransparencyClicked()
    {
        CurrentTransparencyState = (TRANSPARENCY_STATE)(((((int)CurrentTransparencyState) + 1)) % ((int)TRANSPARENCY_STATE.MAX_STATES));
        SwitchRenderers(CurrentTransparencyState);
    }
}
