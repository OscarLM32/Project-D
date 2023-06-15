using UnityEngine;

namespace Level
{
    /// <summary>
    /// Handles the logic behind every checkpoint in the level updating the player's spawn position whenever
    /// it is triggered by the player
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        private void Start()
        {
            CheckColliderIntegrity();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player.Player player = other.GetComponent<Player.Player>();
            if (player)
            {
                GameManager.Instance.playerSpawnPoint = transform.position;
                var spawnRotation = other.transform.rotation;
                spawnRotation.eulerAngles += new Vector3(0,0,180);
                GameManager.Instance.playerSpawnRotation = spawnRotation;
            }
        }

        /// <summary>
        /// Checks if the collider of the checkpoint has been properly set
        /// </summary>
        private void CheckColliderIntegrity()
        {
            var col = GetComponent<Collider2D>();
            if (!col)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.isTrigger = true;
        }
    }
}