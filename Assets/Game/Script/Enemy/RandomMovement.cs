using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;


public class RandomMovement : MonoBehaviour
{
    //Infor Bot
    public int id;
    public float enemyMaxHealth = 100;
    public float currentHealth;
    public float damageSlash = 2;
    private float attackRadius = 3;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private float patrolRadius = 30;
    public Vector3 centrePoint;
    public EnemyState enemyState;

    [SerializeField] private HealthBar healthBar;
    //Target enemy
    Transform nearestTarget;
    private float chaseRadius = 12;
    private float cooldownChangeDestiantion = 0.75f;
    private float timeChangeDes = 0;

    [SerializeField] bool shouldRun = false;
    [SerializeField] RagdollController ragdoll;
    [SerializeField] private List<GameObject> listTargetEnemy;
    public bool isBouching = false;
    [SerializeField] private Collider cd;

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1.0f;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Escape,
        Die
    }
    private void Awake()
    {
        centrePoint = transform.position;       
    }


    void Update()
    {
        if (enemyState != EnemyState.Die)
        {
            if (enemyState == EnemyState.Patrol)
            {
                randomMove();
            }

            timeChangeDes += Time.deltaTime;

            if (timeChangeDes >= cooldownChangeDestiantion)
            {
                nearestTarget = GetNearestTarget();
                timeChangeDes = 0;
            }
            if (shouldRun)
            {
                isAttack(false, EnemyState.Escape);
                Vector3 currentPosition = transform.position;
                if (nearestTarget != null)
                {
                    Vector3 reversedDirection = (currentPosition - nearestTarget.position).normalized;

                    NavMeshHit navMeshHit;
                    if (enemyState == EnemyState.Escape)
                    {
                        if (NavMesh.SamplePosition(currentPosition + reversedDirection * patrolRadius, out navMeshHit, 1f, NavMesh.AllAreas))
                        {
                            Vector3 newPosition = navMeshHit.position;

                            agent.SetDestination(newPosition);
                            StartCoroutine(changeStateRun(EnemyState.Patrol));
                        }
                        
                    }
                }
                else
                {
                    if (enemyState == EnemyState.Escape)
                    {
                        isAttack(false, EnemyState.Patrol);
                    }
                }
            }
            else if (nearestTarget != null)
            {
                agent.SetDestination(nearestTarget.position);
            }
            if (isBouching)
            {
                StartCoroutine(bouchingPlayer());
            }
        }
    }
    public void randomMove()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint, patrolRadius, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }

    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private Transform GetNearestTarget()
    {

        Transform nearestTarget = null;
        Vector3 currentPosition = transform.position;

        foreach (GameObject target in listTargetEnemy.ToArray())
        {
            float distance = Vector3.Distance(target.transform.position, currentPosition);

            if (distance < chaseRadius)
            {
                nearestTarget = target.transform;
                
                if (distance <= attackRadius)
                {
                    transform.DOLookAt(nearestTarget.transform.position, 1f);
                    isAttack(true, EnemyState.Attack);
                    EscapeAndcheckDead(nearestTarget);
                }
                else
                {
                    isAttack(false, EnemyState.Patrol);
                }

            }
            if (!target.active)
            {
                listTargetEnemy.Remove(target);
            }
        }


        return nearestTarget;
    }

    void EscapeAndcheckDead(Transform nearestTarget)
    {
        if (nearestTarget.gameObject.tag == "Enemy")
        {
            RandomMovement Target = nearestTarget.GetComponent<RandomMovement>();
            if (Target.currentHealth <= 0)
            {
                //listTargetEnemy.Remove(nearestTarget.gameObject);
                isAttack(false, EnemyState.Patrol);
            }
            if (currentHealth <= 0.3f * enemyMaxHealth)
            {
                shouldRun = true;
            }
        }
    }
    public void isAttack(bool isAttack, EnemyState state)
    {
        agent.isStopped = isAttack;
        animator.SetBool("enemAttack", isAttack);
        rb.isKinematic = isAttack;
        enemyState = state;
    }
    IEnumerator changeStateRun(EnemyState state)
    {
        agent.speed = 8;
        yield return new WaitForSeconds(5f);
        enemyState = state;
        shouldRun = false;
        agent.speed = 3.5f;
    }
    IEnumerator Die()
    {
        enemyState = EnemyState.Die;
        StartCoroutine(ragdoll.DeathSequence(0f, true, 0f));
        agent.enabled = false;
        yield return new WaitForSeconds(2f);
        cd.isTrigger = true;
        ragdoll.activeCollider(false);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    public IEnumerator bouchingPlayer()
    {
        float bouching = 0.5f;
        transform.Translate(Vector3.back * bouching);
        yield return new WaitForSeconds(0.1f);
        isBouching = false;
    }

    public void takeDamage(float damageAmount)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damageAmount;
            isBouching = true;
            healthBar.UpdateHealthBar(enemyMaxHealth, currentHealth);
            if (currentHealth <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                StartCoroutine(InvulnerabilityCooldown());
            }
        }
    }


    private IEnumerator InvulnerabilityCooldown()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
    public void setCurrentHealth()
    {
        currentHealth = enemyMaxHealth;
        enemyState = EnemyState.Patrol;
        listTargetEnemy = new List<GameObject>(GameEvents.Instance.listTarget);
        listTargetEnemy.Remove(gameObject);
        agent.enabled = true;
        cd.isTrigger = false;
        healthBar.UpdateHealthBar(enemyMaxHealth, currentHealth);
        StartCoroutine(ragdoll.DeathSequence(0f, false, 0.001f));
    }
    public void resetAll()
    {
        transform.position = centrePoint;
    }
    private void OnEnable()
    {
        setCurrentHealth();
        GameEvents.Instance.listAlive.Add(this);
    }
    private void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;
        GameEvents.Instance.listAlive.Remove(this);
    }
}
