using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOperator : MonoBehaviour
{

    [System.Serializable]
    public struct BgmType
    {
        public string name;
        public AudioClip audio;
    }

    // Inspector 에표시할 배경음악 목록
    public BgmType[] BGMList;
    public GameObject Player;
    int currentNum;
    private AudioSource BGM;
    private string NowBGMname = "";
    void Awake(){
        currentNum = 0;
    }
    void Start()
    {
        BGM = gameObject.AddComponent<AudioSource>();
        BGM.loop = true;
        if (BGMList.Length > 0) PlayBGM(BGMList[currentNum].name);
    }
    void Update(){
        if(Player.GetComponent<Dew>().savePointNum > currentNum){
            currentNum = Player.GetComponent<Dew>().savePointNum;
            Debug.Log("nextBGm");
            PlayBGM(BGMList[currentNum].name);
        }
    }
    public void PlayBGM(string name)
    {
        if (NowBGMname.Equals(name)) return;

        for (int i = 0; i < BGMList.Length; ++i)
            if (BGMList[i].name.Equals(name))
            {
                BGM.clip = BGMList[i].audio;
                BGM.Play();
                NowBGMname = name;
            }
    }
}