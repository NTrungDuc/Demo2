using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameEvents : Singleton<GameEvents>
{
    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public Movement playerMovement;
    [SerializeField] public List<RandomMovement> listEnemy;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] public List<GameObject> listTarget;
    [SerializeField] public List<RandomMovement> listAlive;
    public void showWinPanel(int zoom)
    {
        winPanel.transform.DOScale(zoom, 1f);
    }
    public void showLosePanel()
    {
        losePanel.transform.DOScale(1, 1f);
    }
    public void disableLosePanel()
    {
        losePanel.transform.DOScale(0, 1f);        
        foreach(RandomMovement rM in listEnemy)
        {
            rM.isAttack(false, RandomMovement.EnemyState.Patrol);
            rM.resetPos();
            if (rM.gameObject.active)
            {
                rM.setCurrentHealth();
            }
            else
            {
                rM.gameObject.active = true;
            }
        }
    }
}
