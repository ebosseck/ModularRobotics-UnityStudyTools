using System;
using StudyTools.StateManager;
using StudyTools.Utils;
using UnityEngine;

#if MTFS_ROS_CONNECTOR
using Visus.Robotics.RosBridge;
using ros_messages.std_msgs;
using ros_messages.vrsim_messages;
#endif

namespace StudyTools.StudyManager
{
    /// <summary>
    /// StudyManager connecting to ROS. If using ros, this is the main point of contact for incrementing / setting states
    /// </summary>
    public class ROSStudyManager: AbstractStateManager
    {
        [Header("General")] 
        [Tooltip("Ghost Manager, responsible for repositioning of ghosts between states")]
        public GhostManager.GhostManager ghostManager;
        
        [Tooltip("Study Manager, responsible for checking that all requirements for state transisions are met, and sets up scene for the next state")]
        public StudyStateManager studyManager;
        
        [Header("Text Display")] 
        public TextMesh textOutput;
        
        public Color normalTextColor = Color.black;
        public Color errorTextColor = Color.red;
        
        [Header("ROS Topics")]
        public string studyStepResultTopic = "/study/results";
        public string studyStepTopic = "/study/setstep";
        
        public EventHandler onStateChange;
        
        private int state = -1;
        private bool autoProgress = false;
#if MTFS_ROS_CONNECTOR        
        protected ROSConnection connection;
    
        /// <summary>
        /// Called on initialisation, Initializes connection to ROS
        /// </summary>
        void Awake()
        {
            connection = ROSConnection.GetOrCreateInstance();
        }
        
        /// <summary>
        /// Called after initialisation, registers publisher & subscriber for communication with ROS
        /// </summary>
        void Start()
        {
            connection.RegisterPublisher<UInt32Message>(studyStepTopic);
            
            Action<StudyStepResultMessage> onMessageAction = onStudyStepResult;
            connection.Subscribe<StudyStepResultMessage>(studyStepResultTopic, onMessageAction);

        }

        #region States
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
            if (studyManager.displayObjectives)
            {
                Color color;
                string objective = studyManager.getObjective(state, out color);
                studyManager.showText(objective, color);
            }
            studyManager.setupState(state);
            
            ghostManager.setState(state);
            
            this.autoProgress = !studyManager.isUserinputRequired(state);
            publishState();
        }

        // See AbstractStateManager
        public override void nextState()
        {
            if (studyManager.checkStatePrequisites((int)state+1))
            {

                studyManager.endEvaluation(state);
                setState((int)state+1);
                studyManager.startEvaluation(state);
            }
            else
            {
                logError(studyManager.getStateFailError((int)state+1));
            }
        }

        // See AbstractStateManager
        public override void resetState()
        {
            setState(0);
        }
        
        /// <summary>
        /// Publishes the current state to ROS
        /// </summary>
        private void publishState()
        {
            UInt32Message msg = new UInt32Message();
            msg.data = (uint)state;

            try
            {
                connection.Publish(studyStepTopic, msg);
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning(e + "\n Are you sure that the ROS Connection is initialized ?");
            }
            
        }
        
        // See AbstractStateManager
        public override bool activeStateIsUserInput()
        {
            return studyManager.isUserinputRequired(this.state);
        }

        #endregion
        
        #region Study Step Result handling
        /// <summary>
        /// Called when a study step result message is received
        /// </summary>
        /// <param name="msg">Message containing the study step result</param>
        private void onStudyStepResult(StudyStepResultMessage msg)
        {
            if (msg != null)
            {
                if (msg.hasError)
                {
                    logError(msg.message);
                }
                else
                {
                    logText("Successfully completed step " + msg.step + " with message: " + msg.message);
                    if (autoProgress)
                    {
                        logText("Auto-Progressing to next state....");
                        this.nextState();
                    }
                }
            }
        }

        /// <summary>
        /// Logs text and clears previous text
        /// </summary>
        /// <param name="msg">Message to log</param>
        private void logText(string msg)
        {
            Debug.Log(msg);
            clearText();
        }
        
        /// <summary>
        /// Logs an error and displays it on the set text mesh
        /// </summary>
        /// <param name="error">Error message to display</param>
        private void logError(string error)
        {
            Debug.LogError(error);
            showError(error);
        }
        
        #endregion

        #region Text Manager
        /// <summary>
        /// Show an error message on the mesh output
        /// </summary>
        /// <param name="message">Message to show</param>
        private void showError(string message)
        {
            if (textOutput != null)
            {
                textOutput.color = errorTextColor;
                textOutput.fontStyle = FontStyle.Bold;
                textOutput.text = message; 
            }
        }
        
        /// <summary>
        /// Show a normal message on the mesh output
        /// </summary>
        /// <param name="message">Message to show</param>
        private void showText(string message)
        {
            if (textOutput != null)
            {
                textOutput.color = normalTextColor;
                textOutput.fontStyle = FontStyle.Normal;
                textOutput.text = message; 
            }
        }
        
        /// <summary>
        /// Clears the text from the text mesh
        /// </summary>
        private void clearText()
        {
            if (textOutput != null)
            {
                textOutput.color = Color.black;
                textOutput.fontStyle = FontStyle.Normal;
                textOutput.text = ""; 
            }
        }

        #endregion
#else
        void Start()
        {
            Debug.LogError("ROS Connector not found. Please install the ROS Connector module in your project to use ROS Study Manager.");
        }
#endif
    }

}