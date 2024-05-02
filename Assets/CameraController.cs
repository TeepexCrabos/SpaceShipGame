using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform MyShip; // Le vaisseau que la caméra suivra
    public Transform LoockedTarget;
    public float distance = 10.0f; // Distance de la caméra par rapport au vaisseau
    public float height = 5.0f; // Hauteur de la caméra par rapport au vaisseau
    public float mouseSensitivity = 2.0f; // Sensibilité de la souris

    private float rotationX = 0.0f; // Rotation de la caméra autour de l'axe X
    private float rotationY = 0.0f; // Rotation de la caméra autour de l'axe Y
    public CameraView cameraView;
    private CameraView lastCameraView;
    Vector3 desiredPosition;
    public float zoomSpeed;
    public float minDistance;
    public float maxDistance;
    public Transform mapCenter;
    private Camera camera;
    private bool tacticViewInit = false;
    private bool cameraAnimed = false;
    public float lerp = 0;
    public float lerpSpeed;
    private LayerMask layerMask;




    private void Awake()
    {
        camera = GetComponent<Camera>();
        layerMask |= (1 << LayerMask.NameToLayer("Default"));
        layerMask |= (1 << LayerMask.NameToLayer("SpaceShip"));
        layerMask |= (1 << LayerMask.NameToLayer("UI"));
    }
    public enum CameraView
    {
        FreeView,
        CurserView,
        Locked,
        Tactical,
        SpacialView,
    }

    private void Start()
    {
        Cursor.visible = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            switch (cameraView)
            {
                case CameraView.FreeView:
                    lastCameraView = cameraView;
                    cameraView = CameraView.CurserView;
                    Cursor.visible = true; // Affiche le curseur de la souris
                    break;
                case CameraView.CurserView:
                    lastCameraView = cameraView;
                    cameraView = CameraView.FreeView;
                    Cursor.visible = false;
                    break;
                default:

                    break;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && cameraAnimed == false)
        {
            ZoomCamera(scroll);
        }

        if (cameraView == CameraView.Tactical && distance < 1000)
        {

            cameraView = lastCameraView;
            if (tacticViewInit)
            {
                layerMask &= ~(1 << LayerMask.NameToLayer("MapUI"));
                layerMask |= (1 << LayerMask.NameToLayer("SpaceShip"));
                layerMask |= (1 << LayerMask.NameToLayer("EnemieSpaceShip"));
                layerMask |= (1 << LayerMask.NameToLayer("UI"));
                layerMask |= (1 << LayerMask.NameToLayer("Enviro"));
                camera.cullingMask = layerMask;
                camera.orthographic = false;
                tacticViewInit = false;
            }
        }
        if (distance >= 1000 && distance < 2000 && cameraView != CameraView.Tactical)
        {
            if (cameraView != CameraView.SpacialView)
            {
                lastCameraView = cameraView;
            }
            cameraView = CameraView.Tactical;
            layerMask &= ~(1 << LayerMask.NameToLayer("SpaceShip"));
            layerMask &= ~(1 << LayerMask.NameToLayer("EnemieSpaceShip"));
            layerMask &= ~(1 << LayerMask.NameToLayer("UI"));
            layerMask &= ~(1 << LayerMask.NameToLayer("Enviro"));
            layerMask |= (1 << LayerMask.NameToLayer("MapUI"));
            camera.cullingMask = layerMask;
            StartCoroutine(AnimCamera());


        }
        else if (distance > 2000)
        {
            cameraView = CameraView.SpacialView;
        }


    }

    IEnumerator AnimCamera()
    {
        cameraAnimed = true;
        while (lerp <= 1)
        {
            lerp += Time.deltaTime * lerpSpeed;
            transform.position = Vector3.Lerp(transform.position, MyShip.position + Vector3.up * distance - Vector3.forward, lerp);
            transform.LookAt(MyShip.position);
            yield return null;
        }
        cameraAnimed = false;
        lerp = 0;
        yield return null;
    }

    void LateUpdate()
    {
        if (!MyShip)
            return;
        switch (cameraView)
        {
            case CameraView.Locked:
                Vector3 Hauteur = new Vector3(0, height, 0);
                desiredPosition = MyShip.position - transform.forward * distance;
                transform.position = desiredPosition + Hauteur;
                transform.LookAt(LoockedTarget);
                break;

            case CameraView.CurserView:
                if (Input.mousePosition.x <= 0 || Input.mousePosition.x >= Screen.width)
                {
                    PositionXPlusOrLess();

                }
                if (Input.mousePosition.y <= 0 || Input.mousePosition.y >= Screen.height)
                {
                    PositionYPlusOrLess();
                }

                desiredPosition = MyShip.position - transform.forward * distance;
                transform.position = desiredPosition;
                transform.LookAt(MyShip.position);
                break;

            case CameraView.FreeView:
                PositionXCalcul();
                PositionYCalcul();
                desiredPosition = MyShip.position - transform.forward * distance;
                transform.position = desiredPosition;

                transform.LookAt(MyShip.position);
                break;

            case CameraView.Tactical:
                if (!tacticViewInit && cameraAnimed == false)
                {
                    camera.orthographic = true;
                    //transform.position = MyShip.position + Vector3.up * distance - Vector3.forward;
                    //transform.LookAt(MyShip.position);
                    tacticViewInit = true;
                }
                // Positionnez la caméra au-dessus de la carte en regardant vers le centre de la carte
                camera.orthographicSize = distance / 2;

                if (Input.mousePosition.x <= 0 || Input.mousePosition.x >= Screen.width)
                {
                    PositionXStrategiqueView();

                }

                if (Input.mousePosition.y <= 0 || Input.mousePosition.y >= Screen.height)
                {
                    PositionZStrategiqueView();
                }
                break;
            case CameraView.SpacialView:
                camera.orthographicSize = distance / 2;
                if (Input.mousePosition.x <= 0 || Input.mousePosition.x >= Screen.width)
                {
                    PositionXStrategiqueView();

                }

                if (Input.mousePosition.y <= 0 || Input.mousePosition.y >= Screen.height)
                {
                    PositionZStrategiqueView();
                }
                break;
        }
    }

    private void PositionYCalcul()
    {
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.RotateAround(MyShip.position, Vector3.right, mouseY);

    }

    private void PositionYPlusOrLess()
    {
        if (Input.mousePosition.y <= 0)
        {
            transform.RotateAround(MyShip.position, Vector3.right, -0.5f);
        }
        else
        {
            transform.RotateAround(MyShip.position, Vector3.right, 0.5f);
        }

    }

    private void PositionZStrategiqueView()
    {
        if (Input.mousePosition.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4);
        }
    }

    private void PositionXCalcul()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.RotateAround(MyShip.position, Vector3.up, mouseX);
    }

    private void PositionXPlusOrLess()
    {
        if (Input.mousePosition.x <= 0)
        {
            transform.RotateAround(MyShip.position, Vector3.up, -0.5f);
        }
        else
        {
            transform.RotateAround(MyShip.position, Vector3.up, 0.5f);
        }

    }

    private void PositionXStrategiqueView()
    {
        if (Input.mousePosition.x <= 0)
        {
            transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + 4f, transform.position.y, transform.position.z);
        }
    }

    private void ZoomCamera(float scrollAmount)
    {
        distance -= scrollAmount * zoomSpeed; // Ajuster la distance en fonction du mouvement de la molette de la souris

        // Limiter la distance pour éviter que la caméra ne soit trop proche ou trop éloignée
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }
}