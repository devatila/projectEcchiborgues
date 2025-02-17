using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayerPositionTracker : MonoBehaviour
{
    public bool isEnable = true;
    public Transform targetPosition, centerPosition;
    public float distance = 2f;

    public GameObject player;
    private Transform playerPosition;
    [SerializeField] private Transform defaultPosition;
    void Start()
    {
        if(centerPosition == null)
        {
            centerPosition = transform;
        }
        player = FindObjectOfType<PlayerInventory>().gameObject;
        SetParent();
    }

    private void LookTracker()
    {
        //Depois ajustar para que so receba se o IsVisible for true
        playerPosition = player.transform;

        Vector3 direction = playerPosition.position - centerPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        targetPosition.position = centerPosition.position + Quaternion.Euler(0, 0, angle) * new Vector3(distance, 0, 0);
    }
    public void SetParent()
    {
        targetPosition.parent = player.transform;
        targetPosition.localPosition = Vector3.zero;
        
    }
    public void SetDefaultPosition()
    {
        targetPosition.parent = defaultPosition;
        targetPosition.localPosition = Vector3.zero;
    }

}
