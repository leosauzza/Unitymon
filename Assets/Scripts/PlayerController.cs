using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //movement behavior
    public float moveSpeed;
    private Vector2 movement;
    private Rigidbody2D rb;

    //Bullet behavior
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    //tackle behavior
    private float tackleSpeed = 20f;
    private bool isTackling = false;
    private float tackleDistance = 50f;
    private float currentDistance = 0f;
    
    Vector2 lookDirection;
    float lookAngle;
    Vector2 mouseAndPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var playerVector = new Vector2(transform.position.x,transform.position.y);
        mouseAndPlayer = lookDirection - playerVector;
        lookAngle = Mathf.Atan2(mouseAndPlayer.y, mouseAndPlayer.x) * Mathf.Rad2Deg;

        bulletSpawnPoint.rotation = Quaternion.Euler(0, 0, lookAngle);

        if(Input.GetKeyDown(KeyCode.Space)) // Shoot skill behavior
        {

            GameObject bulletClone = Instantiate(bulletPrefab);
            Physics2D.IgnoreCollision(bulletClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            bulletClone.transform.position = bulletSpawnPoint.position;
            bulletClone.transform.rotation = Quaternion.Euler(0, 0, lookAngle);
            bulletClone.transform.parent = this.transform;

            bulletClone.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.right * bulletSpeed;

        }
        else if(Input.GetKeyDown(KeyCode.LeftControl)) //Tackle behavior
        {
            Vector2 tackleDirection = new Vector2(Mathf.Cos(lookAngle * Mathf.Deg2Rad), Mathf.Sin(lookAngle * Mathf.Deg2Rad));
            Vector2 tackleVelocity = tackleDirection * tackleSpeed;

            isTackling = true;

            rb.velocity = tackleVelocity;
                        
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if(isTackling)
        {
            currentDistance += currentDistance + tackleSpeed * Time.fixedDeltaTime;
            if(currentDistance >= tackleDistance)
            {
                isTackling = false;
                currentDistance = 0;
                rb.velocity = new Vector2(0,0);
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

}