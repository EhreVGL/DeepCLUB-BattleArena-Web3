using UnityEngine;
using TMPro;

public class ChatControl : MonoBehaviour
{
    public static ChatControl chat;
    public TextMeshProUGUI message, messageArea;
    private void Awake()
    {
        chat = this;
    }
    public void Submit()
    {
        ServerControl.server.mainAvatar.GetComponent<Avatar>().Submit();
        UIManager.uIManager.message.GetComponent<TMP_InputField>().text = "";
    }
}
