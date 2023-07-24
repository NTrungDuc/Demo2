using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] rigidBodies;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private HealthBar healthBar;

    private void Awake()
    {
        ActivateRagdoll(false);
    }
 
    public IEnumerator DeathSequence(float time,bool isActive,float scaleHP)
    {
        healthBar.activeHealthBar(scaleHP);
        rb.isKinematic = isActive;
        yield return new WaitForSeconds(time);
        ActivateRagdoll(isActive);
    }
    public void ActivateRagdoll(bool isActive)
    {
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = !isActive;
        }
        foreach (Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
        animator.enabled = !isActive;
    }
    public void activeCollider(bool isActive)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
        rb.isKinematic=isActive;
    }
}
