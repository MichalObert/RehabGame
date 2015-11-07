using UnityEngine;
using System.Collections;

public class moveBall : MonoBehaviour {

    public float speed;
    private Rigidbody rigidBody;

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
        speed = 10;
        rigidBody.AddForce (movement * speed);
    }
}
