using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{
    Renderer rend;
    public float RespwanTime;
    //int respawnDelay;
    public float BreakTime; // 몇 초 후에 깨지는 지
    //int breakLatency;
    public float MaxLoad;
    bool exist;
    private Material originalMaterial;
    public Material transparentMaterial;
    public AudioSource audioSource;
    public ParticleSystem effect;

    void Awake(){
        exist = true;
        //respawnDelay = (int) (RespwanTime * 60.0f);
        rend = transform.parent.GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        effect = transform.parent.GetComponent<ParticleSystem>();
        //breakLatency = -1;
    }
    void Start(){
        originalMaterial = transform.parent.GetComponent<Renderer>().material;
        //transparentMaterial = new Material(originalMaterial); // 원래의 머티리얼을 복사하여 새로운 투명한 머티리얼 생성
        //transparentMaterial.color = new Color(1f, 1f, 1f, 0.5f); // 투명도 조절 (4번째)
    }
    void Update(){
        
    }
    void FixedUpdate(){
    
    }
    bool Delay(ref int val){return val-- <= 0;}
    /*
    bool Latency(ref int val, float threshold){
        
        while(val++ > (int)(threshold*60.0f))
        val = -1;
        return true;
    }*/
    void OnTriggerEnter(Collider collision){
        try{
            if(collision.GetComponent<Collider>().GetComponent<Dew>().mod >= MaxLoad){
                //Debug.Log("check");
                Invoke("Break", BreakTime);
            }
        }catch{

        }
        
    }
    void Break(){
        gameObject.GetComponent<BoxCollider>().enabled = false;
        transform.parent.GetComponent<BoxCollider>().enabled = false;
        SetTransparent();
        audioSource.Play();
        //StartCoroutine("BreakAnimation");
        effect.Play();
        exist = false;
        Invoke("Respawn", RespwanTime);
    }
    void Respawn(){
        rend.material = originalMaterial;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        transform.parent.GetComponent<BoxCollider>().enabled = true;
        exist = true;
    }
    void SetTransparent(){
        rend.material = transparentMaterial;
    }
    /*
    public IEnumerator BreakAnimation(){

    }*/
}