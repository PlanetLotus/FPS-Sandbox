using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Responsible for actually moving a character.
/// For local characters, we read things like "direction" and "isJumping" and then affect the character controller.
/// For remote characters, we skip that and simply update the raw transform position based on info we received over the network.
/// </summary>
public class NetworkCharacter : MonoBehaviour {
    // Use this for initialization
    void Start() {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();

        // There's got to be a better way than this...
        Text[] texts = GetComponentsInChildren<Text>(includeInactive: true);
        foreach (Text text in texts) {
            if (text.name == "ChatBox")
                chatBox = text;
            else if (text.name == "ChatInput")
                chatInput = text;
        }
    }

    // Update is called once per frame
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
    }

    void OnGUI() {
        chatBox.text = "";
        foreach (string message in networkManager.ChatMessages) {
            chatBox.text += message + "\n";
        }
    }

    void EnableChatInput() {
        chatInput.enabled = true;
    }

    void DisableChatInput() {
        // If there's anything in the chatbox other than the cursor (and return character), send it
        if (chatInput.text.Length > 2) {
            // Remove the "cursor"
            chatInput.text = chatInput.text.Substring(0, chatInput.text.Length - 1);

            // Send the chat over the network
            networkManager.AddChatMessage(chatInput.text);
        }

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

    const int chatInputMaxLength = 30;
    Text chatInput;
    Text chatBox;
    NetworkManager networkManager;
}
