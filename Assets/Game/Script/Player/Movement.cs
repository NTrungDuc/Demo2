using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Button btnRemake;
    //cooldown dash
    [SerializeField] private Image cooldownImage;
    float cooldownDash = 2;
    [SerializeField] bool isCooldown = false;
    //
    private Vector3 oldPos;
    public float dashForce = 5f;
    public float speedDash = 1.1f;
    public float attackTime = 0.5f;
    private bool isAttacking = false;
    private bool isDashing = false;
    public bool isBouching = false;

    private void Start()
    {
        currentHealth = maxHealth;
        oldPos = transform.position;
        cooldownImage.fillAmount = 0;
        btnRemake.onClick.AddListener(() =>
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Idle;
            StartCoroutine(ragdollController.DeathSequence(1.5f, false,0.001f));
            GameEvents.Instance.disableLosePanel();
            currentHealth = maxHealth;
            healthBar.UpdateHealthBar(maxHealth, currentHealth);
            transform.position = oldPos;
        });
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
        if (h > 0 || v > 0 || h < 0 || v < 0)
        {
            GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Move;
            animator.SetBool("run", true);
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
        rb.velocity = new Vector3(h, 0, v) * speed;
        if (rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v));
        }
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
        //transform.Translate(Vector3.forward * dashForce);
        speed *= speedDash;
        yield return new WaitForSeconds(dashForce);
        speed /= speedDash;
        isDashing = false;
    }
    public IEnumerator bouchingPlayer()
    {
        float bouching = 0.3f;
        transform.Translate(Vector3.back * bouching);
        yield return new WaitForSeconds(0.1f);
        isBouching = false;
    }
    void Die()
    {
        GameEvents.Instance.playerManager.playerState = PlayerManager.PlayerState.Die;
        GameEvents.Instance.showLosePanel();
        StartCoroutine(ragdollController.DeathSequence(1.5f, true,0f));
    }
    public void takeDamage(float damageAmout)
    {
        currentHealth -= damageAmout;
        isBouching = true;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void activeEffect()
    {
        if (GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Attack
            || GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Move)
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

}
