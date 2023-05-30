using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class Dew : MonoBehaviour
{
    public float speed;
    private float realSpeed;
    public float jumpPower;
    public float jumpVeloMax;
    private int jumpChance;
    private int jumpDelay;
    public float groundMaxDistance;
    private float MaxDistance;

    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5};
    double mod;
    bool modeChange;
    bool LifeUP;
    public int savePointNum;

    float smoothness = 0.02f;
    float lifetime;
    public float fullLife;

    GameObject obj;
    Rigidbody rigid;
    //GameManager gameManager;
    CapsuleCollider collider;

    Renderer rend;
    Color colorStart = new Color(57/255f, 221/255f, 208/255f, 255/255f);
    Color colorEnd = new Color(211/255f, 211/255f, 211/255f, 125/255f);

    private Camera camera;
    public float distance;

    int readValInt;
    SerialPort sp = new SerialPort("COM3", 9600);

    //public AudioClip audioFinish, audioJump;
    //AudioSource audioSource;
    void Awake(){
        rigid = GetComponent<Rigidbody>();
        rend = transform.GetChild(0).GetComponent<Renderer>();
        obj = GameObject.Find("Player");
        collider = GetComponent<CapsuleCollider>();
        camera = Camera.main;
    }
    void Start(){
        StartCoroutine("LifeDecrease");

        lifetime = fullLife;
        mod = 0;
        LifeUP = false;
        savePointNum = 0;
        modeChange = false;
        MaxDistance = groundMaxDistance;
    }
    void Update()
    {
        Debug.Log(lifetime);
        if(Delay(ref jumpDelay)) Jump();
        if(Delay(ref jumpDelay)) onGround();
        limitSpeed();
    }
    void FixedUpdate(){
        changeCamera();
        Move();
        CheckNumKey();
        if(modeChange) WaterStatus((float) mod);
    }
    //
    // Basic
    //
    bool Delay(ref int val){
        return val-- <= 0;
    }
    //
    // Arduino
    //
    void ArduinoSetup(){
        sp.Open();
        sp.ReadTimeout = 16;
        Debug.Log("Opening Port");
    }
    void ArduinoInput(){
        sp.Write("c");
        string readVal = sp.ReadLine();
        readValInt = int.Parse(readVal);
    }
    //
    // Player Moving
    //
    void Jump(){
        if (((Input.GetButtonDown("Jump")|| readValInt == 1) &&jumpChance-->0))
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpDelay = 60;
            //anim.SetBool("isJumping", true);

            //audioSource.clip = audioJump;
            //audioSource.Play();
        }
    }
    void Move(){
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(new Vector3(-realSpeed,0,0));
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(new Vector3(realSpeed,0,0));
        }
    }
    void onGround(){
        if(Physics.BoxCast(transform.position, 
                            transform.lossyScale / 3.0f, 
                            Vector3.down, 
                            out RaycastHit hit, new Quaternion(), 
                            MaxDistance)){
            if(rigid.velocity.y <= 0) jumpChance = savePointNum + 1;
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
    void limitSpeed() {
        if (rigid.velocity.y > jumpVeloMax) rigid.velocity = new Vector3(rigid.velocity.x, jumpVeloMax, 0);
        else rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 0);
    }
    //
    // Player Status
    //
    void OnTriggerStay(Collider collision){
        realSpeed = 0;
        //rigid.velocity.x = 0;
        if(collision.transform.CompareTag("Respawn")){
            LifeUP = true;
            Debug.Log("w2");
        }
        else LifeUP = false;
    }
    void OnTriggerExit(Collider collision){
        realSpeed = speed;
    }
    IEnumerator LifeDecrease(){
        while(lifetime > 0 && lifetime <= fullLife){
            if(LifeUP) lifetime += 5 * smoothness;
            else lifetime -= smoothness;
            ColorChange(rend);
            yield return new WaitForSeconds(smoothness);
        }
    }
    void ColorChange(Renderer rend){
        rend.material.color = Color.Lerp(colorEnd, colorStart, lifetime/10);
    }
    void CheckNumKey(){
        for(int i = 0; i<keyCodes.Length; i++){
            if(Input.GetKey(keyCodes[i])){ 
                mod = 0.5 + i * 0.25;
                modeChange = true;
            }
        }
    }
    void WaterStatus(float m){
        obj.transform.localScale = new Vector3(m, m, m);
        MaxDistance = groundMaxDistance * m;
        rigid.mass = 1.0f + 0.45f * (m-1);
        //jumpPower = 4 + 4 * (m-1);
        modeChange = false;
        distance = 10f + 5f * (m-1);
        speed = 0.05f - 0.01f * (m-1);
    }
    //
    // Camera Action
    //
    void changeCamera(){
        camera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -distance);
    }
}  
