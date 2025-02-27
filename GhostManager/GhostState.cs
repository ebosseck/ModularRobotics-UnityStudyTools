using System;
using EditorTools.Attributes;
using UnityEngine;

namespace StudyTools.GhostManager
{
    /// <summary>
    /// GhostState of this class
    /// </summary>
    [Serializable]
    public class GhostState
    {
        public GameObject ghostObject;
        
        public bool isEnabled;

        public Transform parent;
        //public Transform ghostTransform;
        
        [Header("Transform")]
        [Unit("m")]
        public Vector3 ghostPosition;
        [Unit("°")]
        public Quaternion ghostRotation;
        public Vector3 ghostLocalScale;
        
        public Material ghostMaterial;
        
        /// <summary>
        /// Constructs a ghost state for the current configuration of the given game object
        /// </summary>
        /// <param name="ghost">Object to create the state for</param>
        public GhostState(GameObject ghost)
        {
            this.ghostObject = ghost;
            this.isEnabled = ghost.activeSelf;
            this.parent = ghostObject.transform.parent;
            
            this.ghostPosition = ghost.transform.position;
            this.ghostRotation = ghost.transform.rotation;
            this.ghostLocalScale = ghost.transform.localScale;
            
            if (ghost.GetComponent<Renderer>() != null)
            {
                this.ghostMaterial = ghost.GetComponent<Renderer>().sharedMaterial;
            }
        }

        /// <summary>
        /// Updates this state with the current configuration of the gameobject this state is for
        /// </summary>
        public void update()
        {
            this.isEnabled = ghostObject.activeSelf;
            this.parent = ghostObject.transform.parent;
            
            this.ghostPosition = ghostObject.transform.position;
            this.ghostRotation = ghostObject.transform.rotation;
            this.ghostLocalScale = ghostObject.transform.localScale;
            
            if (ghostObject.GetComponent<Renderer>() != null)
            {
                this.ghostMaterial = ghostObject.GetComponent<Renderer>().sharedMaterial;
            }
        }
        
        /// <summary>
        /// Applys this state to its game object
        /// </summary>
        public void apply()
        {
            this.ghostObject.SetActive(this.isEnabled);
            updateTransform();
            ghostObject.transform.parent = this.parent;
            if (this.ghostObject.GetComponent<Renderer>() != null)
            {
                this.ghostObject.GetComponent<Renderer>().material = this.ghostMaterial;
            }
        }

        /// <summary>
        /// Updates the transform used by this game object
        /// </summary>
        private void updateTransform()
        {
            this.ghostObject.transform.position = this.ghostPosition;
            this.ghostObject.transform.rotation = this.ghostRotation;
            this.ghostObject.transform.localScale = this.ghostLocalScale;
        }
    }
}