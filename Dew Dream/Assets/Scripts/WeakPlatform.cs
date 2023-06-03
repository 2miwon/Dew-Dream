using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{
    //public float ReactionTime;
    public float RespwanTime;
    int respawnDelay;
    public float BreakTime; // 몇 초 후에 깨지는 지
    int breakLatency;
    public float MaxLoad;
    bool exist;
    void Awake(){
        exist = true;
        respawnDelay = (int) (RespwanTime * 60.0f);
        breakLatency = -1;
    }
    void Start(){

    }
    void Update(){
        if(Delay(ref respawnDelay) && !exist)Respawn();
    }
    void FixedUpdate(){
        
    }
    bool Delay(ref int val){return val-- <= 0;}
    bool Latency(ref int val, float threshold){
        while(val++ > (int)(threshold*60.0f))
        val = -1;
        return true;
    }
    void OnTriggerEnter(Collider collision){
        if(collision.GetComponent<Collider>().GetComponent<GameObject>().GetComponent<Dew>().mod >= 1.2){
            Break();
        }
    }
    void Break(){
        if(Latency(ref breakLatency ,BreakTime)){
            gameObject.SetActive(false);
            exist = false;
        }
    }
    void Respawn(){
        gameObject.SetActive(true);
        exist = true;
    }
}