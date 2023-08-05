using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelTO : MonoBehaviour
{
    public Transform travelPoint;
    public LightTriiger trigerCheck;
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        trigerCheck = GetComponent<LightTriiger>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void wasTriger()
    {
        Vector3 position = Vector3.MoveTowards(rb.position, travelPoint.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(position);
        //transform.position = Vector2.MoveTowards(transform.localPosition, travelPoint.transform.localPosition, 0.5f);
        Debug.Log("WasTrigger");
    }

    public void stopTrigger()
    {
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, travelPoint.transform.localPosition, 0f);
        Debug.Log("OffTrigger");
    }

   
}
