using System.Collections;
using UnityEngine;

public class KraneHandleMover : MonoBehaviour
{
    private static float MAXDISTANCE = 3f;
    public float heightIncrement = 1f;
    public bool switchSide;
    public float kraneMoveSpeed = 1f;
    public float leftRightLimit = 2.3f;
    [SerializeField] private LayerMask ignoreLayer;

    private Vector3 newPos;
    private Vector3[] positions;

    private void Start()
    {
        positions = new[] {Vector3.one, Vector3.down}; // fill out
        switchSide = false;
        InvokeRepeating("MoveKrane", 1.2f, 1.2f);
    }

    private void Update()
    {
        //CheckHeight();
        SmoothKraneMove();
    }

    void MoveKrane()
    {
        if (switchSide == false)
        {
            newPos = (transform.position + Vector3.right) * kraneMoveSpeed;
            //transform.position += Vector3.right * kraneMoveSpeed;
            if (transform.position.x >= leftRightLimit)
                switchSide = true;
        }
        else
        {
            newPos = transform.position - Vector3.right * kraneMoveSpeed;
            //transform.position -= Vector3.right * kraneMoveSpeed;
            if (transform.position.x <= -leftRightLimit)
                switchSide = false;
        }
    }

    private void SmoothKraneMove()
    {
        if (newPos != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, .125f);
        }
    }

    public void CheckHeight()
    {
        /*RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down* 1.5f, Vector3.down, 10f, ignoreLayer);
        if (hit.collider != null)
        {
            float distanceDown = Vector3.Distance(transform.position, hit.collider.transform.position);
            float smoothness = 0.125f;
            
            if (distanceDown < MAXDISTANCE) // could also check if the hit target is stationary
            {

                Vector3 newCameraPos = Vector3.Lerp(Camera.main.transform.position,Camera.main.transform.position + Vector3.up * heightIncrement, smoothness);
                Camera.main.transform.position = newCameraPos;
                
                Vector3 newPos = Vector3.Lerp(transform.position,transform.position + Vector3.up * heightIncrement, smoothness);
                transform.position = newPos;
            }
            Debug.DrawLine(transform.position, hit.collider.transform.position);
        }*/
    }
}
