using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFrontView : MonoBehaviour
{
    public Transform Target;//transform is used to get position, rotation and scale
                            // Start is called before the first frame update

    // The distance in the x-z plane to the Target
    public float Distance = 5.0f;
    // the height we want the camera to be above the Target
    public float Height = 2.0f;
    // How much we 
    public float HeightDamping = 2.0f;
    public float RotationDamping = 3.0f;
    Camera tisCamera;

    void Start()
    {
        tisCamera = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        float z = tisCamera.nearClipPlane;
        //yo code le camera follow garda paxadi janxa but i want the camera to stay where it is
        if (!Target)
            return;
        // Calculate the current rotation angles
        var wantedRotationAngle = Target.eulerAngles.y + 180;
        var wantedHeight = Target.position.y + Height;
        var currentRotationAngle = transform.eulerAngles.y;
        var currentHeight = transform.position.y;
        var currentDistance = transform.position.z;
        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotationDamping * Time.deltaTime);
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the Target
        transform.position = Target.position;
        transform.position += currentRotation * Vector3.back * Distance;
        // Set the height of the camera
        Vector3 pos = transform.position;
        pos.y = currentHeight;
        //pos.z = currentDistance;
        transform.position = new Vector3(pos.x, pos.y, pos.z);
        // Always look at the Target
        transform.LookAt(Target);
    }
}
