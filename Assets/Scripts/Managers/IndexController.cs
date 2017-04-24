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
using Assets.Scripts.Types;

public class IndexController : MonoBehaviour {

    public Button RenderPathButton = null;
    public Button RenderQueueButton = null;
    public Button ShadowSceneButton = null;
    public Button MultipleCamerasButton = null;
    public Button TransparencyDisableButton = null;
    public Button LightMapProbeButton = null;
    public Button HighResTexButton = null;
    public Button ExtraForwardPassButton = null;
    public Button SimpleShader = null;

    PerfSceneManager PerformanceSceneManager = null;

	void Start ()
    {
        RenderPathButton.onClick.AddListener(() => ClickedRenderPathButton());
        RenderQueueButton.onClick.AddListener(() => ClickedRenderQueueButton());
        ShadowSceneButton.onClick.AddListener(() => ClickedShadowButton());
        MultipleCamerasButton.onClick.AddListener(() => ClickedMultiCamButton());
        TransparencyDisableButton.onClick.AddListener(() => ClickedTransparencyButton());
        LightMapProbeButton.onClick.AddListener(() => ClickedLightMapProbeButton());
        HighResTexButton.onClick.AddListener(() => ClickedHighResTexButton());
        ExtraForwardPassButton.onClick.AddListener(() => ClickedExtraForwardPassButton());
        SimpleShader.onClick.AddListener(() => ClickedSimpleShader());
        PerformanceSceneManager = PerfSceneManager.GetInstance();
    }

    void ClickedRenderPathButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.RENDER_PATHS);
    }

    void ClickedRenderQueueButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.RENDERQUEUE);
    }

    void ClickedShadowButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.SHADOW_SCENE);
    }

    void ClickedMultiCamButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.MULTI_CAM);
    }

    void ClickedTransparencyButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.TRANSPARENCY_SCENE);
    }

    void ClickedLightMapProbeButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.LIGHTMAP_PROBE_SCENE);
    }
    void ClickedHighResTexButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.HIGH_RES_TEX_SCENE);
    }

    void ClickedExtraForwardPassButton()
    {
        PerformanceSceneManager.LoadScene(SCENE.EXTRA_FORWARD_PASS);
    }
    void ClickedSimpleShader()
    {
        PerformanceSceneManager.LoadScene(SCENE.SHADER_VIEWING);
    }
}
