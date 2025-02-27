using EditorTools.BaseTypes;

namespace StudyTools.StateManager
{
    public class ManualStudyTester: ExtendedMonoBehaviour
    {
        public int state = 0;

        
        public AbstractStateManager stateManager;
        
        /// <summary>
        /// Sets the state of the state manager
        /// </summary>
        public void setState()
        {
            stateManager.setState(state);
        }

        /// <summary>
        /// Increments the state of the state manager
        /// </summary>
        public void incrementState()
        {
            state++;
            stateManager.nextState();
        }

        /// <summary>
        /// Resets the state of the state manager
        /// </summary>
        public void resetState()
        {
            state = 0;
            stateManager.resetState();
        }
    }
}