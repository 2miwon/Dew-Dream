using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{
    //public float ReactionTime;
    public float RespwanTime;
    int respawnDelay;
    public float BreakTime; // 몇 초 후에 깨지는 지
    //int breakLatency;
    public float MaxLoad;
    public bool exist;
    private Material originalMaterial;
    private Material transparentMaterial;
    void Awake(){
        exist = true;
        respawnDelay = (int) (RespwanTime * 60.0f);
        //breakLatency = -1;
    }
    void Start(){
        originalMaterial = GetComponent<Renderer>().material;
        transparent = new Material(originalMaterial); // 원래의 머티리얼을 복사하여 새로운 투명한 머티리얼 생성
        transparent.color = new Color(1f, 1f, 1f, 0.0f); // 투명도 조절 (4번째)
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
        if(collision.GetComponent<Collider>().GetComponent<Dew>().mod >= MaxLoad){
            Debug.Log("check");
            Invoke("Break", BreakTime);
        }
    }
    void Break(){
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        SetTransparent();
        exist = false;
        Invoke("Respawn", RespawnTime);
    }
    void Respawn(){
        originalMaterial = GetComponent<Renderer>().material;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        exist = true;
    }
    void SetTransparent(){
        GetComponent<Renderer>().material = transparentMaterial;
    }
}