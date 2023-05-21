using CoreSystems.SaveLoadSystem;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Specific class defining the behaviour of the currency type: information
    /// </summary>
    public class Information : Currency
    {
        [SerializeField]private int _value;

        public override void PickUp()
        {
            SaveLoadManager.Instance.AddPickedInformation(_value);
            StartCoroutine(PickUpAction());
        }
    }
}