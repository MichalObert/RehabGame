using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TerrainAccessories : MonoBehaviour {
    public List<TreeInstance> TreeInstances;
    private Terrain terrain;
    // Use this for initialization
    private void Start() {
        terrain = Terrain.activeTerrain;
    }

    public void AddTree(Vector3 position){
        TerrainData terrainData = terrain.terrainData;
        
    //    Debug.Log("adding tree to " + terrain.terrainData.treeInstanceCount + " already existing");
        TreeInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
       // List<TreePrototype> treePrototypes = new List<TreePrototype>(terrain.terrainData.treePrototypes);
       // add tree with coords. maybe just Get treeindex instead of instance
            // This ray will see where we clicked er chopped
        // We hit the "terrain"! Now, how high is the ground at that point?
       // TreeInstance tree = new TreeInstance();
      //  tree.prototypeIndex = 0;
        //hardcoded 1000
        //TreeInstance tree = treeGO.GetComponent<TreeInstance>();
      //  Vector3 posInTerrain = new Vector3(position.x / terrain.terrainData.heightmapWidth, position.y / 1000, position.z / terrain.terrainData.heightmapHeight);
   //   //  GameObject treeGO = Instantiate(treePrototypes[0].prefab, new Vector3(position.x,position.z,position.y)  , Quaternion.identity) as GameObject;
      //  treeInstance has nothing to do with treeGO
        //  tree.position = posInTerrain;
      //  terrain.AddTreeInstance(tree);
    //    TreeInstances.Add(tree);
    //    Debug.Log(posInTerrain);
    //    Debug.Log(position);
        //terrain.AddTreeInstance(tree);
        // Remove the tree from the terrain tree list
        //Debug.Log("pos: " + tree.position);
        // Now refresh the terrain, getting rid of the collider
       
        var instance = new TreeInstance();
        instance.position = position;//new Vector3(position.x / terrain.terrainData.heightmapWidth, position.z / 1000, position.y / terrain.terrainData.heightmapHeight);
        instance.color = Color.white;
        instance.lightmapColor = Color.white;
        instance.prototypeIndex = 0;
        instance.heightScale = 1;
        instance.widthScale = 1;
 
     TreeInstances.Add(instance);
        terrainData.treeInstances = TreeInstances.ToArray();
        terrain.Flush();
        float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
        terrainData.SetHeights(0, 0, heights);
    }

    public bool removeTree(Vector3 position){
        TreeInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
        
    // This ray will see where we clicked er chopped

        // We hit the "terrain"! Now, how high is the ground at that point?
        float sampleHeight = Terrain.activeTerrain.SampleHeight(position);

        // If the height of the exact point we clicked/chopped at or below ground level, all we did
        // was chop dirt.
        /*if(position.y <= sampleHeight + 0.01f) {
            return;
        }*/

        TerrainData terrainData = terrain.terrainData;
        TreeInstance[] treeInstances = terrainData.treeInstances;

        // Our current closest tree initializes to far away
        //float maxDistance = float.MaxValue;
        float max2DDistance = float.MaxValue;
        // Track our closest tree's position
        Vector3 closestTreePosition = new Vector3();
        // Let's find the closest tree to the place we chopped and hit something
        int closestTreeIndex = 0;
        for(int i = 0; i < treeInstances.Length; i++) {
            TreeInstance currentTree = treeInstances[i];
            // The the actual world position of the current tree we are checking
            //Vector3 currentTreeWorldPosition = Vector3.Scale(currentTree.position, terrainData.size) + terrain.transform.position;

            // Find the distance between the current tree and whatever we hit when chopping
            //float distance = Vector3.Distance(currentTree.position, position);
            // !_ only 2D !!! (not a problem without caves)
            float distance2D = Vector2.Distance(new Vector2(currentTree.position.x, currentTree.position.z), 
                new Vector2(position.x, position.z));
            // Is this tree even closer?
            //if(distance < maxDistance) {
            //    maxDistance = distance;
                
            //}
            if(distance2D < max2DDistance) {
                max2DDistance = distance2D;
                closestTreeIndex = i;
                closestTreePosition = currentTree.position;
            }
        }
        if(max2DDistance >= 0.013f) {
            return false;
        }

        // Remove the tree from the terrain tree list
        TreeInstances.RemoveAt(closestTreeIndex);
        terrainData.treeInstances = TreeInstances.ToArray();

        // Now refresh the terrain, getting rid of the collider
        float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
        terrainData.SetHeights(0, 0, heights);
        return true;
        // Put a falling tree in its place
        //Instantiate(FallingTreePrefab, closestTreePosition, Quaternion.identity);
    }

    public void removeAllTrees() {
        TerrainData terrainData = terrain.terrainData;
        terrainData.treeInstances = new List<TreeInstance>().ToArray();

        // Now refresh the terrain, getting rid of the collider
        float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
        terrainData.SetHeights(0, 0, heights);

    }
}