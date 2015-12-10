using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TerrainAccessories : MonoBehaviour {
    //public List<TreeInstance> TreeInstances;
    //private Terrain terrain;
    //private Transform targeter;
   // private Vector3 up;
    // Use this for initialization
    private void Start() {
      //  terrain = Terrain.activeTerrain;
        //targeter = GameObject.Find("Targeter").transform;
      //  up = Vector3.up;
    }


    public void AddTree(/*Vector3 position, */Vector3 worldPosition){

        GameObject wall = (GameObject)Instantiate(Resources.Load("Prefabs/StoneWall"));
        wall.transform.position = worldPosition;
        wall.transform.LookAt(ChangingHeights.Instance.ball);
        wall.transform.parent = ChangingHeights.Instance.terrain.transform;
        wall.gameObject.tag = "Accessory";
        wall.transform.position += new Vector3(0,wall.GetComponent<BoxCollider>().bounds.size.y / 2,0);
        /*TerrainData terrainData = terrain.terrainData;
        
        TreeInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
       
        var instance = new TreeInstance();
        instance.position = position + Vector3.up;
        instance.color = Color.white;
        instance.lightmapColor = Color.white;
        instance.prototypeIndex = 0;
        instance.heightScale = 3.3f;
        instance.widthScale = 2;
        targeter.position = worldPosition;
        targeter.LookAt(ChangingHeights.Instance.ball);
        instance.rotation = targeter.eulerAngles.y;
        instance.rotation *= Mathf.Deg2Rad;   
        TreeInstances.Add(instance);
        terrainData.treeInstances = TreeInstances.ToArray();
        terrain.Flush();
        float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
        terrainData.SetHeights(0, 0, heights);
        */
    }

    public void removeTree(GameObject tree){
        Destroy(tree);
       /* TreeInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
        
    // This ray will see where we clicked er chopped

        // We hit the "terrain"! Now, how high is the ground at that point?
        float sampleHeight = Terrain.activeTerrain.SampleHeight(position);

        // If the height of the exact point we clicked/chopped at or below ground level, all we did
        // was chop dirt.
        /*if(position.y <= sampleHeight + 0.01f) {
            return;
        }         ///*

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
            float distance2D = Vector2.Distance(new Vector2(currentTree.position.x, currentTree.position.z), 
                new Vector2(position.x, position.z));
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

        */
        // Put a falling tree in its place
        //Instantiate(FallingTreePrefab, closestTreePosition, Quaternion.identity);
    }

   /* public void removeAllTrees() {
        TerrainData terrainData = terrain.terrainData;
        terrainData.treeInstances = new List<TreeInstance>().ToArray();

        // Now refresh the terrain, getting rid of the collider
        float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
        terrainData.SetHeights(0, 0, heights);

    }*/
}