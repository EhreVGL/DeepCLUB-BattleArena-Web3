﻿using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Solana.Unity.SDK;

public class ServerControl : MonoBehaviourPunCallbacks
{
    public static ServerControl server;
    public List<GameObject> character;
    public GameObject mainAvatar, player;
    public List<Transform> spawnPoints;
    public List<GameObject> floors;
    public GameObject  mainFloor, mainShip;
    public int chooseChar, modId;
    public List<int> avatarsId;
    public GameObject photonObj;
    public GameObject avatar;
    //public string avatarAddressableKey;
    public int /*myTeamNumber, teamNumber,*/ step, viewId, index, killCount;
    public string nickName;
    public bool chatAtcive;
    public GameObject electric, lift, portal;
    float electricPosX, electricPosZ;
    public bool start, leave, mod;
    public List<GameObject> powerups, collectable;
    Vector3 camPos, shipPos;
    Quaternion camRot;
    int roomIndex, lobbyIndex;
    public List<GameObject> newPlayers;
    public ParticleSystem lightning;
    public float chooseSound, effectSound;
    public GameObject minimapCam, cMFreeLook;
    [SerializeField] AudioClip win, gameOver;
    float result, myResult;
    public float coinCount, tokenCount;
    public bool wallet;
    //[SerializeField] AssetLabelReference assetLabel;
    DefaultPool pool;
    //Server-Lobby-Room
    private void Awake()
    {
        server = this;
    }
    private void Start()
    {
        //avatar = GameObject.Find("NewAvatar");
        //avatar = Instantiate(Resources.Load<GameObject>("Prefabs/NewAvatar"));
        //pos = floor.transform.GetChild(0).position;
        //pool = PhotonNetwork.PrefabPool as DefaultPool;
        //if (pool != null && avatar != null)
        //{
        //    foreach (GameObject avatar in avatar)
        //    {
        //        pool.ResourceCache.Add(avatar.name, avatar);
        //    }
        //}

        camPos = Camera.main.transform.position;
        camRot = Camera.main.transform.rotation;
        shipPos = mainShip.transform.position;
        //play.onClick.AddListener(CharacterChoose);
        //PhotonNetwork.ConnectUsingSettings();//Servera bağlanma isteği
        //PhotonNetwork.JoinLobby();//Lobiye bağlanma
        //PhotonNetwork.JoinRoom("Oda ismi");//Özel bir odaya bağlanma
        //PhotonNetwork.JoinRandomRoom();//Rastgele bir odaya bağlanma
        //PhotonNetwork.CreateRoom("Oda ismi", roomOptions);//Oda yaratma
        //PhotonNetwork.JoinOrCreateRoom("Oda ismi", roomOptions);//Oda oluştur ve bağlan
        //PhotonNetwork.LeaveRoom();//Odadan çık
        //PhotonNetwork.LeaveLobby();//Lobiden çık
    }
    private void Update()
    {
        ElectricStart();
        WinAndLose();
    }
    void ElectricStart()
    {
        if (step == 2 && !start)
        {
            electric.gameObject.SetActive(true);
            electricPosX = 58;
            electricPosZ = 80;
            electric.transform.localScale = new Vector3(electricPosX, electric.transform.localScale.y, electricPosZ);
            UIManager.uIManager.playerCount.text = GameObject.FindGameObjectsWithTag("Player").Length.ToString() + " / " + 4;
            wallet = FindObjectOfType<Web3>().wallet;
        }
        if (GameObject.FindGameObjectsWithTag("Player").Length == 4)
        {
            start = true;
        }
    }
    void WinAndLose()
    {
        if (start)
        {
            for (int i = 0; i < newPlayers.Count; i++)
            {
                if (newPlayers[i] != null)
                {
                    if (newPlayers[i].GetComponent<Player>().newPlayer == null)
                    {
                        newPlayers[i].GetComponent<Animator>().SetBool("Idle", true);
                        newPlayers[i].SetActive(true);
                        newPlayers[i].GetComponent<Player>().canvas.SetActive(true);
                    }
                }
            }
            UIManager.uIManager.playerCount.text = GameObject.FindGameObjectsWithTag("Player").Length.ToString() + " / " + 4;
            if (player != null && PhotonNetwork.CurrentRoom.Name.Contains("R"))
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            electricPosX -= Time.deltaTime / 100;
            electricPosZ -= Time.deltaTime / 100;
            electric.transform.localScale = new Vector3(electricPosX, electric.transform.localScale.y, electricPosZ);
            if (!leave && player == null && GameObject.FindGameObjectsWithTag("Player").Length >= 2)
            {
                UIManager.uIManager.win.text = (GameObject.FindGameObjectsWithTag("Player").Length + 1).ToString() + ".";
                Lose();
            }
            else if(!leave && player == null && GameObject.FindGameObjectsWithTag("Player").Length < 2 && GameObject.FindGameObjectsWithTag("Player").Length >= 1)
            {
                myResult = GameObject.FindGameObjectsWithTag("Player").Length + 1;
                UIManager.uIManager.win.text = (GameObject.FindGameObjectsWithTag("Player").Length + 1).ToString() + ".";
                Win();
            }
            else if (!leave && player != null && GameObject.FindGameObjectsWithTag("Player").Length == 1)
            {
                myResult = GameObject.FindGameObjectsWithTag("Player").Length;
                UIManager.uIManager.win.text = (GameObject.FindGameObjectsWithTag("Player").Length).ToString() + ".";
                UIManager.uIManager.leave.gameObject.SetActive(true);
                Win();
            }
        }
    }
    void Win()
    {
        AudioSource.PlayClipAtPoint(win, transform.position, .5f);
        UIManager.uIManager.win.transform.parent.GetComponent<Image>().sprite = UIManager.uIManager.winSprite;
        UIManager.uIManager.win.transform.parent.gameObject.SetActive(true);
        result = (4 - myResult) * 50;
        if (wallet)
        {
            result += (tokenCount * 50);
            UIManager.uIManager.collect.text = "(" + (4 - myResult).ToString() + " x" + " 50" + " + " + tokenCount.ToString() +
                " x" + " 50" + ")" + "*" + (1 + (XPManager.xp.xpLevel * .1f)) + " = " + result.ToString();
            int token = int.Parse(UIManager.uIManager.token.text);
            int coin = int.Parse(UIManager.uIManager.coin.text);
            UIManager.uIManager.token.text = (token + result).ToString();
            UIManager.uIManager.coin.text = (coin + result).ToString();
        }
        else
        {
            result += (coinCount * 25);
            UIManager.uIManager.collect.text = "(" + (4 - myResult).ToString() + " x" + " 50" + " + " + tokenCount.ToString() +
                " x" + " 25" + ")" + " * " + (1 + (XPManager.xp.xpLevel * .1f)) + " = " + result.ToString();
            int coin = int.Parse(UIManager.uIManager.coin.text);
            UIManager.uIManager.coin.text = (coin + result).ToString();
        }
        float level = XPManager.xp.xpLevel * .1f;
        result *= (1 + level);
        leave = true;
    }
    void Lose()
    {
        AudioSource.PlayClipAtPoint(gameOver, transform.position, .5f);
        UIManager.uIManager.win.transform.parent.GetComponent<Image>().sprite = UIManager.uIManager.loseSprite;
        UIManager.uIManager.win.transform.parent.gameObject.SetActive(true);
        if (wallet)
        {
            result = (tokenCount * 50);
            UIManager.uIManager.collect.text = "(" + tokenCount.ToString() + " x" + " 50" + ")" + " * " + 
                (1 + (XPManager.xp.xpLevel * .1f)) + " = " + result.ToString();
            int token = int.Parse(UIManager.uIManager.token.text);
            int coin = int.Parse(UIManager.uIManager.coin.text);
            UIManager.uIManager.token.text = (token + result).ToString();
            UIManager.uIManager.coin.text = (coin + result).ToString();
        }
        else
        {
            result = (coinCount * 25);
            UIManager.uIManager.collect.text = "(" + tokenCount.ToString() + " x" + " 25" + ")" + " * " +
                (1 + (XPManager.xp.xpLevel * .1f)) + " = " + result.ToString();
            int coin = int.Parse(UIManager.uIManager.coin.text);
            UIManager.uIManager.coin.text = (coin + result).ToString();
        }
        float level = XPManager.xp.xpLevel * .1f;
        result *= (1 + level);
        leave = true;
    }
    //1. step bittikten sonra 2. stepe başlamak için oyuna girme
    void MainExit()
    {
        chatAtcive = false;
        UIManager.uIManager.NewStepOne();
        Camera.main.transform.position = camPos;
        Camera.main.transform.rotation = camRot;
        PhotonNetwork.JoinLobby();
    }
    void ModActive()
    {
        modId = -1;
        chatAtcive = false;
        UIManager.uIManager.StepOne();
        mainFloor.SetActive(false);
        mainShip.SetActive(false);
        cMFreeLook.SetActive(false);
        Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
        PhotonNetwork.JoinLobby();
    }
    public void Rejoin()
    {
        floors[modId].SetActive(true);
        PhotonNetwork.JoinLobby();
        UIManager.uIManager.StepTwo();
    }
    void GameExit()
    {
        electric.gameObject.SetActive(false);
        for (int i = 0; i < floors.Count; i++)
        {
            floors[i].SetActive(false);
        }
        for (int i = 0; i < powerups.Count; i++)
        {
            powerups[i].SetActive(false);
            powerups[i].transform.GetChild(1).gameObject.SetActive(false);
            powerups[i].transform.GetChild(powerups[i].transform.childCount - 1).rotation = Quaternion.Euler(-90, 0, 0);
            powerups[i].transform.GetChild(powerups[i].transform.childCount - 2).rotation = Quaternion.Euler(-90, 90, 270);
            powerups[i].transform.GetChild(powerups[i].transform.childCount - 3).rotation = Quaternion.Euler(-90, 180, 180);
            powerups[i].transform.GetChild(powerups[i].transform.childCount - 4).rotation = Quaternion.Euler(-90, 270, 90);
        }
        for (int i = 0; i < collectable.Count; i++)
        {
            collectable[i].SetActive(false);
            collectable[i].transform.GetChild(1).gameObject.SetActive(false);
            collectable[i].transform.GetChild(collectable[i].transform.childCount - 1).rotation = Quaternion.Euler(-90, 0, 0);
            collectable[i].transform.GetChild(collectable[i].transform.childCount - 2).rotation = Quaternion.Euler(-90, 90, 270);
            collectable[i].transform.GetChild(collectable[i].transform.childCount - 3).rotation = Quaternion.Euler(-90, 180, 180);
            collectable[i].transform.GetChild(collectable[i].transform.childCount - 4).rotation = Quaternion.Euler(-90, 270, 90);
        }
        mainShip.transform.position = shipPos;
        Camera.main.transform.position = camPos;
        Camera.main.transform.rotation = camRot;
        UIManager.uIManager.StepZero();
        PhotonNetwork.JoinLobby();
    }
    public override void OnConnectedToMaster()
    {
        //Servera bağlanınca çalışan callback fonksiyon
        Debug.Log("Servera bağlandı");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobiye bağlandı");
        if (mod)
        {
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Mod", roomOptions, TypedLobby.Default);
        }
        else if (step == 0)
        {
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Home", roomOptions, TypedLobby.Default);
        }
        else if (step == 1)
        {
            Debug.Log("Lobby" + modId + lobbyIndex);
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Lobby" + modId + lobbyIndex, roomOptions, TypedLobby.Default);
        }
        else if (step == 2)
        {
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Room" + modId + roomIndex, roomOptions, TypedLobby.Default);
        }
    }
    //void LoadAddressablePrefab()
    //{
    //    Addressables.LoadAssetAsync<GameObject>(assetLabel).Completed +=
    //       (asyncOperationHandle) =>
    //       {
    //           if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
    //           {
    //               pool = PhotonNetwork.PrefabPool as DefaultPool;
    //               if (pool != null)
    //               {
    //                   Debug.Log(asyncOperationHandle.Result.name);
    //                   pool.ResourceCache.Add(asyncOperationHandle.Result.name, asyncOperationHandle.Result);
    //                   mainAvatar = PhotonNetwork.Instantiate(pool.ResourceCache[asyncOperationHandle.Result.name].name, Vector3.zero, Quaternion.identity, (byte)pool.ResourceCache[asyncOperationHandle.Result.name].GetPhotonView().ViewID, null);
    //                   mainAvatar.GetPhotonView().Owner.NickName = nickName;
    //               }
    //           }
    //           else
    //           {
    //               Debug.Log("adsad");
    //           }
    //       };

