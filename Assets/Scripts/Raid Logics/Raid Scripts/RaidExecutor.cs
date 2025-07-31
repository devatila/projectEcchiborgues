using UnityEngine;

public class RaidExecutor : MonoBehaviour
{
    [SerializeField] private RaidPresetsSO RaidPreset;
    [SerializeField] private bool hasExecuted = false;
    [SerializeField] private PolygonCollider2D cameraColliderArea; // Area de camera da Raid
    [SerializeField] private Transform[] spawnPositions;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExecuted || !collision.CompareTag("Player")) return; // Previne de executar a mesma raid uma vez que ja foi executada

        //GameController.SetPolyCollider(polygonCollider)
        RaidManager.instance.StartRaid(RaidPreset, spawnPositions, cameraColliderArea); // eu atila admito que gosto de lolis vsfdr
        hasExecuted = true;
    }
}
 