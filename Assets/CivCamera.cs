using UnityEngine;
using System.Collections;

public class CivCamera : MonoBehaviour
{

    public GameObject zoomElementZ;
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public float zoomSpeed = 10.0F;

    private Vector3? isAutoMovingTowards = null;
    private float currentAutoMoveSpeed = 0;
    private float maxAutoMoveSpeed = 100f;
    private float startDeceleratingAt = 50f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool cancelAutoMovement = false;

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
        zoomElementZ.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime);

        // cancel the auto movement if the player moves the camera
        cancelAutoMovement = cancelAutoMovement || (translationForwardBackward > 0 || translationSideways > 0);
        if (cancelAutoMovement || (isAutoMovingTowards.HasValue && transform.position == isAutoMovingTowards.Value))
        {
            isAutoMovingTowards = null;
            currentAutoMoveSpeed = 0;
        }

        if (isAutoMovingTowards.HasValue)
        {
            var sqrDist = (isAutoMovingTowards.Value - transform.position).sqrMagnitude;

            if (sqrDist > startDeceleratingAt)
            {
                var speedIncrease = (maxAutoMoveSpeed - currentAutoMoveSpeed) * 15f * Time.deltaTime;
                currentAutoMoveSpeed += speedIncrease;
            }
            else
            {
                var nearGoalSlow = (1f - (startDeceleratingAt - sqrDist) / startDeceleratingAt) * 13f * Time.deltaTime + 0.5f;
                currentAutoMoveSpeed = Mathf.Max(maxAutoMoveSpeed * nearGoalSlow, 4f);
            }
            transform.position = Vector3.MoveTowards(transform.position, isAutoMovingTowards.Value, currentAutoMoveSpeed * Time.deltaTime);
        }
    }

    public void FlyToTarget(Vector3 target)
    {
        isAutoMovingTowards = target;
    }
}