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
    public double mod;
    double lastMod;
    bool LifeUP;
    public int savePointNum;
    bool DoubleJump = false;

    float smoothness = 0.02f;
    float lifetime;
    public float fullLife;

    GameObject obj;
    Rigidbody rigid;
    //GameManager gameManager;
    CapsuleCollider collider;
    Transform graphic;

    Renderer rend;
    Color colorStart = new Color(57/255f, 221/255f, 208/255f, 255/255f);
    Color colorEnd = new Color(211/255f, 211/255f, 211/255f, 125/255f);

    private Camera camera;
    public float CameraDistance;
    bool cameraLock;

    int readValInt;
    SerialPort sp = new SerialPort("COM3", 9600);

    public AudioClip audioJump;
    AudioSource audioSource;

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        graphic = transform.GetChild(0);
        rend = transform.GetChild(0).GetComponent<Renderer>();
        obj = GameObject.Find("Player");
        collider = GetComponent<CapsuleCollider>();
        camera = Camera.main;

        lifetime = fullLife;
        
        mod = 1;
        LifeUP = false;
        savePointNum = 0;
        lastMod = mod;
        MaxDistance = groundMaxDistance;
        realSpeed = speed;
        cameraLock = false;
        audioSource = GetComponent<AudioSource>();
    }
    void Start(){
        StartCoroutine("LifeDecrease");
    }
    void Update()
    {
        //Debug.Log(lifetime);
        if(Delay(ref jumpDelay)) Jump();
        if(Delay(ref jumpDelay)) onGround();
        limitSpeed();
    }
    void FixedUpdate(){
        if(!cameraLock) changeCamera();
        Move();
        CheckNumKey();
        if(lastMod != mod) WaterStatus((float) mod);
    }
    //
    // Basic
    //
    bool Delay(ref int val){return val-- <= 0;}
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

            audioSource.clip = audioJump;
            audioSource.Play();
        }
    }
    void Move(){
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(new Vector3(-realSpeed,0,0));
            graphic.eulerAngles = new Vector3(-90,0,70);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(new Vector3(realSpeed,0,0));
            graphic.eulerAngles = new Vector3(-90,0,30);
        }
    }
    void onGround(){
        if(Physics.BoxCast(transform.position, 
                            transform.lossyScale / 3.0f, 
                            Vector3.down, 
                            out RaycastHit hit, new Quaternion(), 
                            MaxDistance)){
            if(rigid.velocity.y <= 0){
                //if(DoubleJump) jumpChance = 2;
                //else jumpChance = 1;
                jumpChance = savePointNum + 1;
            } 
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
    // Collision
    //
    void OnTriggerEnter(Collider collision){
        if(collision.GetComponent<Collider>().CompareTag("Respawn")){
            int Snum = collision.GetComponent<Collider>().GetComponent<SavePoint>().SavePointNum;
            if(savePointNum < Snum){
                savePointNum = Snum;
            }
        }
    }
    void OnTriggerStay(Collider collision){
        if(collision.GetComponent<Collider>().CompareTag("Terrain")){
            realSpeed = 0;
        }
        if(collision.GetComponent<Collider>().CompareTag("Respawn")){
            LifeUP = true;
        }
        else LifeUP = false;
    }
    void OnTriggerExit(Collider collision){
        if(collision.GetComponent<Collider>().CompareTag("Terrain")){
            realSpeed = speed;
        }
        if(collision.GetComponent<Collider>().CompareTag("Respawn")){
            LifeUP = false;
        }
    }
    //
    // Player Status
    //
    IEnumerator LifeDecrease(){
        while(lifetime > 0){
            if(LifeUP && lifetime <= fullLife) lifetime += 10 * smoothness;
            else lifetime -= smoothness;
            ColorChange(rend);
            yield return new WaitForSeconds(smoothness);
        }
        Debug.Log("Die");
    }
    void ColorChange(Renderer rend){
        rend.material.color = Color.Lerp(colorEnd, colorStart, lifetime/fullLife);
    }
    void CheckNumKey(){
        for(int i = 0; i<keyCodes.Length; i++){
            if(Input.GetKey(keyCodes[i])){ 
                mod = 0.5 + i * 0.25;
                //modeChange = true;
            }
        }
    }
    void WaterStatus(float m){
        obj.transform.localScale = new Vector3(m, m, m);
        MaxDistance = groundMaxDistance * m;
        rigid.mass = 1.0f + 0.4f * (m-1);
        //jumpPower = 4 + 4 * (m-1);
        
        CameraDistance = 10f + 5f * (m-1);
        speed = 0.05f - 0.01f * (m-1);

        transform.Translate(new Vector3(0,(float) (0.5f * (m-lastMod)),0));
        lastMod = m;
    }
    void OnDie(){
        Invoke("Respawn", 1.5f);
        lifetime = fullLife;
        StartCoroutine("LifeDecrease");
    }
    void Respawn(){
        switch(savePointNum){
            case 0:
                transform.position = new Vector3(0,0,0);
                break;
            case 1:
                transform.position = new Vector3(71, -5, 0);
                break;
            default:
                break;
        }
    }
    //
    // Camera Action
    //
    void changeCamera(){
        camera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -CameraDistance);
    }
    IEnumerator ZoomIn(){
        cameraLock = true;
        while(true){
            
            yield return new WaitForSeconds(smoothness);
        }
        cameraLock = false;
    }
}  
