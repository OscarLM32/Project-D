using System.Collections;
using CoreSystems.SaveLoadSystem;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Specific class defining the behaviour of the type of currency classified documents
    /// </summary>
    public class ClassifiedDocument : Currency
    {
        [Tooltip("Specifies the ordinal position in which this classified document appears in relation to the other two in the level.")]
        public int position = -1;
        public override void PickUp()
        {
            if (position == -1)
            {
                Debug.LogError("The classified document has not been properly set up");
                return;
            } 
            
            SaveLoadManager.Instance.AddPickedClassifiedDocument(position);
            StartCoroutine(PickUpAction()); 
        }
    }
}