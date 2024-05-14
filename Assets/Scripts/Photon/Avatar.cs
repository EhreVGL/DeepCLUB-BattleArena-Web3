using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using Cinemachine;

public class Avatar : MonoBehaviour
{
    PhotonView photonView;
    PhotonAnimatorView animView;
    Animator avatarAnim;
    Rigidbody rb;
    float inputX;
    float inputZ;
    Vector3 movement;
    public float moveSpeed;
    public LayerMask groundLayer;
    public float turnSmoothTime = 2, turnSmoothVelocity;
    bool pressed = true;
    public GameObject canvas;
    public Text nickName;
    public List<Material> bodyMaterials, headMaterials;
    string avatarId;
    public AudioClip notification;
    bool wall, move = true;
    private void Awake()
    {
        canvas.transform.parent = null;
        photonView = GetComponent<PhotonView>();
        animView = GetComponent<PhotonAnimatorView>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        if (photonView.IsMine)
        {
            InvokeRepeating("LiftDown", 1, 6);
            UIManager.uIManager.birdSource.Play();
            ServerControl.server.cMFreeLook.SetActive(true);
#if UNITY_ANDROID || UNITY_IOS
            ServerControl.server.cMFreeLook.GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
            ServerControl.server.cMFreeLook.GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
#endif
            ServerControl.server.cMFreeLook.GetComponent<CinemachineFreeLook>().Follow = transform;
            ServerControl.server.cMFreeLook.GetComponent<CinemachineFreeLook>().LookAt = transform;
            ServerControl.server.minimapCam.GetComponent<MinimapCamFollow>().target = transform;
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            for (int i = 0; i < ServerControl.server.avatarsId.Count; i++)
            {
                avatarId = avatarId + ServerControl.server.avatarsId[i];
            }
        }
    }
    void Update()
    {
        if (!move)
        {
            return;
        }
        if (photonView.IsMine)
        {
            UIManager.uIManager.playerCount.text = GameObject.FindGameObjectsWithTag("Avatar").Length.ToString();
            photonView.RPC("MyName", RpcTarget.All, ServerControl.server.nickName, ServerControl.server.avatarsId[0], avatarId);
            if (ServerControl.server.chatAtcive)
            {
                return;
            }
            if (UIManager.uIManager.interact.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                Application.OpenURL("https://www.spatial.io/s/MARDIN-UMOB-22-5-ARCHERIORI-ATOLYE-SERGISI-65c7563c849e3776c719ee45?share=6929607632919475985");
            }
            //if (Input.GetKeyDown(KeyCode.Space) && ground)
            //{
            //    rb.velocity = Vector3.up * 4;
            //    avatarAnim.SetTrigger("Jump");
            //}
            //if (Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down * .1f, out hit, .2f, groundLayer))
            //{
            //    ground = true;
            //}
            //else
            //{
            //    ground = false;
            //}
            WebMove();
            AndroidMove();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && photonView.IsMine)
        {
            move = false;
            //transform.GetChild(ServerControl.server.avatarsId[0]).GetComponent<Animator>().SetBool("Walk", false);
            transform.GetChild(ServerControl.server.avatarsId[0]).gameObject.SetActive(false);
            //Debug.Log(transform.GetChild(ServerControl.server.avatarsId[0]).gameObject.name);
            ServerControl.server.mainShip.transform.DOMoveZ(ServerControl.server.mainShip.transform.position.z + 15, 2).SetEase(Ease.Linear).OnComplete(() => 
            {
                ServerControl.server.mainShip.GetComponent<Animator>().SetTrigger("Octopus");
                StartCoroutine(LeaveMainRoom());
                //PhotonNetwork.LeaveRoom();
            });
        }
        else if (other.gameObject.layer == 20 && photonView.IsMine)
        {
            UIManager.uIManager.Interact(true, "Interact");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 20 && photonView.IsMine)
        {
            UIManager.uIManager.Interact(false, "");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine && !collision.gameObject.CompareTag("Ground") && !collision.gameObject.CompareTag("Lift"))
        {
            wall = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (photonView.IsMine && !collision.gameObject.CompareTag("Ground") && !collision.gameObject.CompareTag("Lift"))
        {
            StartCoroutine(NotWall());
        }
    }
    void WebMove() 
    {
#if UNITY_WEBGL
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) ||
            Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            pressed = true;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            pressed = false;
        if (!pressed)
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
            if (wall)
            {
                inputX = -inputX;
                inputZ = -inputZ;
            }
            Vector3 direction = new Vector3(inputX, 0, inputZ).normalized;
            float targetAngle = 0;
            if (direction.magnitude >= .1f)
            {
                targetAngle = (float)Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
            movement = transform.forward;
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            transform.GetChild(ServerControl.server.avatarsId[0]).GetComponent<Animator>().SetBool("Walk", true);
        }
        else if(pressed && !UIManager.uIManager.ai)
        {
            transform.GetChild(ServerControl.server.avatarsId[0]).GetComponent<Animator>().SetBool("Walk", false);
        }
#endif
    }
    void AndroidMove()
    {
#if UNITY_ANDROID || UNITY_IOS
            FloatingJoystick moveJoystick = UIManager.uIManager.moveJoystick;
            bool active = moveJoystick.transform.GetChild(0).gameObject.activeSelf;
            if (active && moveJoystick.Horizontal > 0 || moveJoystick.Horizontal < 0 || moveJoystick.Vertical > 0 || moveJoystick.Vertical < 0)
            {
                float horizontal = moveJoystick.Horizontal;
                float vertical = moveJoystick.Vertical;
                if (wall)
                {
                    horizontal = -horizontal;
                    vertical = -vertical;
                }
                Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
                if (direction.magnitude >= .1f) 
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0, angle, 0);

                    movement = transform.forward;
                    transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
                    transform.GetChild(ServerControl.server.avatarsId[0]).GetComponent<Animator>().SetBool("Walk", true);
                }
                //Vector3 pos = new Vector3(horizontal + transform.position.x, 0, vertical + transform.position.z);
                //transform.LookAt(new Vector3(pos.x, 0, pos.z));
                //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                //movement = Vector3.forward;
                //transform.Translate(movement * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.GetChild(ServerControl.server.avatarsId[0]).GetComponent<Animator>().SetBool("Walk", false);
            }
