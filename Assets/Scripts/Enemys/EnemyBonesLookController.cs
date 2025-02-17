using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBonesLookController : MonoBehaviour
{
    [Header("Bones Reference")]
    public Transform spineBone;
    public Transform armBone;
    public Transform headBone;

    [Space()]

    [Header("Control Values")]
    public float minRotation;
    public float maxRotation;

    public GameObject player;
    private EnemyType1 enemyType1;


    void Start()
    {
        player = FindObjectOfType<PlayerInventory>().gameObject;
        enemyType1 = transform.parent.parent.gameObject.GetComponent<EnemyType1>();


    }


    void Update()
    {
        RotationController();
    }

    void RotationController()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float clampedAngle = Mathf.Clamp(angle, minRotation, maxRotation);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
