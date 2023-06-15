using System;
using Level;
using Level.DifficultySettings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.ShootingEnemy
{
    /// <summary>
    /// It handles the behaviour of the shooting enemies
    /// </summary>
    public class ShootingEnemy : MonoBehaviour
    {
        /// <summary>
        /// The projectile it shoots
        /// </summary>
        public EnemyProjectileType projectileType;
        public ShootingDirection shootingDirection;

        public SOShootingEnemyDifficultySettings difficultySettings;

        private AudioSource _audioSource;
        
        /// <summary>
        /// The rate at which every attack is done
        /// </summary>
        private float _attackRate = 2f;
        private float _timeElapsedSinceLastAttack = -1f; //Delay the first attack

        private Vector2 _shootingDirectionVector;
        private Vector2 _shootingPosition;
        private Vector2 _shootingPositionOffset;
        
        /// <summary>
        /// Sets ups every aspect needed for the shooting
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            
            _shootingDirectionVector = GetShootingDirectionVector();
            _shootingPosition = transform.position;
            _shootingPositionOffset = _shootingDirectionVector;
            
            _shootingPosition += _shootingPositionOffset;

            _attackRate = difficultySettings.attackRate[(int) GameManager.Instance.difficulty];
        }

        private void Update()
        {
            if (_timeElapsedSinceLastAttack > _attackRate)
            {
                _timeElapsedSinceLastAttack = 0;
                Shoot();
            }
            _timeElapsedSinceLastAttack += Time.deltaTime;
        }

        /// <summary>
        /// Shoots a projectile
        /// </summary>
        private void Shoot()
        {
            var projectile = EnemyProjectilePool.I.GetProjectile(projectileType);
            projectile.GetComponent<EnemyProjectile>().SetUpForShooting(_shootingDirectionVector, _shootingPosition);
            _audioSource.Play();
        }

        /// <summary>
        /// Transforms the direction enum into a Vector2
        /// </summary>
        /// <returns>A vector2 representation of the direction</returns>
        private Vector2 GetShootingDirectionVector()
        {
            switch (shootingDirection)
            {
                case ShootingDirection.UP:
                    return Vector2.up;
                case ShootingDirection.DOWN:
                    return Vector2.down;
                case ShootingDirection.LEFT:
                    return Vector2.left;
                case ShootingDirection.RIGHT:
                    return Vector2.right;
                default:
                    Debug.LogError($"[ShootingEnemy]: The shooting direction {shootingDirection} is not a valid direction");
                    return Vector2.up;
            }
        }

        /// <summary>
        /// The different ways in which the player can shoot
        /// </summary>
        public enum ShootingDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }
    }
}