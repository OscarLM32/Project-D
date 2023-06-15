using System.Collections;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Defines the behaviour of any type of currency pickable during a level
    /// </summary>
    public abstract class Currency : MonoBehaviour, IPickable
    {
        private AudioSource _source;
        private Collider2D _collider; 
        
        protected void Start()
        {
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
            _source.Play();
            _collider.enabled = false;

            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
        }  
    }
}