using EditorTools.Attributes;
using EditorTools.Inspector;
using UnityEngine.UIElements;

namespace StudyTools.StateManager.Editor
{
    /// <summary>
    /// Custom Property Inspector for ManualStudyTester
    /// </summary>
    [CustomEditorInfo(typeof(ManualStudyTester))]
    public class ManualStudyTesterInspector : ExtendedBehaviourInspector
    {
        /// <summary>
        /// Called when the PropertyInspector GUI needs to be created
        /// </summary>
        /// <returns>root of the property inspector</returns>
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = base.CreateInspectorGUI();
            ManualStudyTester tester = (ManualStudyTester) serializedObject.targetObject;

            Button btn_setstate = new Button();
            btn_setstate.text = "SET STATE";
            btn_setstate.clicked += delegate { tester.setState(); };
            myInspector.Add(btn_setstate);
            
            Button btn_incstate = new Button();
            btn_incstate.text = "INCREMENT STATE";
            btn_incstate.clicked += delegate { tester.incrementState(); };
            myInspector.Add(btn_incstate);
            
            Button btn_resetstate = new Button();
            btn_resetstate.text = "RESET STATE";
            btn_resetstate.clicked += delegate { tester.resetState(); };
            myInspector.Add(btn_resetstate);
            
            return myInspector;
        }
        
    }
}