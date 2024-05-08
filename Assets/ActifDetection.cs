using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActifDetection : MonoBehaviour
{
    public GameObject myShip;

    public Collider[] actifDetectedColliderList;
    public Collider[] lastDetectedColliderList;
    public float porteDeDetection;

    public LayerMask layerMask;

    public bool isActif = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (isActif)
            {
                case true:
                    isActif = false;
                    break;
                case false:
                    isActif=true;
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isActif)
        {
            Detection();
            if (actifDetectedColliderList != null)
            {
                Analyse();
            }
        }
        else
        {
            if (actifDetectedColliderList != null)
            {
                
            }
        }
    }

    void Detection()
    {
        actifDetectedColliderList = Physics.OverlapSphere(myShip.transform.position, porteDeDetection, layerMask);
    }

    void Analyse()
    {

        foreach (Collider collider in actifDetectedColliderList)
        {
            if (collider.gameObject.GetComponent<MeshRenderer>().enabled == false)
            {
                collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        if (lastDetectedColliderList != null)
        {
            foreach (Collider collider in lastDetectedColliderList)
            {
                bool isStillIn = false;
                foreach (Collider collider1 in actifDetectedColliderList)
                {
                    if (collider == collider1)
                    {
                        isStillIn = true;
                    }
                }

                if (isStillIn == false)
                {
                    collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        lastDetectedColliderList = actifDetectedColliderList;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(myShip.transform.position, porteDeDetection);
    }
}
