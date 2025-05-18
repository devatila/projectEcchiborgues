using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Atributos")]
    public float ACTUAL_SPEED; //Velocidade atual do player
    [SerializeField] private float WALK_SPEED; // Velociade inicial e padrão do player ao andar
    [SerializeField] private float RUN_SPEED; // Velocidade de corrida do player
    [HideInInspector] public Vector2 MOVE_DIRECTION;
    public GameObject playerObject;
    private Rigidbody2D rb;

    private bool FACING_RIGHT = true;
    public bool facingRight { get { return FACING_RIGHT; } set { FACING_RIGHT = value; } }
    private bool canMove = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ACTUAL_SPEED = WALK_SPEED;
        TypewriterEffectTMP.stopAll += StopMovement;
        TypewriterEffectTMP.ContinueAll += ContinueMove;
    }


    void Update()
    {
        if (canMove)
        {
            GetMove();
            GetFlip(playerObject);
        }
        
    }

    private void GetMove()
    {
        MOVE_DIRECTION = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ACTUAL_SPEED = RUN_SPEED;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ACTUAL_SPEED = WALK_SPEED;
        }
    }
    void GetFlip(GameObject target)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x && FACING_RIGHT)
            Flip(target);
        else if (mousePos.x > transform.position.x && !FACING_RIGHT)
            Flip(target);
    }
    void Flip(GameObject obj)
    {
        FACING_RIGHT = !FACING_RIGHT;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * -1, obj.transform.localScale.y, obj.transform.localScale.z);
        //obj.transform.Rotate(0, 180, 0);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + MOVE_DIRECTION * ACTUAL_SPEED * Time.fixedDeltaTime);
    }

    public void StopMovement()
    {
        canMove = false;
        ACTUAL_SPEED = 0f;
    }

    public void ContinueMove()
    {
        canMove = true;
        ACTUAL_SPEED = WALK_SPEED;
    }

    public void SwitcherPlayerMovement()
    {
        canMove = !canMove;
        if (canMove)
        {
            canMove = true;
            ACTUAL_SPEED = WALK_SPEED;
        }
        else
        {
            canMove = false;
            ACTUAL_SPEED = 0f;
        }
    }

    public void StopPlayerMovement()
    {
        canMove = false;
        ACTUAL_SPEED = 0f;
    }
    public void ContinuePlayerMovement()
    {
        canMove = true;
        ACTUAL_SPEED = WALK_SPEED;
    }

    public void SetMultiplierMovement(float effector)
    {
        ACTUAL_SPEED = WALK_SPEED * effector;
    }
    
}