    //    //mainAvatar = Instantiate(handle.Result, transform.position, transform.rotation);

    //    // Handle'ı release etmek önemlidir.
    //    //Addressables.Release(handle);
    //}
    public override void OnJoinedRoom()
    {
        if (mod)
        {
            mod = false;
            chooseChar = -1;
            modId = -1;
            UIManager.uIManager.LoadingComplete(UIManager.uIManager.modLoading);
        }
        else if (step == 0)
        {
            UIManager.uIManager.LoadingComplete(UIManager.uIManager.mainLoading);
            //LoadAddressablePrefab();
            pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool.ResourceCache.Count == 0)
            {
                pool.ResourceCache.Add(avatar.name, avatar);
            }
            mainAvatar = PhotonNetwork.Instantiate(pool.ResourceCache[avatar.name].name, Vector3.zero, Quaternion.identity, (byte)pool.ResourceCache[avatar.name].GetPhotonView().ViewID, null);
            mainAvatar.GetPhotonView().Owner.NickName = nickName;
        }
        else if (step == 1)
        {
            player = PhotonNetwork.Instantiate(photonObj.name, Vector3.zero, Quaternion.identity, (byte)photonObj.GetPhotonView().ViewID, null);
            UIManager.uIManager.LoadingComplete(UIManager.uIManager.gameBeforeLoading);
        }
        else if (step == 2)
        {
            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].transform.position = floors[modId].transform.GetChild(floors[modId].transform.childCount - 5 - i).position;
                powerups[i].SetActive(true);
                powerups[i].layer = 9;
            }
            for (int i = 0; i < collectable.Count; i++)
            {
                collectable[i].transform.position = floors[modId].transform.GetChild(floors[modId].transform.childCount - 1 - i).position;
                collectable[i].SetActive(true);
                collectable[i].layer = 9;
            }
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i] = floors[modId].transform.GetChild(floors[modId].transform.childCount - 1 - i - 8);
            }
            player = PhotonNetwork.Instantiate(character[chooseChar].name, spawnPoints[index].position - (Vector3.up / 2), Quaternion.identity, (byte)character[chooseChar].GetPhotonView().ViewID, null);
            player.SetActive(false);
            for (int i = 0; i < PhotonNetwork.PlayerList.Count(); i++)
            {
                if (PhotonNetwork.PlayerList[i].NickName == nickName)
                {
                    player.transform.position = spawnPoints[i].position;
                }
            }
            player.GetPhotonView().Owner.NickName = nickName;
            player.gameObject.SetActive(true);
        }
        //Odaya bağlanınca çalışan callback fonksiyon
        Debug.Log("Odaya bağlandı");
    }
    public override void OnLeftRoom()
    {
        //Odadan ayrılınca çalışan callback fonksiyon
        if (mod)
        {
            ModActive();
        }
        else if (step == 0)
        {
            step = 1;
            MainExit();
        }
        else if (step == 1)
        {
            step = 2;
            Rejoin();
        }
        else if (step == 2)
        {
            leave = false;
            start = false;
            step = 0;
            lobbyIndex = 0;
            roomIndex = 0;
            GameExit();
        }
        Debug.Log("Odadan ayrıldı");
    }
    public override void OnLeftLobby()
    {
        //Lobiden ayrılınca çalışan callback fonksiyon
        Debug.Log("Lobiden ayrıldı");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //Bir odaya girmeye çalışınca hata oluşursa çalışan callback fonksiyon
        if (step == 1)
        {
            lobbyIndex++;
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Lobby" + modId + lobbyIndex, roomOptions, TypedLobby.Default);
        }
        else if (step == 2)
        {
            roomIndex++;
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom("Room" + modId + roomIndex, roomOptions, TypedLobby.Default);
        }
        Debug.Log("Herhangi bir odaya girilemedi");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Rastgele bir odaya girmeye çalışınca hata oluşursa çalışan callback fonksiyon
        Debug.Log("Rastgele bir odaya girilemedi");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Bir odaya yaratmaya esnasında hata oluşursa çalışan callback fonksiyon
        Debug.Log("Oda oluşturulurken hata oluştu");
    }
}
