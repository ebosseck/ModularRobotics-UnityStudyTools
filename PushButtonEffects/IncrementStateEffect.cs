using StudyTools.StateManager;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Script to increment states on the push button
/// </summary>
namespace StudyTools.PushButtonEffects
{
    public class IncrementStateEffect : MonoBehaviour
    {
        public AbstractStateManager stateManager;
        public float lockBetweenPush = 10;
    
        private float lastPush = -10;
    
        /// <summary>
        /// Called when the button is pressed
        /// </summary>
        /// <param name="fromHand">Hand pressing the button</param>
        public void OnButtonDown(Hand fromHand)
        {
            float dT = Time.unscaledTime - lastPush;
        
            if (stateManager.activeStateIsUserInput() && dT > lockBetweenPush)
            {
                stateManager.nextState();
                fromHand.TriggerHapticPulse(1000);
                lastPush = Time.unscaledTime;
            }
        }

        /// <summary>
        /// Called when the button is released
        /// </summary>
        /// <param name="fromHand">Hand releasing the button</param>
        public void OnButtonUp(Hand fromHand)
        {
            // Not Implemented
        }
    }

}

