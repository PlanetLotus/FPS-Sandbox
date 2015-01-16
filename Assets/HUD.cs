using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {
    void Start() {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();

        // There's got to be a better way than this...
        Image[] backgrounds = GetComponentsInChildren<Image>(includeInactive: true);
        foreach (Image background in backgrounds) {
            if (background.name == "ChatBoxBackground")
                chatBoxBackground = background;
            else if (background.name == "ChatInputBackground")
                chatInputBackground = background;
        }

        Text[] texts = GetComponentsInChildren<Text>(includeInactive: true);
        foreach (Text text in texts) {
            if (text.name == "ChatBox")
                chatBox = text;
            else if (text.name == "ChatInput")
                chatInput = text;
            else if (text.name == "Health")
                healthIndicator = text;
        }

        playerHealth = GetComponentInParent<Health>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
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
        // TODO: Only do this if ChatMessages has changed recently
        chatBox.text = "";
        foreach (string message in networkManager.ChatMessages) {
            chatBox.text += message + "\n";
        }

        healthIndicator.text = "Health: " + playerHealth.CurrentHitPoints;
    }

    void EnableChatInput() {
        chatInputBackground.enabled = true;
        chatInput.enabled = true;

        // TODO: Turn off character motor while chat is open.
        // Couldn't figure out how to do this...don't see a way to disable here.
        // Might have to write own PlayerController script as done in FPS Tutorial, or download a C# version of it.
        //GetComponent("CharacterMotor").
    }

    void DisableChatInput() {
        // If there's anything in the chatbox other than the cursor (and return character), send it
        if (chatInput.text.Trim().Length > 2) {
            // Remove the "cursor"
            chatInput.text = chatInput.text.Substring(0, chatInput.text.Length - 1);

            // Send the chat over the network
            // TODO: Get this player's name to send along with the text
            networkManager.AddChatMessage("[TestPlayer] " + chatInput.text);
        }

        chatInput.text = "_";
        chatInputBackground.enabled = false;
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
    Image chatInputBackground;
    Text chatBox;
    Image chatBoxBackground;
    Text healthIndicator;
    NetworkManager networkManager;
    Health playerHealth;
}
