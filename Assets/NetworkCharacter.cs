using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {
    public float Speed = 10f;
    public float JumpSpeed = 6f;

    [HideInInspector]
    public Vector3 Direction = Vector3.zero;
    [HideInInspector]
    public bool IsJumping = false;

    // Use this for initialization
    void Start() {
        CacheComponents();
    }

    // Update is called once per frame
    void Update() {

    }

    void CacheComponents() {
        if (charController == null)
            charController = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        if (photonView.isMine) {
            DoLocalMovement();
        } else {
        }
    }

    void DoLocalMovement() {
        Vector3 distance = Direction * Speed * Time.deltaTime;

        if (IsJumping) {
            IsJumping = false;
            if (charController.isGrounded) {
                verticalVelocity = JumpSpeed;
            }
        }

        if (charController.isGrounded && verticalVelocity < 0) {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        } else {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        distance.y = verticalVelocity * Time.deltaTime;

        charController.Move(distance);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // OnPhotonSerializeView is called before Start sometimes, giving an NRE on animator.
        // This call prevents that. It's unfortunate though because this is called unnecessarily several times per second for the rest of the game
        // when it is not needed.
        CacheComponents();

        if (stream.isWriting) {
            // Our player. Need to send our actual position to network.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            // Someone else's player. Need to receive their position (as of a few milliseconds ago) and update our version of them.
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    CharacterController charController;
    float verticalVelocity = 0f;
}
