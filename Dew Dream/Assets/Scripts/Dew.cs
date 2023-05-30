using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dew : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float jumpVeloMax;
    private int jumpChance;
    public float groundMaxDistance;
    public float fullLife;
    public GameObject obj;
    Renderer rend;
    Color color;
    Color colorStart = Color.blue;
    Color colorEnd = Color.gray;
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    float smoothness = 0.02f;
    float lifetime;
    //GameManager gameManager;  // for stages
    //public AudioClip audioFinish, audioJump;
    //AudioSource audioSource;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //color = obj.GetComponent<Renderer>().material.color;
        rend = transform.GetChild(0).GetComponent<Renderer>();
        lifetime = fullLife;
        StartCoroutine("LifeDecrease");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(lifetime);
        Jump();
        onGround();
    }
    void FixedUpdate(){
        Move();
    }

    void Jump(){
        if (Input.GetButtonDown("Jump")&&jumpChance>1) //&& !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpChance--;
            //anim.SetBool("isJumping", true);

            //audioSource.clip = audioJump;
            //audioSource.Play();
        }
    }
    void Move(){
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(new Vector3(-speed,0,0));
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(new Vector3(speed,0,0));
        }
    }
    void onGround(){
        if(Physics.BoxCast(transform.position, 
                            transform.lossyScale / 3.0f, 
                            Vector3.down, 
                            out RaycastHit hit, new Quaternion(), 
                            groundMaxDistance)){
            jumpChance = 2;
        }
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        if(Physics.BoxCast(transform.position, 
                            transform.lossyScale / 3.0f, 
                            Vector3.down, 
                            out RaycastHit hit, new Quaternion(), 
                            groundMaxDistance)){
            Gizmos.DrawRay(transform.position, Vector3.down * hit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * hit.distance, transform.lossyScale / 2.0f);
        }else{
            Gizmos.DrawRay(transform.position, Vector3.down * groundMaxDistance);
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
