using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    [Header("Rush")] 
    [SerializeField] private float RushTime = 0.5f;
    [SerializeField] private float RushSpeed = 16.0f;
    [SerializeField] private float RushCooldown = 1.0f;
    
    [Header("Move")]
    [SerializeField] private float MoveSpeed = 8.0f;
    [SerializeField] private float RotateSpeed = 10.0f;

    public Action<GameObject> OnHitObstacle;
    public Action OnAwayObstacle;
    public CapsuleCollider PlayerCollider { get; private set; }

    private Animator PlayerAnimator;
    private Player Player;
    private float LastRushTime;

    private void Awake() {
        PlayerAnimator = GetComponent<Animator>();
        PlayerCollider = GetComponent<CapsuleCollider>();
        Player = GetComponent<Player>();
    }

    private void Start() {
        if (!Player.IsOwner) {
            this.enabled = false;
            return;
        }
        InputManager.Instance.OnRun += OnRun;
        NetworkGameManager.Instance.OnSceneChange += OnSceneChange;
    }

    private void OnDestroy() {
        if (!Player.IsOwner) return;
        InputManager.Instance.OnRun -= OnRun;
        NetworkGameManager.Instance.OnSceneChange -= OnSceneChange;
    }

    private void OnSceneChange() {
        this.LastRushTime = 0.0f;
        this.transform.position = NetworkGameManager.Instance.GetPlayerPosition(Player.OwnerIndex);
        this.transform.rotation = Quaternion.identity; 
        PlayerAnimator.SetBool(AnimationParams.IsWalking, false);
    }

    private void OnRun() {
        if (!this.enabled) return;
        if (Time.time - LastRushTime < RushCooldown) return;
        StartCoroutine(RushCoroutine());
    }

    private IEnumerator RushCoroutine() {
        this.enabled = false;
        for (float t = 0.0f; t < RushTime; t += Time.deltaTime) {
            if (PlayerCollider.CollisionDetect(out RaycastHit _, this.transform.forward, RushSpeed * Time.deltaTime)) {
                break;
            }
            this.transform.position += Time.deltaTime * RushSpeed * this.transform.forward;
            yield return null;
        }
        this.enabled = true;
        LastRushTime = Time.time;
    }

    private void Update() { 
          if (!GameManager.Instance.IsPlaying) return;
          Vector3 velocity = InputManager.Instance.GetMoveVectorNormalized();
          if (velocity == Vector3.zero) {
              PlayerAnimator.SetBool(AnimationParams.IsWalking, false);
              return;
          }
          
          Quaternion target = Quaternion.LookRotation(velocity, Vector3.up);
          this.transform.rotation = Quaternion.Slerp(this.transform.rotation, target, RotateSpeed * Time.deltaTime);
          
          bool hitObstacle = PlayerCollider.CollisionDetect(out RaycastHit hit, velocity, MoveSpeed * Time.deltaTime);
          this.PlayerAnimator.SetBool(AnimationParams.IsWalking, !hitObstacle);
          if (hitObstacle) {
              this.OnHitObstacle?.Invoke(hit.collider.gameObject);
          } else {
              this.transform.position += Time.deltaTime * MoveSpeed * velocity;
              this.OnAwayObstacle?.Invoke();
          }
    }
}
