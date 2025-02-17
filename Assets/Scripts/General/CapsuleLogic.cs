using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CapsuleLogic : MonoBehaviour
{
    public Rigidbody2D rb;

    public float footPos;
    public Pool_Projectiles pool_Projectiles;
    public float lifetime = 2f;

    public GameObject player;
    private GameObject gobj;
    // Start is called before the first frame update
    void Start()
    {
        
        gobj = this.gameObject;
        
        pool_Projectiles = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<Pool_Projectiles>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void ToEnable(float position)
    {
        footPos = position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player = FindObjectOfType<Player_Movement>().gameObject;

        float multiplicador = (mousePosition.x < player.transform.position.x) ? -1 : 1;
        float randomico = Random.Range(-30, -60);
        rb.AddForce(new Vector3(randomico * multiplicador, 100, 0));
        rb.simulated = true;
        StartCoroutine(CapsuleTimer(gameObject));
    }

    void Deactivate()
    {
        if (gobj != null)
            pool_Projectiles.ReturnObject(gobj);
    }

    IEnumerator CapsuleTimer(GameObject capsule)
    {
        while (capsule.activeSelf)
        {
            Vector3 capsulePos = capsule.transform.position;

            if (capsulePos.y < footPos)
            {
                capsule.transform.position = new Vector3(capsulePos.x, footPos, capsulePos.z);
                rb.simulated = false;
                yield return new WaitForSeconds(lifetime);
                capsule.SetActive(false);
                Deactivate();
            }
            yield return null;
        }
    }

}
