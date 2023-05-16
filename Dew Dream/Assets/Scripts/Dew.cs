using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dew : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower; 
    public float fullLife;
    public GameObject obj;
    public Renderer rend;
    public Color color;
    public GameObject metrial;

    Color colorStart = Color.blue;
    Color colorEnd = Color.gray;
    Rigidbody rigid;
    float smoothness = 0.02f;
    SphereCollider collider;
    float lifetime;
    //GameManager gameManager;  // for stages
    //public AudioClip audioFinish, audioJump;
    //AudioSource audioSource;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //color = obj.GetComponent<Renderer>().material.color;
        rend = obj.GetComponent<Renderer>();
        lifetime = fullLife;
        StartCoroutine("LifeDecrease");
        //StartCoroutine("ColorChange");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(lifetime);
        Jump();
        Move();
    }
    void FixedUpdate(){
        
    }

    void Jump(){
        if (Input.GetButtonDown("Jump")) //&& !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            //anim.SetBool("isJumping", true);

            //audioSource.clip = audioJump;
            //audioSource.Play();
        }
    }
    void Move(){
        if(Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector3(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
    }
    
    IEnumerator LifeDecrease(){
        while(lifetime > 0){
            lifetime -= smoothness;
            ColorChange();
            yield return new WaitForSeconds(smoothness);
        }
    }

    void ColorChange(){
        rend.material.color = Color.Lerp(colorEnd, colorStart, lifetime/10);
    }

}
