using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO.Ports;
using UnityEngine.UI;

[System.Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip audio;
    }
[System.Serializable]
    public struct Particles{
        public string name;
        public ParticleSystem effect;
    }
[System.Serializable]
    public struct UI{
        public Canvas image;
        public bool flag;
    }
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
    //bool DoubleJump = false;

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

    GameObject fade;
    GameObject end;
    Color FadeColor = new Color(0.0f, 0.0f, 0.0f, 255/255f);
    Color FadeTrans = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color endColor;

    private Camera camera;
    public float CameraDistance;
    bool freeze;
    bool MoveLock;
    bool FreezeAnyKey;

    int readValInt;
    SerialPort sp = new SerialPort("COM3", 9600);

    public Sound[] Sounds;
    AudioSource audioSource;

    private Material originalMaterial;
    public Material transparentMaterial;

    public Particles[] Effects;

    public UI[] Images;
    int disImg;
    void Awake(){
        ArduinoSetup();

        rigid = GetComponent<Rigidbody>();
        graphic = transform.GetChild(0);
        rend = transform.GetChild(0).GetComponent<Renderer>();
        obj = GameObject.Find("Player");
        collider = GetComponent<CapsuleCollider>();
        camera = Camera.main;
        fade = GameObject.Find("Fade");
        end = GameObject.Find("EndPoint");
        endColor = end.transform.GetComponent<Renderer>().material.color;
        lifetime = fullLife;

        mod = 1;
        LifeUP = false;
        savePointNum = 0;
        lastMod = mod;
        MaxDistance = groundMaxDistance;
        realSpeed = speed;
        freeze = false;
        FreezeAnyKey = false;
        audioSource = GetComponent<AudioSource>();
        disImg = -1;
    }
    void Start(){
        originalMaterial = rend.material;
        StartCoroutine("LifeDecrease");
        DisplayImage(0);
    }
    void Update()
    {
        //Debug.Log(lifetime);
        if(!MoveLock){
            FreezeKey();
            if(Delay(ref jumpDelay)) Jump();
            if(Delay(ref jumpDelay)) onGround();
        }
        limitSpeed();
    }
    void FixedUpdate(){
        ArduinoInput();
        changeCamera();
        SerializeImage(0, 1);
        SerializeImage(1, 2);
        SerializeImage(3, 4);
        if(!MoveLock){
            Move();
            CheckNumKey();
            Reset();
        } 
        if(lastMod != mod){
            if(lastMod < mod) Audio(2);
            else Audio(3);
            WaterStatus((float) mod);
        }
    }
    //
    // Basic
    //
    void Audio(int num){
        audioSource.clip = Sounds[num].audio;
        audioSource.Play();
    }
    bool Delay(ref int val){return val-- <= 0;}
    void Freeze(){
        freeze = true;
        Time.timeScale = 0;
    }
    void UnFreeze(){
        freeze = false;
        Time.timeScale = 1.0f;
    }
    void FreezeKey(){
        if(FreezeAnyKey){
            if(Input.anyKeyDown){
                FreezeAnyKey = false;
                Audio(6);
                /*
                for(int i=0; i<Images.Length;i++){
                    Images[i].image.GetComponent<RawImage>().enabled = false;
                }*/
                Images[disImg].image.GetComponent<RawImage>().enabled = false;
                Images[disImg].flag = true;
                UnFreeze();
            }
        }else{
            if(Input.GetKeyDown(KeyCode.P)){
                if(!freeze) Freeze();
                else UnFreeze();
            } 
        }
    }
    //
    // Arduino
    //
    void ArduinoSetup(){
        try{
            sp.Open();
            sp.ReadTimeout = 16;
            Debug.Log("Opening Port");
        } catch {
            readValInt = 0;
            Debug.Log("Warn :: Arduino Input Not Available");
        }
        
    }
    void ArduinoInput(){
	try{
	    if(readValInt >= 0){
            p.Write("c");
            string readVal = sp.ReadLine();
            readValInt = int.Parse(readVal);
        }
	} catch {}
        
    }
    //
    // Player Moving
    //
    void Jump(){
        if (
            ((Input.GetButtonDown("Jump")|| readValInt == 1) &&jumpChance-->0)
            || readValInt >= 20
        ){
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpDelay = 60;
            //anim.SetBool("isJumping", true);
            Audio(0);
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
                if(rigid.velocity.y < -2){
                    Effects[0].effect.Play();
                    Audio(5);
                }
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
                DisplayImage(3);
                collision.GetComponent<Collider>().GetComponent<AudioSource>().Play();
            }
            collision.GetComponent<Collider>().transform.parent.GetComponent<AudioSource>().Play();
            Effects[0].effect.Play();
        }
        if(collision.GetComponent<Collider>().CompareTag("Finish")){
            StartCoroutine("EndFade");
            collision.GetComponent<Collider>().GetComponent<AudioSource>().Play();
            Invoke("Ending", 2.0f);
            
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
            if(LifeUP && lifetime <= fullLife) lifetime += 15 * smoothness;
            else lifetime -= smoothness;
            if(!MoveLock)ColorChange(rend);
            yield return new WaitForSeconds(smoothness);
        }
        OnDie();
    }
    void ColorChange(Renderer rend){
        rend.material.color = Color.Lerp(colorEnd, colorStart, lifetime/fullLife);
    }
    void CheckNumKey(){
        int jumpVal = readValInt % 10;
        if(jumpVal != 0) mod = 0.5 + jumpVal * 0.25;
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
        realSpeed = 0;
        Audio(1);
        Audio(4);
        MoveLock = true;
        rigid.velocity = new Vector3(0,0,0);
        rend.material = transparentMaterial;
        Effects[1].effect.Play();
        Invoke("Respawn", 2.0f);
    }
    void Respawn(){
        StartCoroutine("FadeInOut");
        //FadeIn();
        CameraDistance = 10f + 5f * ((float)mod-1);
        MoveLock = false;
        realSpeed = speed;
        rend.material = originalMaterial;
        lifetime = fullLife;
        StartCoroutine("LifeDecrease");
        switch(savePointNum){
            case 0:
                transform.position = new Vector3(0,0,0);
                break;
            case 1:
                transform.position = new Vector3(71, -9, 0);
                break;
            default:
                break;
        }
        //Invoke("FadeOut", 0.3f);
    }
    void Reset(){
        if (Input.GetKey(KeyCode.R)){
            OnDie();
        }
    }
    void Ending(){
        SceneManager.LoadScene("EndAnimation");
    }
    //
    // Camera Action
    //
    void changeCamera(){
        camera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -CameraDistance);
    }
    IEnumerator ZoomIn(){
        //cameraLock = true;
        while(CameraDistance > 1){
            //camera.transform.Translate = new Vector3(0, 0, smoothness);
            CameraDistance -= 25 *smoothness;
            yield return new WaitForSeconds(smoothness);
        }
        //cameraLock = false;
    }
    IEnumerator FadeInOut(){
        float progress = 1;
        while(progress > 0){
            progress -= 2 * smoothness;
            fade.transform.GetComponent<Renderer>().material.color = Color.Lerp(FadeTrans, FadeColor, progress);
            yield return new WaitForSeconds(smoothness);
        }
        /*
        while(progress > 0){
            progress -= smoothness;
            fade.transform.GetComponent<Renderer>().material.color = Color.Lerp(FadeColor, FadeTrans, progress);
            yield return new WaitForSeconds(smoothness);
        }*/
    }
    IEnumerator EndFade(){
        float progress = 0;
        while(progress < 1){
            progress += smoothness/2;
            end.transform.GetComponent<Renderer>().material.color = Color.Lerp(endColor, FadeColor, progress);
            yield return new WaitForSeconds(smoothness);
        }
    }
    //
    // UI
    //
    void DisplayImage(int n){
        if(!Images[n].flag){
            Freeze();
            FreezeAnyKey = true;
            Images[n].image.GetComponent<RawImage>().enabled = true;
            disImg = n;
        }
    }
    void SerializeImage(int prev, int next){
        if(Images[prev].flag){
            DisplayImage(next);
        }
    }
}  
