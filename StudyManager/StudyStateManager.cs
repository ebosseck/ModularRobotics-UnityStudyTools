using System;
using StudyTools.StateManager;
using StudyTools.StudyEvaluator;
using StudyTools.Utils;
using UnityEngine;


namespace StudyTools.StudyManager
{
    public class StudyStateManager: AbstractStateManager
    {
        [Header("Evaluation")] 
        public StudyStateEvaluator evaluator = null;
        
        [Header("Text Display")] 
        public bool displayObjectives = true;
        public TextMesh  objectiveOutput;
        
        public Color errorTextColor;
        public FontStyle errorStyle;
        
        public Color normalTextColor;
        public FontStyle normalStyle;

        [HideInInspector]
        public EventHandler onStateChange;

        private int state = 0;
        
        #region states

        #region generic states
        
        /// <summary>
        /// Check state function returning always successfully
        /// </summary>
        /// <returns>true</returns>
        public bool checkStateTrue()
        {
            return true;
        }
    
        /// <summary>
        /// Place holder for no setup required
        /// </summary>
        /// <returns>true</returns>
        public bool setupNone()
        {
            return true;
        }

        /// <summary>
        /// Waiting Objective
        /// </summary>
        /// <param name="color">out the color this text should be drawn in</param>
        /// <returns>Waiting Objective</returns>
        public string objectiveWait(out Color color)
        {
            color = Color.yellow;
            return "Please Wait...";
        }

        /// <summary>
        /// Generic error state function
        /// </summary>
        /// <returns>Generic Error</returns>
        public string errorGeneric()
        {
            return "State Failed: Unknown Reason";
        }

        #endregion

        #region tools
        
        /// <summary>
        /// Sets the transform of the given game object
        /// </summary>
        /// <param name="obj">GameObject whose transform to set</param>
        /// <param name="posX">X component of Posiition</param>
        /// <param name="posY">Y component of Posiition</param>
        /// <param name="posZ">Z component of Posiition</param>
        /// <param name="rotX">X component of Rotation</param>
        /// <param name="rotY">Y component of Rotation</param>
        /// <param name="rotZ">Z component of Rotation</param>
        /// <param name="scaleX">X component of local Scale</param>
        /// <param name="scaleY">Y component of local Scale</param>
        /// <param name="scaleZ">Z component of local Scale</param>
        public void setTransform(GameObject obj, double posX, double posY, double posZ, double rotX, double rotY,
            double rotZ, double scaleX, double scaleY, double scaleZ)
        {
            obj.transform.localPosition = new Vector3((float)posX, (float)posY, (float)posZ);
            obj.transform.localRotation = Quaternion.Euler((float)rotX, (float)rotY, (float)rotZ);
            obj.transform.localScale = new Vector3((float)scaleX, (float)scaleY, (float)scaleZ);
        }

        /// <summary>
        /// Enables the given game object
        /// </summary>
        /// <param name="obj"></param>
        public void enable(GameObject obj)
        {
            obj.SetActive(true);
        }

        /// <summary>
        /// Disables the given game object
        /// </summary>
        /// <param name="obj"></param>
        public void disable(GameObject obj)
        {
            obj.SetActive(false);
        }

        /// <summary>
        /// Attach the second game object to the first
        /// </summary>
        /// <param name="parent">Parent object</param>
        /// <param name="child">Child object</param>
        public void attach(GameObject parent, GameObject child)
        {
            child.transform.parent = parent.transform;
        }

        /// <summary>
        /// Detach the given object from its parent
        /// </summary>
        /// <param name="obj">Object to detach</param>
        public void detach(GameObject obj)
        {
            obj.transform.parent = null;
        }

        #endregion
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
        public bool loadState()
        {
            return loadState(this.state);
        }
        
        /// <summary>
        /// Loads the state with the given Index
        /// </summary>
        /// <param name="thisstate">Index of the state to load</param>
        public bool loadState(int thisstate)
        {
            
            if (checkStatePrequisites(thisstate))
            {
                if (displayObjectives)
                {
                    Color color;
                    string objective = getObjective(thisstate, out color);
                    showText(objective, color);
                }

                return setupState(thisstate);
            }
            
            return false;
        }
        
        /// <summary>
        /// Starts writing evaluation data for the given state
        /// </summary>
        /// <param name="state">state index</param>
        public void startEvaluation(int state)
        {
            if (evaluator != null)
            {
                evaluator.startState(state);
            }
        }

        /// <summary>
        /// Stops writing evaluation data for the given state
        /// </summary>
        /// <param name="state">state index</param>
        public void endEvaluation(int state)
        {
            if (evaluator != null)
            {
                evaluator.endState(state);
            }
        }
        
        #endregion

        #region Text

        #region Text Manager
        
        /// <summary>
        /// Show an error message on the mesh output
        /// </summary>
        /// <param name="message">Message to show</param>
        public void showError(string message)
        {
            if (objectiveOutput != null)
            {
                objectiveOutput.color = errorTextColor;
                objectiveOutput.fontStyle = FontStyle.Bold;
                objectiveOutput.text = message; 
            }
        }
        
        /// <summary>
        /// Show a normal message on the mesh output
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="color">Color to show the message from</param>
        public void showText(string message, Color color)
        {
            if (objectiveOutput != null)
            {
                objectiveOutput.color = color;
                objectiveOutput.fontStyle = FontStyle.Normal;
                objectiveOutput.text = message; 
            }
        }
        
        /// <summary>
        /// Clears the text from the text mesh
        /// </summary>
        public void clearText()
        {
            if (objectiveOutput != null)
            {
                objectiveOutput.color = Color.black;
                objectiveOutput.fontStyle = FontStyle.Normal;
                objectiveOutput.text = ""; 
            }
        }

        #endregion

        #endregion
        
        #region Overridables

        /// <summary>
        /// Checks if all prequisites for entering the given state are fulfilled
        /// </summary>
        /// <param name="state">index of the state to check</param>
        /// <returns>True if all prequisites are fulfilled, false otherwise</returns>
        public virtual bool checkStatePrequisites(int state)
        {
            // Not Implemented
            return true;
        }

        /// <summary>
        /// Sets the given state up
        /// </summary>
        /// <param name="state">index of the state</param>
        /// <returns>True if successfull</returns>
        public virtual bool setupState(int state)
        {
            // Not Implemented
            return true;
        }

        /// <summary>
        /// Gets the objective for the given state
        /// </summary>
        /// <param name="state">index of the state</param>
        /// <param name="color">out color of the objective</param>
        /// <returns>String containing the objective's text</returns>
        public virtual string getObjective(int state, out Color color)
        {
            color = normalTextColor;
            return "No Objectives";
        }

        /// <summary>
        /// Gets the error message if not all requirements are fulfilled
        /// </summary>
        /// <param name="state">index of the state to check</param>
        /// <returns>the error to display if the given state failed</returns>
        public virtual string getStateFailError(int state)
        {
            return "ERROR: Not all requirements fulfilled to perform next state";
        }

        /// <summary>
        /// Checks if the user should be able to continue to the next state
        /// </summary>
        /// <param name="state">index of the state</param>
        /// <returns>True, if the user should be able to manually continue, false otherwise</returns>
        public virtual bool isUserinputRequired(int state)
        {
            return false;
        }

        /// <summary>
        /// Checks if the active state requires user input
        /// </summary>
        /// <returns>True, if user input is required, false otherwise</returns>
        public override bool activeStateIsUserInput()
        {
            return isUserinputRequired(this.state);
        }
        
        #endregion

        
    }
}