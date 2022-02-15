using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveViewer : MonoBehaviour
{

    public float speed = 0.1f;
    private void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");
        float yDirection = Input.GetAxis("UpDown");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, zDirection);

        transform.position += moveDirection * speed;

    }

}
