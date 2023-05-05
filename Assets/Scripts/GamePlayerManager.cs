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

    public override void OnNetworkSpawn()
    {
        setPlayerHealth();
        assignToPersistentPlayer();
    }

    void Update()
    {
        if (!IsServer) return;
        
        if (_health.Value <= 0)
        {
            _health.Value = _maxHealth;
        }
    }

    private void setPlayerHealth()
    {
        if (!IsServer) return;
        _health.Value = _maxHealth;
    }

    [ServerRpc (RequireOwnership = false)]
    public void RequestHitServerRpc(float mod, ulong damagerId, Vector3[] targetLocationHistory, Vector3 targetPosition)
    {
        // TODO Check this report can be legitimate
        if (HitDetection(targetPosition, targetLocationHistory)) {
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

    private bool HitDetection(Vector3 playerHitPosition, Vector3[] playerLocationHistory)
    {
        foreach (Vector3 i in playerLocationHistory)
        {
            Vector3 vectordiff = playerHitPosition - i;
            float locationDiff = vectordiff.sqrMagnitude;

            if (locationDiff < 0.025)
            {
                return true;
            }
        }
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
