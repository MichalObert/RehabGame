using UnityEngine;
using System.Collections;

public class ResetingTerrain : MonoBehaviour {

 public Terrain Terrain;

    public float[,] originalHeights;



    private void OnDestroy() {
        this.Terrain.terrainData.SetHeights(0, 0, this.originalHeights);

    }

    private void Start() {
       // RaiseEverything(0.01f);
        originalHeights = this.Terrain.terrainData.GetHeights(
            0, 0, this.Terrain.terrainData.heightmapWidth, this.Terrain.terrainData.heightmapHeight);

        
        
    }
    private void RaiseEverything(float height) {
        float[,] heights = Terrain.terrainData.GetHeights(0, 0, Terrain.terrainData.heightmapWidth, Terrain.terrainData.heightmapHeight);
        // we set each sample of the terrain in the size to the desired height and in the shape of a circle
        for(int i = 0; i < Terrain.terrainData.heightmapWidth; i++) {
            for(int j = 0; j < Terrain.terrainData.heightmapHeight; j++) {
                    heights[i, j] += height;
                
            }
        }
        this.Terrain.terrainData.SetHeights(0, 0, heights);
    }

}