using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    private Camera _Cam;
    private void Awake()
    {
        _Cam = Camera.main;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position+ _Cam.transform.rotation*-Vector3.forward,_Cam.transform.rotation*Vector3.up);
    }
    public void UpdateHealthBar(float maxHealth,float currentHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    public void activeHealthBar(float Active)
    {
        transform.DOScale(Active, 1f);
    }
}
