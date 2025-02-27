using System;

namespace StudyTools.Utils
{
    /// <summary>
    /// State Changed Event Args
    /// </summary>
    public class StateChangeEventArgs : EventArgs
    {
        public uint oldState { get; protected set; }
        public uint newState { get; protected set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldState">index of the previous state</param>
        /// <param name="newState">index of the next state</param>
        public StateChangeEventArgs(uint oldState, uint newState)
        {
            this.oldState = oldState;
            this.newState = newState;
        }
    }
}