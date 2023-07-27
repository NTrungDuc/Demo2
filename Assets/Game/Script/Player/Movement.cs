using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Movement : MonoBehaviour
{
    //infor player
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public float damagePerSlash;
    //
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private UltimateJoystick joystick;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource sword;
    [SerializeField] private ParticleSystem attackslash;
    [SerializeField] private RagdollController ragdollController;
    //UI
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Button btnAttack;
    [SerializeField] private Button btnDash;
    [SerializeField] private Button[] btnRemake;
    //cooldown dash
    [SerializeField] private Image cooldownImage;
    float cooldownDash = 2;
    [SerializeField] bool isCooldown = false;
    //
    private Vector3 oldPos;
    public float timeDash;
    public float speedDash = 1.1f;
    public float attackTime = 0.5f;
    private bool isAttacking = false;
    private bool isDashing = false;
    public bool isBouching = false;
    public float knockbackForce = 10f;

    private Vector3 knockbackDirection;

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1.0f;
    public float fallGravity = 2f;
    private void Start()
    {
        currentHealth = maxHealth;
        oldPos = transform.position;
        cooldownImage.fillAmount = 0;
        foreach (Button btn in btnRemake)
        {
            btn.onClick.AddListener(() =>
            {
                GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Idle;
                StartCoroutine(ragdollController.DeathSequence(0f, false, 0.001f));
                GameEvents.Instance.disableLosePanel();
                currentHealth = maxHealth;
                healthBar.UpdateHealthBar(maxHealth, currentHealth);
                transform.position = oldPos;
            });
        }
        btnAttack.onClick.AddListener(() =>
        {
            StartCoroutine(Attack());
        });
        btnDash.onClick.AddListener(() =>
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Dash;
            if (!isCooldown)
            {
                isDashing = true;
            }
        });
    }
    void Update()
    {
        if (GameEvents.Instance.playerManager.playerState != PlayerManager.PlayerState.Die)
        {
            playerMovement();
        }
        Ability();
    }
    public void playerMovement()
    {
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        float h = joystick.GetHorizontalAxis();
        float v = joystick.GetVerticalAxis();
        if (h != 0 || v != 0)
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Move;
            animator.SetBool("run", true);
            transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v));
        }
        if (h == 0 && v == 0)
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Idle;
            animator.SetBool("run", false);
        }
        if (isAttacking)
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Attack;
            rb.isKinematic = true;
        }
        activeEffect();
        if (isDashing)
        {
            StartCoroutine(Dash());
            isCooldown = true;
            cooldownImage.fillAmount = 1;
        }
        if (isBouching)
        {
            StartCoroutine(bouchingPlayer());
        }

        Vector3 movement = new Vector3(h, 0, v) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }
    void Ability()
    {
        if (isCooldown)
        {
            cooldownImage.fillAmount -= 1 / cooldownDash * Time.deltaTime;
            if (cooldownImage.fillAmount <= 0)
            {
                cooldownImage.fillAmount = 0;
                isCooldown = false;
            }
        }
    }
    private IEnumerator Attack()
    {
        //rb.isKinematic = true;
        animator.SetBool("attack", true);
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        rb.isKinematic = false;
        animator.SetBool("attack", false);
        isAttacking = false;
    }
    private IEnumerator Dash()
    {
        speed *= speedDash;
        yield return new WaitForSeconds(timeDash);
        speed /= speedDash;
        isDashing = false;
    }
    public IEnumerator bouchingPlayer()
    {
        //rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        knockbackDirection.Normalize();
        Vector3 movement = knockbackDirection;
        movement.y = 0;
        transform.Translate(movement * knockbackForce, Space.World);
        yield return new WaitForSeconds(0.1f);
        isBouching = false;
    }
    void Die(float time)
    {
        GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Die;
        GameEvents.Instance.showLosePanel();
        StartCoroutine(ragdollController.DeathSequence(time, true,0f));
    }

    public void takeDamage(float damageAmount)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damageAmount;
            isBouching = true;
            healthBar.UpdateHealthBar(maxHealth, currentHealth);
            if (currentHealth <= 0)
            {
                Die(1.5f);
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
    void activeEffect()
    {
        if (GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Attack)
        {
            attackslash.Play();
        }
        else
        {
            attackslash.Stop();
        }
        if (animator.GetBool("attack") && sword.enabled)
        {
            sword.Play();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fall"))
        {
            Die(0f);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("bladeEnemy"))
        {
            knockbackDirection = (transform.position - collision.gameObject.transform.position).normalized;
        }
    }
}
