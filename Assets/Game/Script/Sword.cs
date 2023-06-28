using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] public RandomMovement enemyMovement;
    public float damageSlash;
    public bool isPlayer=false;
    float time = 0;
    float timeAttack = 1;
    // Start is called before the first frame update
    private void Awake()
    {
        getDamageSword();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }
    public void getDamageSword()
    {
      
        if (gameObject.tag == "bladePlayer")
        {
            damageSlash = GameEvents.Instance.playerMovement.damagePerSlash;
            isPlayer=true;
        }
        if (gameObject.tag == "bladeEnemy")
        {
            damageSlash = GameEvents.Instance.listEnemy[enemyMovement.id].damageSlash;
            isPlayer = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.gameObject.tag == "Enemy") {
                RandomMovement randomMovement=other.gameObject.GetComponent<RandomMovement>();
                if (GameEvents.Instance.playerManager.playerState == PlayerManager.PlayerState.Attack)
                {
                    //Debug.Log(randomMovement.id);
                    if (time < timeAttack)
                    {
                        GameEvents.Instance.listEnemy[randomMovement.id].takeDamage(damageSlash);
                    }else
                    {
                        time = 0;
                    }
                }
            }
        }
        else
        {
            if(other.gameObject.tag == "Player")
            {
                
                if (time < timeAttack)
                {
                    GameEvents.Instance.playerMovement.takeDamage(damageSlash);
                }
                else
                {
                    time = 0;
                }
            }
            if (other.gameObject.tag == "Enemy") {
                RandomMovement randomMovement=other.gameObject.GetComponent<RandomMovement>();
                if (time < timeAttack)
                {
                    GameEvents.Instance.listEnemy[randomMovement.id].takeDamage(damageSlash);
                }
                else
                {
                    time = 0;
                }
            }
        }
    }
}
