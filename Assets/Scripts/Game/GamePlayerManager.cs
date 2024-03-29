 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// GamePlayerManager runs the player specific logic when in game
public class GamePlayerManager : NetworkBehaviour
{
    [SerializeField] public float _maxHealth = 100;
    public NetworkVariable<float> _health = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private PositionHistoryManager _positionHistoryManager;
    [SerializeField] private ViewController _viewController;
    public bool _canMove;

    public override void OnNetworkSpawn()
    {
        setPlayerHealth();
        assignToPersistentPlayer();
    }

    public void Update()
    {
        if (_canMove) _viewController.CheckRotation();
    }

    private void setPlayerHealth()
    {
        if (!IsServer) return;
        _health.Value = _maxHealth;
    }

    [ServerRpc (RequireOwnership = false)]
    public void RequestHitServerRpc(float mod, ulong damagerId, Vector3 targetPosition)
    {
        if (IsValidHit(targetPosition)) {
            _health.Value += mod;
            if (_health.Value > _maxHealth)
            {
                _health.Value = _maxHealth;
            }
            else if (_health.Value <= 0)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().ReportDeathServerRpc(OwnerClientId, damagerId);
            }
        }
    }

    private bool IsValidHit(Vector3 playerHitPosition)
    {
        Vector3[] positionHistory = _positionHistoryManager.GetPositionHistory();

        foreach (Vector3 i in positionHistory)
        {
            Vector3 vectordiff = playerHitPosition - i;
            float locationDiff = vectordiff.sqrMagnitude;

            if (locationDiff < 0.025)
            {
                Debug.Log("Valid Hit");
                return true;
            }
            Debug.Log($"Invalid check - {playerHitPosition} in {i} - distance {locationDiff}");
        }
        Debug.Log("Invalid Hit");
        return false;
    }

    // assignToPersistentPlayer runs on client to assign this to the owning PersistentPlayerObject
    private void assignToPersistentPlayer()
    {
        if (!IsClient || !IsOwner) return;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>().gamePlayer = this;
    }

    [ClientRpc]
    public void SetPositionAndRotationClientRpc(Vector3 pos, Quaternion rotation, ClientRpcParams clientRpcParams = default)
    {
        transform.SetPositionAndRotation(pos, rotation);
    }
}
