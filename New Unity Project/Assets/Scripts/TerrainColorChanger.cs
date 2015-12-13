using UnityEngine;
using System.Collections;
using System.Linq; // used for Sum of array

public class TerrainColorChanger : MonoBehaviour {
    public Terrain terrain;
    private TerrainData terrainData;
    public float currentHeight2;
    public Vector2 pos;
    public Vector2 pos2;
    void Start() {

        terrainData = terrain.terrainData;

        recolorSquare(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
    }

    public void recolorSquare(int x, int y, int width, int height) {
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[, ,] splatmapData = new float[width, height, terrainData.alphamapLayers];

        for(int heightPosition = y; heightPosition < height + y; heightPosition++) {
            for(int widthPosition = x; widthPosition < width + x; widthPosition++) {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)heightPosition / (float)terrainData.alphamapHeight; 
                float x_01 = (float)widthPosition / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
             //  x and y was reversed. Or not. Who knows.
                float currentHeight;
                if(width < 100) {
                    currentHeight = terrainData.GetHeight(Mathf.RoundToInt(x_01 * terrainData.heightmapWidth), Mathf.RoundToInt(y_01 * terrainData.heightmapHeight));
                } else {
                    currentHeight = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));
                }
                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];
                //if(width < 20)Debug.Log("pos:" + y_01 + "," + x_01 + "; cur: " + currentHeight);
                //if(width < 20) {
                  //  Debug.Log("pos:" + Mathf.RoundToInt(x_01 * terrainData.heightmapWidth) + "," + Mathf.RoundToInt(y_01 * terrainData.heightmapHeight));
                //}
                if(currentHeight < 22) {
                    splatWeights[0] = 1;
                    splatWeights[1] = 0;
                    if(splatWeights.Length > 2) {
                        splatWeights[2] = 0;
                    }
                }
                if(currentHeight >= 22 && currentHeight <= 29) {
                    splatWeights[0] = Mathf.Clamp01((29 - currentHeight)/29);
                    splatWeights[1] = Mathf.Clamp01(currentHeight/29);
                    if(splatWeights.Length > 2) {
                        splatWeights[2] = 0;
                    }
                }
                if(currentHeight > 29 && currentHeight < 35) {
                    splatWeights[0] = 0;
                    splatWeights[1] = 1;
                    if(splatWeights.Length > 2) {
                        splatWeights[2] = 0;
                    }
                }
                if(currentHeight >= 35 && currentHeight <= 100) {
                    splatWeights[0] = 0;
                    splatWeights[1] = Mathf.Clamp01(/*terrainData.heightmapHeight */(100 - currentHeight)/100);
                    if(splatWeights.Length > 2) {
                        splatWeights[2] = Mathf.Clamp01(currentHeight / 100);
                    }
           //         does not work as it should for smaller squares. Test with debugs here
                }
                if(currentHeight > 100) {
                    splatWeights[0] = 0;
                    splatWeights[1] = 0;
                    if(splatWeights.Length > 2) {
                        splatWeights[2] = 1;
                    } else {
                        splatWeights[1] = 1;
                    }
                }
              //  Debug.Log("currentHeight: " + currentHeight);
                //ADDING STEEPNESS AND NORMALS. Currently unused
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                //              splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                //                splatWeights[3] = height * Mathf.Clamp01(normal.z);


                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for(int i = 0; i < terrainData.alphamapLayers; i++) {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    if(width < 100) {
                        splatmapData[heightPosition - y, widthPosition - x, i] = splatWeights[i];
                    } else {
                        splatmapData[widthPosition - x, heightPosition - y, i] = splatWeights[i];

                    }

                }
            }
        }
      /*  string message = "";
        if(message != "s") {
            for(int i = 0; i < splatmapData.GetLength(0); i++) {
                for(int j = 0; j < splatmapData.GetLength(1); j++) {
                    message += (splatmapData[i, j, 2]) + ";";//.ToString("F");
                }
                //message += '\n';
            }
            Debug.Log(message);
            message = "s";
        }*/
        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(x, y, splatmapData);
    }
}

