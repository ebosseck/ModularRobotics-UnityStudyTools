using System;
using System.Collections.Generic;
using StudyTools.StateManager;
using StudyTools.Utils;
using UnityEngine;

namespace StudyTools.GhostManager
{
    
    /// <summary>
    /// StateManager for managing Ghosts displayed in the scene
    /// </summary>
    public class GhostManager : AbstractStateManager
    {
        [HideInInspector]
        public String ghostTag = "";
        
        public GameObject[] trackedGhosts;

        public List<GhostStateCollection> ghostStates = new List<GhostStateCollection>();

        [HideInInspector]
        public EventHandler onStateChange;

        private int state = 0;
        
        [HideInInspector]
        public int overrideState;
        
        /// <summary>
        /// Gets the number of ghost states currently used by this ghost manager
        /// </summary>
        /// <returns></returns>
        public int getStateCount()
        {
            return ghostStates.Count;
        }
        
        /// <summary>
        /// Gets the current state of this manager
        /// </summary>
        /// <returns>the current state of this ghost manager</returns>
        public int getCurrentState()
        {
            return state;
        }

        #region Tracking Tools
        
        /// <summary>
        /// Selects all objects with ghostTag and adds them to the list of objects tracked
        /// </summary>
        public void addToTrackingByTag()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(ghostTag);
            List<GameObject> canidates = new List<GameObject>(objects);
            foreach (GameObject c in trackedGhosts)
            {
                if (!canidates.Contains(c))
                {
                    canidates.Add(c);
                }
            }

            trackedGhosts = canidates.ToArray();
        }

        #endregion
        
        #region State Creation
        
        /// <summary>
        /// Creates a new state. if stateGoal is an index of an already existing state, this state will be overweitten
        /// </summary>
        /// <param name="stateGoal">index of the state targeted</param>
        public void createState(int stateGoal)
        {
            if (stateGoal < ghostStates.Count && stateGoal >= 0)
            {
                ghostStates[stateGoal] = computeStates();
            }
            else
            {
                ghostStates.Add(computeStates());
            }
        }

        /// <summary>
        /// Removes the state with the given index from the list of states
        /// </summary>
        /// <param name="state">State to remove</param>
        public void removeState(int state)
        {
            if (state < ghostStates.Count && state >= 0)
            {
                ghostStates.RemoveAt(state);
            }
        }
        
        /// <summary>
        /// Updates all states with all objects in tracking but missing in that particular state
        /// </summary>
        public void updateMissingStates()
        {
            for (int i = 0; i < ghostStates.Count; i++) {
                loadState(i);
                ghostStates[i] = computeStates();
            }
        }

        /// <summary>
        /// Returns the current state of the scene as a new ghost state collection
        /// </summary>
        /// <returns>the computed GhostStateCollection</returns>
        private GhostStateCollection computeStates()
        {
            
            List<GhostState> state = new List<GhostState>();
            
            foreach (GameObject ghost in trackedGhosts)
            {
                state.Add(new GhostState(ghost));
            }

            return new GhostStateCollection(state);
        }

        #endregion
        
        #region State Management
        /// <summary>
        /// Fires the state changed event
        /// </summary>
        /// <param name="oldState">index of the old state</param>
        /// <param name="newState">index of the new state</param>
        private void fireStateChange(uint oldState, uint newState)
        {
            if (onStateChange != null)
            {
                onStateChange.Invoke(this, new StateChangeEventArgs(oldState, newState));
            }

        }

        // See AbstractStateManager
        public override void setState(int state)
        {
            fireStateChange((uint)this.state, (uint)state);
            this.state = state;
            loadState();
        }

        // See AbstractStateManager
        public override void nextState()
        {
            fireStateChange((uint)state, (uint)state + 1);
            state += 1;
            loadState();
        }

        // See AbstractStateManager
        public override void resetState()
        {
            setState(0);
        }
        
        /// <summary>
        /// Loads the current state
        /// </summary>
        public void loadState()
        {
            loadState(this.state);
        }
        
        /// <summary>
        /// Loads the state with the given Index
        /// </summary>
        /// <param name="thisstate">Index of the state to load</param>
        public void loadState(int thisstate)
        {
            if (thisstate >= 0 && thisstate < ghostStates.Count)
            {
                GhostState[] state = this.ghostStates[thisstate].states;

                foreach (GhostState s in state)
                {
                    s.apply();
                }
            }
        }

        #endregion
    }
}