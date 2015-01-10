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
        chatBox = GetComponentInChildren<Text>();
        Debug.Log(chatBox.fontSize + ", " + chatBox.minHeight + ", " + chatBox.flexibleHeight + ", " + chatBox.preferredHeight);
        chatMessages = new List<string>();

        Connect();
    }

    void Update() {
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

    const int maxChatMessages = 10;
    List<string> chatMessages;
    Text chatBox;
}
