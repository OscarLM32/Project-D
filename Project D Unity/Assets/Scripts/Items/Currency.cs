using System.Collections;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Defines the behaviour of any type of currency pickable during a level
    /// </summary>
    public abstract class Currency : MonoBehaviour, IPickable
    {
        private Animator _animator;
        private AudioSource _source;
        private Collider2D _collider; 
        
        protected void Start()
        {
            _animator = GetComponent<Animator>();
            _source = GetComponent<AudioSource>();
            _collider = GetComponent<Collider2D>(); 
        }

        /// <summary>
        /// Hides the method PickUp by IPickable and turn it into an abstract method 
        /// </summary>
        public abstract void PickUp();

        /// <summary>
        /// The action to be performed whenever the currency is picked up
        /// </summary>
        /// <returns></returns>
        protected IEnumerator PickUpAction()
        {
            _animator.Play("Collection");
            _source.Play();
            _collider.enabled = false;

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            Destroy(gameObject);
        }  
    }
}