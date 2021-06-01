using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class CameraControl : MonoBehaviour
{
    #region Variable Declaration
    public Transform Target;//transform is used to get position, rotation and scale
    public float LookSmooth = 0.09f;
    public float XTilt = 10;

    Vector3 _destination = Vector3.zero;// if not coliding
                                        //CharacterController charController;

    Vector3 _adjustedDestination = Vector3.zero;//if colliding
    Vector3 _camVel = Vector3.zero;

    // The distance in the x-z plane to the Target
    public float Distance = 10.0f;
    // the height we want the camera to be above the Target
    public float Height = 2.0f;
    // How much we 
    public float HeightDamping = 2.0f;
    public float RotationDamping = 3.0f;

    public float MinDistance = 1.0f;
    public float MaxDistance = 4.0f;

    #endregion

    #region Camera Position setting variables
    [System.Serializable]
    public class PositionSettings
    {

        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);

        public Vector3 TargetPosOffset = new Vector3(0, 3.4f, 0);

        public float distanceFromTarget = -0.01f;
        public float zoomSmooth = 100f;
        public float zoomStep = 2;
        public float maxZoom = -2;
        public float minZoom = -15;
        public bool smoothFollow = true;
        public float smooth = 0.05f;


        [HideInInspector]
        public float newDistance = -8;//set by zoom input
        [HideInInspector]
        public float adjustmentDistance = -8;
    }
    #endregion

    #region settings for debgging
    //to check the casted ray
    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }
    #endregion

    #region class CollisionHandler
    [System.Serializable]//for re use
    public class CollisionHandler // checks if something comes in between 
    {
        #region variable declaration
        public LayerMask collisionLayer;

        [HideInInspector]//Makes a variable not show up in the inspector but be serialized.
        public bool colliding = false;
        [HideInInspector]
        public Vector3[] adjustedCameraClipPoints;//clip points that surrounds the camera's current position
        [HideInInspector]
        public Vector3[] desiredCameraClipPoints;

        private Camera camera;
        #endregion

        #region value initialization
        public void Initialize(Camera cam)
        {
            camera = cam;
            adjustedCameraClipPoints = new Vector3[5];// 4 near clip points, 1 camera's position
            desiredCameraClipPoints = new Vector3[5];
        }
        #endregion

        #region Get updated camera clip points
        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)//as camera moves around calculate x,y,z
        {
            if (!camera)
                return;

            //clear the contents of intoArray;
            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;// can change 3.41f value
            float y = x / camera.aspect;

            //top left
            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;// added and rotated the point relative to camera

            //top right
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;// added and rotated the point relative to camera

            //bottom left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;// added and rotated the point relative to camera

            //bottom right
            intoArray[3] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;// added and rotated the point relative to camera

            //camera's position
            intoArray[4] = cameraPosition - camera.transform.forward;//lil space behind camera for collision
        }
        #endregion


        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if (Physics.Raycast(ray, distance, collisionLayer))
                {
                    return true;
                }
            }
            return false;
        }

        public float GetAdjustedDistanceWithRayFrom(Vector3 from)//return the distance that camera needs to move forward towards Target

        {
            float distance = -1;

            for (int i = 0; i < desiredCameraClipPoints.Length; i++)//since there are 5 collision points
            {
                Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (distance == -1)
                        distance = hit.distance;
                    else
                    {
                        if (hit.distance < distance)
                            distance = hit.distance;
                    }
                }
            }

            if (distance == -1)
                return 0;
            else
                return distance;
        }


        public void CheckColliding(Vector3 TargetPos)//updating collision bool
        {
            if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, TargetPos))

            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }
        }


    }

    #endregion

    public PositionSettings position = new PositionSettings();
    public DebugSettings debug = new DebugSettings();
    public CollisionHandler collision = new CollisionHandler();

    // Start is called before the first frame update
    void Start()
    {
        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(_destination, transform.rotation, ref collision.desiredCameraClipPoints);
   

    }
    private void FixedUpdate()
    {
        MoveToTarget();        
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(_destination, transform.rotation, ref collision.desiredCameraClipPoints);


        //draw debug lines, ray cast to Target from camera
        for (int i = 0; i < 5; i++)
        {
            if (debug.drawDesiredCollisionLines)
            {
                Debug.DrawLine(Target.position, collision.desiredCameraClipPoints[i], Color.white);
            }
            if (debug.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(Target.position, collision.adjustedCameraClipPoints[i], Color.green);
            }
        }

        collision.CheckColliding(Target.position);//using raycast
        position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(Target.position);
    }   
    void MoveToTarget()
    {
        #region camera follow     
        if (!Target)
            return;
        // Calculate the current rotation angles
        var wantedRotationAngle = Target.eulerAngles.y;
        var wantedHeight = Target.position.y + Height;
        var currentRotationAngle = transform.eulerAngles.y;
        var currentHeight = transform.position.y;
        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotationDamping * Time.deltaTime);
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the Target
        transform.position = Target.position;
        transform.position -= currentRotation * Vector3.forward * Distance;

        // Set the height of the camera
        Vector3 pos = transform.position;
        pos.y = currentHeight;
        transform.position = pos;

        // Always look at the Target
        transform.LookAt(Target);
        #endregion

        _destination = Quaternion.Euler(transform.rotation.x, transform.rotation.y + Target.eulerAngles.y, 0) * Vector3.forward * position.distanceFromTarget;
        _destination += transform.position;
        if (collision.colliding)
        {
            _adjustedDestination = Quaternion.Euler(transform.rotation.x, transform.rotation.y + Target.eulerAngles.y, 0) * -Vector3.forward * position.adjustmentDistance;
            _adjustedDestination += Target.position;

            if (position.smoothFollow)
            {
                pos = Vector3.SmoothDamp(transform.position, _adjustedDestination, ref _camVel, position.smooth);
                pos.y = currentHeight;
                transform.position = pos;
            }
            else
            {
                pos = _adjustedDestination;
                pos.y = currentHeight;
                transform.position = pos;
            }
           
            // Always look at the Target
            transform.LookAt(Target);
        }
    }
}
