using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
    // Goal #1: Create a reasonable chat system using the NEW UI System
    public void AddChatMessage(string message) {
        PhotonView.Get(this).RPC("AddChatMessageRPC", PhotonTargets.All, message);
    }

    [RPC]
    private void AddChatMessageRPC(string message) {
        // TODO: If message is longer than x characters, split it into multiple messages so that it fits inside the textbox
        while (chatMessages.Count >= maxChatMessages) {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(message);
    }

    void Start() {
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Matt");
        chatMessages = new List<string>();

        // There's got to be a better way than this...
        Text[] texts = GetComponentsInChildren<Text>(includeInactive: true);
        foreach (Text text in texts) {
            if (text.name == "ChatBox")
                chatBox = text;
            else if (text.name == "ChatInput")
                chatInput = text;
        }

        Debug.Log(chatBox.fontSize + ", " + chatBox.minHeight + ", " + chatBox.flexibleHeight + ", " + chatBox.preferredHeight);

        Connect();
    }

    void Update() {
        if (Input.GetButtonDown("Submit")) {
            if (chatInput.enabled) {
                DisableChatInput();
            } else {
                EnableChatInput();
            }
        }

        if (chatInput.enabled) {
            AddTextToChatInput();
        }

        /*
        if (PhotonNetwork.connectedAndReady) {
            AddChatMessage(Time.deltaTime.ToString());
        }
        */
    }

    void Connect() {
        PhotonNetwork.ConnectUsingSettings("FPS Test v001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        chatBox.text = "";
        foreach (string message in chatMessages) {
            chatBox.text += message + "\n";
        }
    }

    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        AddChatMessage("OnJoinedRoom!");
    }

    void EnableChatInput() {
        chatInput.enabled = true;
    }

    void DisableChatInput() {
        // Remove the "cursor"
        chatInput.text = chatInput.text.Substring(0, chatInput.text.Length - 1);

        // Send the chat over the network
        AddChatMessage(chatInput.text);

        chatInput.text = "_";
        chatInput.enabled = false;
    }

    void AddTextToChatInput() {
        if (chatInput.text.Length > chatInputMaxLength)
            return;

        // Remove the "cursor"
        chatInput.text = chatInput.text.Substring(0, chatInput.text.Length - 1);

        foreach (char c in Input.inputString) {
            if (c == '\b' && chatInput.text.Length != 0) {
                chatInput.text = chatInput.text.Substring(0, chatInput.text.Length - 1);
            } else {
                chatInput.text += c;
            }
        }

        // Add the "cursor" back
        chatInput.text += "_";
    }

    const int maxChatMessages = 10;
    const int chatInputMaxLength = 30;
    List<string> chatMessages;
    Text chatBox;
    Text chatInput;
}
