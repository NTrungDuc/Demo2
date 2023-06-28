using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private Vector3 offset;
    void Update()
    {
        updatePos();
    }
    void updatePos()
    {
        transform.position = Player.position + offset;
    }
}
