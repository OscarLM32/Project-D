using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Encapsulates the behaviour of the different enemies that simply patrol around the level from one
    /// point to another making use of DoTween.
    /// </summary>
    public class PatrollingEnemy : MonoBehaviour
    {
        [Header("Patrol attributes")] 
        public float patrolDuration;
        public Ease easeType = Ease.Linear;
        public Vector3[] patrolPoints;

        //The enemies are going to move linearly, the same way as the player;
        private const PathType _pathType = PathType.Linear;
        private const PathMode _pathMode = PathMode.Sidescroller2D;
        private const int _resolution = 0;//The resolution is useless in linear paths. 

        [SerializeField]private LoopType _loopType = LoopType.Yoyo; 
        private const int _loops = -1; //Infinite
        
        private void Start()
        {
            CheckColliderIntegrity();
            StartPatrolling();
        }

        private void OnDisable()
        {
            transform.DOKill(); //Kill all the tweens referencing this component
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            Player.Player player = col.gameObject.GetComponent<Player.Player>();
            if (player)
            {
                player.Kill();
            }
        }

        /// <summary>
        /// Starts the patrol movement based on the parameters specified
        /// </summary>
        private void StartPatrolling()
        {
            transform.DOLocalPath(
                patrolPoints,
                patrolDuration,
                _pathType,
                _pathMode,
                _resolution
            )
                .SetLoops(_loops, _loopType)
                .SetEase(easeType);
        }

        /// <summary>
        /// Checks of the collider of the enemy has been set properly
        /// </summary>
        private void CheckColliderIntegrity()
        {
            var col = GetComponent<Collider2D>();
            if (!col)
            {
                Debug.LogWarning("["+gameObject.name+"]: This enemy does not possess a collider. Adding a box collider");
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.isTrigger = true; //Make sure that the "is trigger" value is set to true
        }
    }
}