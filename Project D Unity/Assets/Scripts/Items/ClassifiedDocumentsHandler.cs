using System.Collections.Generic;
using CoreSystems.SaveLoadSystem;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Handles the spawning of the classified documents in each level checking if all the conditions are met properly
    /// </summary>
    public class ClassifiedDocumentsHandler : MonoBehaviour
    {
        private const int _numberOfClassifiedDocumentsPerLevel = 3;
        
        private bool[] _bitmap;
        //All the classified documents in the level must be children of this script
        private List<GameObject> _documents = new List<GameObject>();
        private void Start()
        {
            SpawnClassifiedDocuments();
        }

        /// <summary>
        /// Spawns the classified documents depending if they have been collected before or not
        /// </summary>
        private void SpawnClassifiedDocuments()
        {
            if(!CheckClassifiedDocumentIntegrity()) return;
            if (!CheckBitMapIntegrity()) return;

            for (int i = 0; i < _numberOfClassifiedDocumentsPerLevel; i++)
            {
                _documents[i].SetActive(_bitmap[i]);
            }
        }

        /// <summary>
        /// Checks if the classified documents have been set up properly in the level
        /// </summary>
        /// <return>If they have been properly set</return>
        private bool CheckClassifiedDocumentIntegrity()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                _documents.Add(transform.GetChild(i).gameObject);
                _documents[i].GetComponent<ClassifiedDocument>().position = i;
            }

            if (_documents.Count != _numberOfClassifiedDocumentsPerLevel)
            {
                AppLogger.LogError(DebugFlag.CLASSIFIED_DOCUMENTS_HANDLER,
                    "The number of classified documents in the level must be exactly 3");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the bitmap stored in the permanent data is a proper one
        /// </summary>
        /// <returns>If it is properly set up</returns>
        private bool CheckBitMapIntegrity()
        {
            _bitmap = SaveLoadManager.Instance.GetCurrentLevelClassifiedDocumentsBitmap();

            if (_bitmap.Length != _numberOfClassifiedDocumentsPerLevel)
            {
                AppLogger.LogError(DebugFlag.CLASSIFIED_DOCUMENTS_HANDLER, "The bitmap associated to the level must have exactly 3 bits");
                return false;
            }

            return true;
        }
    }
}