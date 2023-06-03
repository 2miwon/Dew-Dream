using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{
    //public float ReactionTime;
    public float RespwanTime;
    int realTime;
    public float MaxLoad;
    bool exist;
    void Awake(){
        exist = true;
        realTime = RespwanTime * 60;
    }
    void Start(){

    }
    void Update(){
        if(Delay(ref realTime) && !exist)Respawn();
    }
    void FixedUpdate(){
        
    }
    bool Delay(ref int val){return val-- <= 0;}
    void OnTriggerEnter(Collider collision){
        if(collision.GetComponent<Collider>().GameObject.GetComponent<Dew>().mod >= 1.2){
            Break();
        }
    }
    void Break(){

    }
    void Respawn(){
        
        exist = true;
    }
}