using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class SpaceShipController : MonoBehaviour
{
    public float shipMass;

    public int shipPower;
    private int shipPowerMax = 100;
    private int shipPowerMin = -100;

    public float shipPowerAccelerate;

    public float shipMaxSpeed;
    public float shipMinSpeed;
    public float shipSpeed;
    private float shipSpeedTarget;

    public float DragPower;

    private string verticalAxis = "Vertical";
    private string horizontalAxis = "Horizontal";


    private float horizontalInput;
    private float verticalInput;
    private float rightStickHorizontalInput;

    private bool coroutineStart = false;
    private string lastAction;
    private Rigidbody rb;
    public float rotationSpeed;
    private Quaternion Newrotation;
    public float derivPower;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetRigidbodyMass();
        SetRigidbodyDrag();
        SetPowerAccelerate();





    }


    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rightStickHorizontalInput = Input.GetAxis("RightStickHorizontal");

        if (verticalInput >= 1)
        {
            if (lastAction != "MoteurPowerPlus")
            {
                coroutineStart = false;
            }
            if (coroutineStart == false)
            {
                lastAction = "MoteurPowerPlus";
                StartCoroutine(MoteurPowerPlus());
                coroutineStart = true;
            }
        }

        if (verticalInput <= -1)
        {
            if (lastAction != "MoteurPowerMoins")
            {
                coroutineStart = false;
            }
            if (coroutineStart == false)
            {
                lastAction = "MoteurPowerMoins";
                StartCoroutine(MoteurPowerMoins());
                coroutineStart = true;
            }
        }
    }


    private void FixedUpdate()
    {
        shipSpeed = rb.velocity.magnitude;

        //rotation du rigidbody
        if (rightStickHorizontalInput >= 0.2f || rightStickHorizontalInput <= -0.2f)
        {
            Newrotation = Quaternion.Euler(0f, rightStickHorizontalInput * rotationSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * Newrotation);
        }

        if (horizontalInput >= 0.2f || horizontalInput <= -0.2f)
        {
            Vector3 localMovement = transform.right * horizontalInput * derivPower;
            rb.AddForce(localMovement, ForceMode.Force);
        }

        if (shipPower > 0)
        {
            Vector3 localMovement = transform.forward * shipPower * (shipPowerAccelerate / 100);
            Vector3 globalMovement = transform.TransformDirection(localMovement);
            if (shipSpeed < shipSpeedTarget)
            {

                rb.AddForce(localMovement, ForceMode.Acceleration);
            }

        }
        else if (shipPower < 0)
        {
            Vector3 localMovement = transform.forward * shipPower * (shipPowerAccelerate / 100);
            Vector3 globalMovement = transform.TransformDirection(localMovement);
            if (-shipSpeed > shipSpeedTarget)
            {
                rb.AddForce(localMovement, ForceMode.Acceleration);
            }
        }
    }

    IEnumerator MoteurPowerPlus()
    {

        if (shipPower < shipPowerMax)
        {
            shipPower += 25;
        }

        switch (shipPower)
        {
            case 100:
                shipSpeedTarget = shipMaxSpeed;
                break;
            case 75:
                shipSpeedTarget = (shipMaxSpeed / 100) * 75;
                break;
            case 50:
                shipSpeedTarget = (shipMaxSpeed / 100) * 50;
                break;
            case 25:
                shipSpeedTarget = (shipMaxSpeed / 100) * 25;
                break;
            case 0:
                shipSpeedTarget = 0;
                break;
            case -100:
                shipSpeedTarget = shipMinSpeed;
                break;
            case -75:
                shipSpeedTarget = (shipMinSpeed / 100) * 75;
                break;
            case -50:
                shipSpeedTarget = (shipMinSpeed / 100) * 50;
                break;
            case -25:
                shipSpeedTarget = (shipMinSpeed / 100) * 25;
                break;
        }

        yield return new WaitForSeconds(0.5f);
        coroutineStart = false;
    }

    IEnumerator MoteurPowerMoins()
    {

        if (shipPower > shipPowerMin)
        {
            shipPower -= 25;
        }
        switch (shipPower)
        {
            case -100:
                shipSpeedTarget = shipMinSpeed;
                break;
            case -75:
                shipSpeedTarget = (shipMinSpeed / 100) * 75;
                break;
            case -50:
                shipSpeedTarget = (shipMinSpeed / 100) * 50;
                break;
            case -25:
                shipSpeedTarget = (shipMinSpeed / 100) * 25;
                break;
            case 100:
                shipSpeedTarget = shipMaxSpeed;
                break;
            case 75:
                shipSpeedTarget = (shipMaxSpeed / 100) * 75;
                break;
            case 50:
                shipSpeedTarget = (shipMaxSpeed / 100) * 50;
                break;
            case 25:
                shipSpeedTarget = (shipMaxSpeed / 100) * 25;
                break;
            case 0:
                break;
        }
        yield return new WaitForSeconds(0.5f);
        coroutineStart = false;
    }

    public void SetRigidbodyMass()
    {
        rb.mass = shipMass;
    }

    private void SetRigidbodyDrag()
    {
        rb.drag = DragPower / rb.mass;
    }

    private void SetPowerAccelerate()
    {
        shipPowerAccelerate = (100 / shipMass) * 100;
    }

}
