using Valve.VR.InteractionSystem;

namespace StudyTools.StudySettings
{
    /// <summary>
    /// Class for persistant storage of settings over multiple scenes for studies
    /// </summary>
    public class StudySettingsManager
    {
        private static StudySettingsManager _instance;
        public static StudySettingsManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StudySettingsManager();
                }

                return _instance;
            }
            
        }

        public Hand dominantHand;
        
        private StudySettingsManager()
        {
            
        }
    }
}


