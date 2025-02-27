using EditorTools.BaseTypes;

namespace StudyTools.StateManager
{
    public abstract class AbstractStateManager : ExtendedMonoBehaviour
    {
        /// <summary>
        /// Sets the state to the given state
        /// </summary>
        /// <param name="state">Index of the state to set</param>
        public virtual void setState(int state)
        {
            //Not Implemented
        }
        
        /// <summary>
        /// Sets the state to the next state (defaults to increment)
        /// </summary>
        public virtual void nextState()
        {
            //Not Implemented
        }
        
        /// <summary>
        /// Resets the StateManager to the first state (defaults to 0)
        /// </summary>
        public virtual void resetState()
        {
            setState(0);
        }
        /// <summary>
        /// Checks if the current state requires user input (in order to disable incrementing states while the robots are still running)
        /// </summary>
        /// <returns>True, if the state is a state, where input from the user is required (defaults to true)</returns>
        public virtual bool activeStateIsUserInput()
        {
            return true;
        }
    }
}