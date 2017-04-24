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
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Assets.Scripts.Types;

public class PerfSceneManager
{
    private static PerfSceneManager ManagerSingleton = null;
    private Dictionary<SCENE, string> SceneToNameDict = new Dictionary<SCENE, string>();

    public static PerfSceneManager GetInstance()
    {
        if (ManagerSingleton == null)
        {
            ManagerSingleton = new PerfSceneManager();
        }
        return ManagerSingleton;
    }

    public PerfSceneManager()
    {
        SceneToNameDict[SCENE.UI] = "__UI_Cam_Scene";
        SceneToNameDict[SCENE.TOWER] = "__TowerScene";
        SceneToNameDict[SCENE.TOWER_LOW_COUNT] = "__TowerScene_LowModelsCount";
        SceneToNameDict[SCENE.RENDER_PATHS] = "__RenderPathScene";
        SceneToNameDict[SCENE.RENDERQUEUE] = "__RenderQueueScene";
        SceneToNameDict[SCENE.SHADOW_SCENE] = "__ShadowScene";
        SceneToNameDict[SCENE.TRANSPARENCY_SCENE] = "__TransparentTextureScene";
        SceneToNameDict[SCENE.MULTI_CAM] = "__MultiCamScene";
        SceneToNameDict[SCENE.LIGHTMAP_PROBE_SCENE] = "__LightMapProbeScene";
        SceneToNameDict[SCENE.SCENE_INDEX] = "__SceneIndex";
        SceneToNameDict[SCENE.HIGH_RES_TEX_SCENE] = "__HighResTextureScene";
        SceneToNameDict[SCENE.EXTRA_FORWARD_PASS] = "__ForwardDraws";
        SceneToNameDict[SCENE.SHADER_VIEWING] = "__ShaderViewing";
    }

    public void LoadScene(SCENE sceneID)
    {
        SceneManager.LoadScene(SceneToNameDict[sceneID], LoadSceneMode.Single);
    }

    public void LoadSceneAdditive(SCENE sceneID)
    {
        SceneManager.LoadScene(SceneToNameDict[sceneID], LoadSceneMode.Additive);
    }
}
