using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    private Rigidbody _rigidBody;

    public Image Steering;

    public WheelCollider[] WheelColliders;

    private bool _isBreaking;
    private bool _isForward;
    private bool _isBackward;
    private bool _isRight;
    private bool _isLeft;
    public float Transition;


    public GameObject MainCamera;
    public GameObject CameraFrontView;


    public Transform[] Wheels;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        //Camera Position Set
        cameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));

    }

    // Update is called once per frame
    void Update()
    {
        if (Accelarator.Accelerate)
        {
            _isForward = true;
        }
        else
        {
            _isForward = false;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            _isBackward = true;
        }
        else
        {
            _isBackward = false;
        }

        if (SimpleInputNamespace.SteeringWheel.IsRight)
        {
            _isRight = true;
        }
        else
        {
            _isRight = false;
        }

        if (SimpleInputNamespace.SteeringWheel.IsLeft)
        {
            _isLeft = true;
        }
        else
        {
            _isLeft = false;
        }

        if (Brake.Braking)
        {
            _isBreaking = true;
        }
        else
        {
            _isBreaking = false;
        }

        SyncWheelAndCollider();

        //Change Camera Keyboard
        switchCamera();

    }

    private void SyncWheelAndCollider()
    {

        for (int i = 0; i < WheelColliders.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            WheelColliders[i].GetWorldPose(out pos, out rot);

            Wheels[i].SetPositionAndRotation(pos, rot);
        }
    }

    private void FixedUpdate()
    {
        ApplyAcc();
        ApplyBreak();
    }

    private void ApplyAcc()
    {
        if (_isForward)
        {

            foreach (WheelCollider wheel in WheelColliders)

            {
                wheel.motorTorque = 5;
            }
        }

        if (_isBackward)
        {
            foreach (WheelCollider wheel in WheelColliders)
            {
                wheel.motorTorque = -5;
            }
        }

        if (!_isBackward & !_isForward)
        {
            foreach (WheelCollider wheel in WheelColliders)
            {
                wheel.motorTorque = 0;
            }
        }

        if (_isRight)
        {
            WheelColliders[0].steerAngle = 15;
            WheelColliders[1].steerAngle = 15;
        }

        if (_isLeft)
        {
            WheelColliders[0].steerAngle = -15;
            WheelColliders[1].steerAngle = -15;
        }

        if (!_isLeft && !_isRight)
        {
            float angle = Mathf.MoveTowards(WheelColliders[0].steerAngle, 0f, Transition * Time.deltaTime);
            //            float angle = Mathf.Lerp(WheelColliders[0].steerAngle, 0f, Transition);
            WheelColliders[0].steerAngle = angle;
            WheelColliders[1].steerAngle = angle;

        }
    }

    void ApplyBreak()
    {
        if (_isBreaking)
        {

            foreach (WheelCollider wheel in WheelColliders)
            {
                wheel.brakeTorque = 5;
            }
        }
        else
        {

            foreach (WheelCollider wheel in WheelColliders)
            {
                wheel.brakeTorque = 0;
            }
        }
    }


    //UI JoyStick Method
    public void cameraPositonM()
    {
        cameraChangeCounter();
    }

    //Change Camera Keyboard
    void switchCamera()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            cameraChangeCounter();
        }
    }

    //Camera Counter
    void cameraChangeCounter()
    {
        int cameraPositionCounter = PlayerPrefs.GetInt("CameraPosition");
        cameraPositionCounter++;
        cameraPositionChange(cameraPositionCounter);
    }

    //Camera change Logic
    void cameraPositionChange(int camPosition)
    {
        if (camPosition > 1)
        {
            camPosition = 0;
        }

        //Set camera position database
        PlayerPrefs.SetInt("CameraPosition", camPosition);

        //Set camera position 1
        if (camPosition == 0)
        {
            MainCamera.SetActive(true);

            CameraFrontView.SetActive(false);
        }

        //Set camera position 2
        if (camPosition == 1)
        {
            CameraFrontView.SetActive(true);

            MainCamera.SetActive(false);
        }
    }

    public void ImageClick()
    {
        MonoBehaviour.print("image clicked");
    }
}