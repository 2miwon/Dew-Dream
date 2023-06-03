using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject obj;
    public GameObject target;
    bool ButtonOn;
    public int type;
    float smoothness = 0.1f;
    void Awake(){
        //collider = obj.GetComponent<BoxCollider>();
        ButtonOn = false;
    }
    void Start(){

    }
    void Update(){

    }
    void FixedUpdate(){

    }
    void OnTriggerEnter(Collider collision){
        if(!ButtonOn){
            ButtonOn = true;
            obj.transform.Translate(new Vector3(0,0.3f,0));
            if(type == 0) StartCoroutine("OpenDoor");
        }
    }
    IEnumerator OpenDoor(){
        float move = 6.0f;
        while(move > 0){
            move -= smoothness;
            target.transform.Translate(new Vector3(0,+smoothness,0));
            yield return new WaitForSeconds(smoothness);
        }
    }
}