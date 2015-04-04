using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public const int MaxHitPoints = 100;

    public int CurrentHitPoints { get { return currentHitPoints; } }

    [RPC]
    public void TakeDamage(int amount) {
        currentHitPoints -= amount;

        if (currentHitPoints <= 0) {
            Die();
        }
    }

    void Start() {
        currentHitPoints = MaxHitPoints;
    }

    private void OnGUI() {
        if (GetComponent<PhotonView>().isMine) {
            if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 40), "Suicide")) {
                Die();
            }
        }
    }

    void Update() {

    }

    void Die() {
        Debug.Log("DETH");

        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView.instantiationId == 0) {
            Destroy(gameObject);
        } else {
            if (photonView.isMine) {
                NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
                nm.StandbyCamera.SetActive(true);
                nm.RespawnTimer = 3f;

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    int currentHitPoints;
}
