﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
    public List<string> ChatMessages;
    public bool IsOfflineMode;

    // Goal #1: Create a reasonable chat system using the NEW UI System
    public void AddChatMessage(string message) {
        PhotonView.Get(this).RPC("AddChatMessageRPC", PhotonTargets.All, message);
    }

    [RPC]
    private void AddChatMessageRPC(string message) {
        // TODO: If message is longer than x characters, split it into multiple messages so that it fits inside the textbox
        while (ChatMessages.Count >= maxChatMessages) {
            ChatMessages.RemoveAt(0);
        }
        ChatMessages.Add(message);
    }

    void Start() {
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Matt");
        ChatMessages = new List<string>();

        if (IsOfflineMode) {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        } else {
            Connect();
        }
    }

    void Update() {
    }

    void Connect() {
        PhotonNetwork.ConnectUsingSettings("FPS Test v001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

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
        AddChatMessage("[SYSTEM] OnJoinedRoom!");
    }

    const int maxChatMessages = 7;
}
