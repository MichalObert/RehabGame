using UnityEngine;
using System.Collections;

public class EditorEnabler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ChangingHeights.Instance.Mode = ChangingHeights.Modes.Editor;
        Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
