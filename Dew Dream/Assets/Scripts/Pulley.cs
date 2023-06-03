using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pulley : MonoBehaviour
{
    public GameObject obj;
    double length;
    double angle;
    double x;

    void Awake(){
        length = obj.transform.localScale.x;
        x = length;
    }
    void Start(){

    }
    void Update(){
        
    }
    void FixedUpdate(){
        x = ( 1 / Math.Cos(obj.transform.eulerAngles.z) ) * length;
        //Debug.Log(( 1 / Math.Cos(obj.transform.eulerAngles.z) ));
        obj.transform.localScale = new Vector3((float)x, 1, 1);
    }
}