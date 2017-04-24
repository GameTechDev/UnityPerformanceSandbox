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
using Assets.Scripts.Managers;
using Assets.Scripts.Types;

public class LightMapProbesController : MonoBehaviour {

    PerfSceneManager PerformanceSceneManager = null;
    UIController UIControllerRef = null;

    void Awake()
    {
        PerformanceSceneManager = PerfSceneManager.GetInstance();
        Assert.AreNotEqual(null, PerformanceSceneManager);

        // Load UI scene and get a reference
        PerformanceSceneManager.LoadSceneAdditive(SCENE.UI);
    }

    void Start()
    {

        var canvasRoot = GameObject.FindGameObjectWithTag("UICanvasRoot");
        Assert.AreNotEqual(null, canvasRoot);

        UIControllerRef = canvasRoot.GetComponent<UIController>();
        Assert.AreNotEqual(null, UIControllerRef);

        UIControllerRef.SetUIMode(UIController.UI_MODE.WITHOUT_UI);
	}
}
