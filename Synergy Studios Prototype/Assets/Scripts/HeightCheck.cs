using System;
using UnityEngine;

public class HeightCheck : MonoBehaviour
{
    private StackGameManager sgm;
    public AudioClip hitAudio;
    public AudioClip hitUtensilAudio;

    private void Start()
    {
        sgm = FindObjectOfType<StackGameManager>();
    }

    private void Update()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb.velocity.magnitude < .01f && rb.velocity.y > 2.8165E-08)
                gameObject.layer = 0;
        }
        
        if(transform.position.y <= -10)
            Destroy(gameObject);
    }
        

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("HeightTrigger"))
        {
            if(GetComponent<Rigidbody2D>().velocity.magnitude < .01f)
                sgm.GameWon();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Utensils"))
            GetComponent<AudioSource>().PlayOneShot(hitUtensilAudio);
        else
            GetComponent<AudioSource>().PlayOneShot(hitAudio);
    }
}
