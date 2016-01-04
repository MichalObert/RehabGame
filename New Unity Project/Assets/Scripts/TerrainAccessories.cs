using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TerrainAccessories : MonoBehaviour {
    private void Start() {
    }


    public void AddTree(Vector3 worldPosition){
        GameObject tree;
        if(Application.loadedLevel != 5) {
            tree = (GameObject)Instantiate(Resources.Load("Prefabs/StoneWall"));
            tree.transform.position = worldPosition;
            tree.transform.position += new Vector3(0, tree.GetComponent<BoxCollider>().bounds.size.y / 2, 0);
            tree.transform.LookAt(ChangingHeights.Instance.ball);
            tree.transform.eulerAngles = new Vector3(0, tree.transform.eulerAngles.y, 0);
        } else {
            tree = (GameObject)Instantiate(Resources.Load("Prefabs/Tree" + UnityEngine.Random.Range(0, 3)));
            tree.transform.position = worldPosition;
            tree.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0, 360));    
        }
        tree.gameObject.tag = "Accessory";
        tree.transform.parent = ChangingHeights.Instance.terrain.transform;
    }

    public void removeTree(GameObject tree){
        Destroy(tree);
    }
 
}