using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) gameObject.SetActive(false);
    }
    
}
