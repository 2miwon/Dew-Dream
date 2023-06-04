using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public int SavePointNum;
    public GameObject Player;
    public bool doubleJumpFlag;
    void Awake(){
        //activate = false;
    }
    void Start(){

    }
    void Update(){

    }
    void FixedUpdate(){

    }
    void OnTriggerEnter(Collider collision){
        if(collision.GetComponent<Collider>().CompareTag("Player")){
            if(Player.GetComponent<Dew>().savePointNum < SavePointNum){
                Debug.Log("Save! :" + SavePointNum);
                //if(doubleJumpFlag)
              
                //Player.GetComponent<Dew>().DoubleJump = true;
                Player.GetComponent<Dew>().savePointNum = SavePointNum;
            }
        }
    }
}