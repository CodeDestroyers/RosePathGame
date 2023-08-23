using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class PlayerAttackScript : MonoBehaviour
{
    //How much damage the melee attack does
    //Reference to Character script which contains the value if the player is facing left or right; if you don't have this or it's named something different, either omit it or change the class name to what your Character script is called
    private PlayerGodController character;

    [SerializeField] Transform playerTransform;
    //Reference to the Rigidbody2D on the player

    private int damageAmount = 25;

    //Reference to the direction the player needs to go in after melee weapon contacts something
    public Vector2 direction;
    //Bool that manages if the player should move after melee weapon colides
    public bool collided;
    //Determines if the melee strike is downwards to perform extra force to fight against gravity
    
    public bool downwardStrike;

    public Vector2 moveVal;

    private PlayerControls playerControls;

    private void Start()
    {
        //Reference to the Character script on the player; if you don't have this or it's named something different, either omit it or change the class name to what your Character script is called
        character = GetComponentInParent<PlayerGodController>();
        //Reference to the Rigidbody2D on the player
        //Reference to the MeleeAttackManager script on the player
        OnEnable();
        playerTransform = GetComponentInParent<GameObject>().transform;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.PlayerActions.ChooseVerticalDerection.performed += ctx => moveVal = ctx.ReadValue<Vector2>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void FixedUpdate()
    {
        //Uses the Rigidbody2D AddForce method to move the player in the correct direction
        character.HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks to see if the GameObject the MeleeWeapon is colliding with has an EnemyHealth script
        if (collision.GetComponent<EnemyHealth>())
        {
            //Method that checks to see what force can be applied to the player when melee attacking
            HandleCollision(collision.GetComponent<EnemyHealth>());
            Debug.Log("Enemy was hit!");
        }

        if (collision.tag.Equals("Enemy"))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHitEnemy, this.transform.position);
        }

        if (collision.tag.Equals("Wall"))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHitWall, this.transform.position);
        }

        if (collision.tag.Equals("Enviroment"))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHitEnviroment, this.transform.position);
        }
    }

    private void HandleCollision(EnemyHealth objHealth)
    {
        //Checks to see if the GameObject allows for upward force and if the strike is downward as well as grounded
        if (objHealth.giveUpwardForce && moveVal.y < 0 && !character.IsGround())
        {
            //Sets the direction variable to up
            direction = Vector2.up;
            //Sets downwardStrike to true
            downwardStrike = true;
            //Sets collided to true
            collided = true;
        }
        if (moveVal.y > 0 && !character.IsGround())
        {
            //Sets the direction variable to up
            direction = Vector2.down;
            //Sets collided to true
            collided = true;
        }
        //Checks to see if the melee attack is a standard melee attack
        if ((moveVal.y <= 0 && character.IsGround()) || moveVal.y == 0)
        {
            //Checks to see if the player is facing left; if you don't have a character script, the commented out line of code can also check for that
            if (playerTransform.transform.localScale.x == -1) //(transform.parent.localScale.x < 0)
            {
                //Sets the direction variable to right
                direction = Vector2.right;
            }
            else if (playerTransform.transform.localScale.x == 1)
            {
                //Sets the direction variable to right left
                direction = Vector2.left;
            }
            //Sets collided to true
            collided = true;
        }
        //Deals damage in the amount of damageAmount
        objHealth.Damage(damageAmount);
        //Coroutine that turns off all the bools related to melee attack collision and direction
        StartCoroutine(NoLongerColliding());
    }

    //Method that makes sure there should be movement from a melee attack and applies force in the appropriate direction based on the amount of force from the melee attack manager scrip

    //Coroutine that turns off all the bools that allow movement from the HandleMovement method
    private IEnumerator NoLongerColliding()
    {
        //Waits in the amount of time setup by the meleeAttackManager script; this is by default .1 seconds
        yield return new WaitForSeconds(character.enumAttackTime);
        //Turns off the collided bool
        collided = false;
        //Turns off the downwardStrike bool
        downwardStrike = false;
    }
}
