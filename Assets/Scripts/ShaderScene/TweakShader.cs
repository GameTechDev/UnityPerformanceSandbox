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

public class TweakShader : MonoBehaviour {

    MeshRenderer SphereRenderer = null;
    Color ReplacementColor = Color.black;

    const float MaxSpeed = 10.0f;
    [Range(1.0f, MaxSpeed)]
    public float ColorSwitchSpeed;
    
    void Start () {
        SphereRenderer = GetComponent<MeshRenderer>();
        Assert.AreNotEqual(null, SphereRenderer);
	}
	
	void Update () {
        float newVal = Time.time * (1 / (MaxSpeed - ColorSwitchSpeed)) % 1;
        ReplacementColor.r = newVal;
        ReplacementColor.g = newVal;
        ReplacementColor.b = newVal;
        SphereRenderer.material.SetColor("_DiffuseColor", ReplacementColor);
    }
}