using UnityEngine;
using UnityEditor;

public class TerrainMenu {

    [MenuItem("RehabGame/LoadNextTerrain")]
    private static void LoadNextTerrain() {
        //Adding Terrain prefab will screw this up...
        Terrain[] terrains = Resources.FindObjectsOfTypeAll(typeof(Terrain)) as Terrain[];
        int activeTerrain = 0;
        for(int i = 0; i < terrains.Length; i++) {
            if(terrains[i].gameObject.activeInHierarchy == true) {
                activeTerrain = i;
            }
            terrains[i].gameObject.SetActive(false);
        }
        terrains[(activeTerrain + 1) % terrains.Length].gameObject.SetActive(true);
        Debug.Log(terrains[(activeTerrain + 1) % terrains.Length].gameObject.name + " activated");
    }

    [MenuItem("RehabGame/SaveTerrain")]
    private static void SaveTerrain() {
        Debug.Log("Terrain Saved");
    }

}
