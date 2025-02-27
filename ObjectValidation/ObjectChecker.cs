using System.Collections.Generic;
using EditorTools.Attributes;
using EditorTools.BaseTypes;
using UnityEngine;

namespace StudyTools.ObjectValidation
{
    /// <summary>
    /// Class to change colors of ghosts on collision with appropriate objects
    /// </summary>
    public class ObjectChecker : ExtendedMonoBehaviour
    {
        [Tag]
        public string objectTag = "";

        public Material material_unfulfilled;
        public Material material_fulfilled;
    
        private List<GameObject> connectedObjects = new List<GameObject>();
        
        /// <summary>
        /// Checks if this object has at least one appropriate object colliding
        /// </summary>
        /// <returns>True if fulfilled, false otherwise</returns>
        public bool isFulfilled()
        {
            return (connectedObjects.Count != 0);
        }

        /// <summary>
        /// Returns all appropriate objects colliding with this object
        /// </summary>
        /// <returns>a list of all connected objects</returns>
        public GameObject[] getConnectedObjects()
        {
            return connectedObjects.ToArray();
        }

        /// <summary>
        /// Unity Event, called on collision enter
        /// </summary>
        /// <param name="other">Collision partner</param>
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == objectTag)
            {
                if (!connectedObjects.Contains(other.gameObject))
                {
                    connectedObjects.Add(other.gameObject);
                    Renderer renderer = GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = material_fulfilled;
                    }
                }
            }
        }

        /// <summary>
        /// Unity Event, called on collision exit
        /// </summary>
        /// <param name="other">Collision partner</param>
        private void OnCollisionExit(Collision other)
        {
            if (connectedObjects.Contains(other.gameObject))
            {
                connectedObjects.Remove(other.gameObject);

                if (connectedObjects.Count == 0)
                {
                    Renderer renderer = GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = material_unfulfilled;
                    }
                }
            }
        }
    }
}