#endif
    }
    IEnumerator NotWall()
    {
        yield return new WaitForSeconds(.15f);
        wall = false;
    }
    private void LiftDown()
    {
        ServerControl.server.lift.transform.DOLocalMoveY(.3f, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            ServerControl.server.lift.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = true;
            StartCoroutine(LiftUp(ServerControl.server.lift.gameObject));
        });
    }
    IEnumerator LiftUp(GameObject lift)
    {
        yield return new WaitForSeconds(2);
        lift.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
        lift.transform.DOLocalMoveY(2, 1).SetEase(Ease.Linear);
            }
    [PunRPC]
    void MyName(string myName, int id, string avatarId)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                animView.m_Animator = transform.GetChild(i).GetComponent<Animator>();
            }
        }
        nickName.text = myName;
        gameObject.name = myName;
        transform.GetChild(id).gameObject.SetActive(true);
        Transform avatar = transform.GetChild(id);
        avatar.GetComponent<Animator>().SetBool("Idle", true);
        SkinnedMeshRenderer avatarHead = avatar.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer avatarBody = avatar.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        bodyMaterials = new List<Material> { avatarBody.sharedMaterials[4] };
        bodyMaterials[0].name = "Skin";
        bodyMaterials[0].color = UIManager.uIManager.avatarButtons[2].buttons[Convert.ToInt32(avatarId.Substring(2, 1))].GetComponent<Button>().colors.normalColor;
        avatarBody.materials[4] = bodyMaterials[0];
        for (int i = 0; i < avatarHead.materials.Count(); i++)
        {
            if (avatarHead.materials[i].name == "sac (Instance)")
            {
                headMaterials = new List<Material> { avatarHead.sharedMaterials[i] };
                headMaterials[0].name = "Hair";
                headMaterials[0].color = UIManager.uIManager.avatarButtons[1].buttons[Convert.ToInt32(avatarId.Substring(1, 1))].GetComponent<Button>().colors.normalColor;
                avatarHead.materials[i] = headMaterials[0];
            }
            else if (avatarHead.materials[i].name == "ten (Instance)")
            {
                avatarHead.materials[i] = bodyMaterials[0];
            }
        }
        for (int i = 0; i < avatarBody.materials.Count() - 3; i++)
        {
            bodyMaterials = new List<Material> { avatarBody.sharedMaterials[i] };
            bodyMaterials[0].name = "Three";
            bodyMaterials[0].color = UIManager.uIManager.avatarButtons[i + 3].buttons[Convert.ToInt32(avatarId.Substring(i + 3, 1))].GetComponent<Button>().colors.normalColor;
            //avatarHead.materials[i] = bodyMaterials[0];
        }
    }
    public void Submit()
    {
        photonView = ServerControl.server.mainAvatar.GetPhotonView();
        photonView.RPC("SubmitText", RpcTarget.All, ServerControl.server.mainAvatar.GetPhotonView().Owner.NickName, ChatControl.chat.message.text);
        photonView.RPC("Notification", RpcTarget.Others, ChatControl.chat.message.text/*, ServerControl.server.messageArea*/);
    }
    [PunRPC]
    void SubmitText(string nickname, string message)
    {
        if (message.Length <= 25 && message.Length >= 2)
        {
            DateTime nowTime = DateTime.Now;
            if (nowTime.Hour != 0)
            {
                UIManager.uIManager.messageArea.text = UIManager.uIManager.messageArea.text + "\n" +
                    nickname + "[" + nowTime.Hour + "." + nowTime.Minute + "." + nowTime.Second + "]" + ": " + message;
            }
            else
            {
                UIManager.uIManager.messageArea.text = UIManager.uIManager.messageArea.text + "\n" +
                    nickname + "[" + "24" + "." + nowTime.Minute + "." + nowTime.Second + "]" + ": " + message;
            }
            float currentValue = UIManager.uIManager.messageArea.GetComponentInChildren<Scrollbar>().value;
            DOTween.To(() => currentValue, x => currentValue = x, 1, .1f).SetEase(Ease.Linear)
                        .OnUpdate(() =>
                         {
                             UIManager.uIManager.messageArea.GetComponentInChildren<Scrollbar>().value = currentValue;
                         });
        }
    }
    [PunRPC]
    void Notification(string message/*, TMP_InputField messageArea*/)
    {
        if (message.Length <= 25 && message.Length >= 2 && !UIManager.uIManager.messageArea.gameObject.activeSelf)
        {
            AudioSource.PlayClipAtPoint(notification, transform.position, .5f);
            UIManager.uIManager.warningImage.SetActive(true);
        }
    }
    IEnumerator LeaveMainRoom()
    {
        yield return new WaitForSeconds(1);
        UIManager.uIManager.birdSource.Play();
        UIManager.uIManager.LoadingStart(UIManager.uIManager.modLoading);
        ServerControl.server.mod = true;
        PhotonNetwork.LeaveRoom();
    }
}
