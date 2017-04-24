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
using System.Collections;

public class RenderableManager : MonoBehaviour {

    public float MaxItems;
    public float MaxItemsLowCount = 1;
    public float HorizontalBounds;
    public float VerticalBounds;

    public GameObject[] PrefabsToSpawn;
    float NumItemsPerRow = 0;

    public float Spacing;
    ArrayList Renderables = new ArrayList();

    public bool IsLowCountScene = false;

	void Start () {
        if(IsLowCountScene)
            NumItemsPerRow = MaxItemsLowCount;
        else
            NumItemsPerRow = Mathf.Sqrt(MaxItems);
        Assert.AreNotEqual(0, NumItemsPerRow);

        int currentPrefabToSpawn = 0;
        Assert.AreNotEqual(0, PrefabsToSpawn.Length);

        Vector3 currentSpawnPos = Vector3.zero;

        for(int i = 0; i < NumItemsPerRow; i++)
        {
            for(int j = 0; j < NumItemsPerRow; j++)
            {
                currentSpawnPos.x = i * Spacing +
                    -(NumItemsPerRow / 2) * Spacing;
                currentSpawnPos.z = j * Spacing +
                    -(NumItemsPerRow / 2) * Spacing;
                var newObject = Instantiate(PrefabsToSpawn[currentPrefabToSpawn], currentSpawnPos, PrefabsToSpawn[currentPrefabToSpawn].transform.rotation) as GameObject;
                newObject.transform.SetParent(gameObject.transform);
                Renderables.Add(newObject);

                currentPrefabToSpawn++;
                if(currentPrefabToSpawn >= PrefabsToSpawn.Length)
                {
                    currentPrefabToSpawn = 0;
                }
            }
        }
	}
}
