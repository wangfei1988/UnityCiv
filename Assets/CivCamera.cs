using UnityEngine;
using System.Collections;

public class CivCamera : MonoBehaviour
{

    public GameObject zoomElementZ;
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public float zoomSpeed = 10.0F;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float translationForwardBackward = Input.GetAxis("Vertical");
        float translationSideways = Input.GetAxis("Horizontal");
        translationForwardBackward *= Time.deltaTime;
        translationSideways *= Time.deltaTime;

        float rotation = 0;

        if (Input.GetMouseButton(2))
        {
            rotation = Input.GetAxis("Mouse X") * 0.01f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            rotation = -translationSideways;
            translationSideways = 0;
        }
        // if middle mouse button is pressed

        transform.Rotate(0, rotation * rotationSpeed, 0);
        transform.Translate(translationSideways * speed, 0, translationForwardBackward * speed);

        // Zoom in and out
        zoomElementZ.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
    }
}