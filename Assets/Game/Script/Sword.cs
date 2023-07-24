using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] public RandomMovement enemyMovement;
    public float damageSlash;
    public bool isPlayer = false;
    int id;
    // Start is called before the first frame update
    private void Awake()
    {
        getDamageSword();
    }

    public void getDamageSword()
    {

        if (gameObject.tag == "bladePlayer")
        {
            damageSlash = GameEvents.Instance.playerMovement.damagePerSlash;
            isPlayer = true;
        }
        if (gameObject.tag == "bladeEnemy")
        {
            damageSlash = GameEvents.Instance.listEnemy[enemyMovement.id].damageSlash;
            isPlayer = false;
            id = enemyMovement.id;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.gameObject.tag == "Enemy")
            {
                RandomMovement randomMovement = other.gameObject.GetComponent<RandomMovement>();
                if (GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Attack)
                {

                    GameEvents.Instance.listEnemy[randomMovement.id].takeDamage(damageSlash);

                }
            }
        }
        else
        {
            if (other.gameObject.tag == "Player")
            {
                if (enemyMovement.enemyState == RandomMovement.EnemyState.Attack)
                {

                    GameEvents.Instance.playerMovement.takeDamage(damageSlash);

                }
            }
            if (other.gameObject.tag == "Enemy")
            {
                RandomMovement randomMovement = other.gameObject.GetComponent<RandomMovement>();
                if (enemyMovement.enemyState == RandomMovement.EnemyState.Attack)
                {

                    if (randomMovement.id != id)
                    {
                        GameEvents.Instance.listEnemy[randomMovement.id].takeDamage(damageSlash);
                    }

                }
            }
        }
    }
}
