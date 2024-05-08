using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;


public class DetectionSystem : MonoBehaviour
{
    public GameObject myShip;

    public Collider[] detectedColliderTab;
    public List<Collider> detectedCollidersList;
    public float porteDeDetectionMax;


    public bool systemVisuel = true;
    public float porteDuSystemVisuel;
    public List<Collider> detectedBySystemVisuel;
    public List<Collider> lastDetectedBySystemVisuel;

    public bool systemPassif = true;
    public float porteDuSystemPassif;
    public List<Collider> detectedBySystemPassif;
    public List<Collider> lastDetectedBySystemPassif;

    public KeyCode inputForActiveSystemActif;
    public bool systemActif = false;
    public float porteDuSystemActif;
    public List<Collider> detectedBySystemActif;
    public List<Collider> lastDetectedBySystemActif;
    public LayerMask layerMaskForRaycast;
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        porteDeDetectionMax = Mathf.Max(porteDuSystemActif, porteDuSystemPassif, porteDuSystemVisuel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(inputForActiveSystemActif))
        {
            switch (systemActif)
            {
                case true:
                    systemActif = false;
                    break;
                case false:
                    systemActif = true;
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        Detection();
        if (detectedCollidersList != null)
        {
            Analyse();
        }

        if (detectedBySystemActif != null || detectedBySystemPassif != null || detectedBySystemVisuel != null)
        {
            MiseEnOeuvre();
        }

        if(lastDetectedBySystemActif != null || lastDetectedBySystemPassif !=null || lastDetectedBySystemVisuel != null)
        {
            DetectionClean();
        }
       
        SauvegardeOfLastDetection();
    }

    void Detection()
    {
        detectedColliderTab = Physics.OverlapSphere(myShip.transform.position, porteDeDetectionMax, layerMask);
        foreach (Collider collider in detectedColliderTab)
        {
            detectedCollidersList.Add(collider);
        }
    }

    void Analyse()
    {
        detectedBySystemActif.Clear();
        detectedBySystemPassif.Clear();
        detectedBySystemVisuel.Clear();

        RaycastHit hit;
        foreach (Collider collider in detectedCollidersList)
        {

            if (Physics.Raycast(myShip.transform.position, collider.gameObject.transform.position, out hit))
            {

                Debug.DrawRay(myShip.transform.position, hit.collider.gameObject.transform.position, UnityEngine.Color.red, 0.5f);
                float distance = hit.distance;
                //Debug.Log(distance);
                if (distance <= porteDuSystemVisuel)
                {
                    Debug.Log("System Visuel");
                    if (Physics.Raycast(myShip.transform.position, collider.gameObject.transform.position, out hit, porteDeDetectionMax))
                    {
                        if (hit.collider == collider)
                        {
                            detectedBySystemVisuel.Add(collider);

                        }
                    }
                }
                else if (distance <= porteDuSystemPassif && collider.gameObject.GetComponent<DetectionSystem>() != null && collider.gameObject.GetComponent<DetectionSystem>().systemActif == true)
                {
                    Debug.Log("System Passif");
                    detectedBySystemPassif.Add(collider);

                }
                else if (systemActif == true)
                {
                    Debug.Log("System Actif");
                    detectedBySystemActif.Add(collider);

                }
            }
        }

        detectedCollidersList.Clear();

    }

    void MiseEnOeuvre()
    {
        foreach (Collider collider in detectedBySystemVisuel)
        {
            collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        foreach (Collider collider in detectedBySystemPassif)
        {
            collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        foreach (Collider collider in detectedBySystemActif)
        {
            collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

    }

    void DetectionClean()
    {
        foreach (Collider collider in lastDetectedBySystemVisuel)
        {
            bool isPresent = detectedBySystemVisuel.Contains(collider);
            Debug.Log(isPresent);
            if (!isPresent)
            {
                collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        lastDetectedBySystemVisuel.Clear();
        
        foreach (Collider collider in lastDetectedBySystemPassif)
        {
            bool isPresent = detectedBySystemPassif.Contains(collider);

            if (!isPresent)
            {
                collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        lastDetectedBySystemPassif.Clear();

        foreach (Collider collider in lastDetectedBySystemActif)
        {
            bool isPresent = detectedBySystemActif.Contains(collider);

            if (!isPresent)
            {
                collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

        }
        lastDetectedBySystemActif.Clear();
    }

    void SauvegardeOfLastDetection()
    {
       foreach(Collider collider in detectedBySystemVisuel)
        {
            lastDetectedBySystemVisuel.Add(collider);
        }

        foreach (Collider collider in detectedBySystemActif)
        {
            lastDetectedBySystemActif.Add(collider);
        }

        foreach(Collider collider in detectedBySystemPassif)
        {
            lastDetectedBySystemPassif.Add(collider);
        }
        
    }

   

   

    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawWireSphere(myShip.transform.position, porteDeDetectionMax);
    }
}
