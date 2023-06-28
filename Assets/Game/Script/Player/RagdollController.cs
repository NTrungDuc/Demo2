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
    private void Update()
    {
       StartCoroutine(DeathSequence(1.5f));
    }
    public IEnumerator DeathSequence(float time)
    {
        if (GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Die)
        {
            healthBar.activeHealthBar(0f);
            rb.isKinematic = true;                       
            yield return new WaitForSeconds(time);
            ActivateRagdoll(true);            
        }
        else
        {
            ActivateRagdoll(false);
            yield return new WaitForSeconds(time);
            rb.isKinematic = false;
            healthBar.activeHealthBar(0.001f);        
        }
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
}
