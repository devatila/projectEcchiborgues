using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SimpleEnemyPursuit : MonoBehaviour
{
    [SerializeField] private float enemySpeed;
    [SerializeField] private float enemyRange;

    private NavMeshAgent agent;
    private Transform playerPosition;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        playerPosition = FindObjectOfType<PlayerInventory>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerPosition.position);
    }
}
