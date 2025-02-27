using EditorTools.Attributes;
using EditorTools.Inspector;
using EditorTools.InspectorTools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StudyTools.GhostManager.Editor
{
    /// <summary>
    /// Custom Property Inspector of Ghostmanager
    /// </summary>
    [CustomEditorInfo(typeof(GhostManager))]
    public class GhostManagerInspector : ExtendedBehaviourInspector
    {

        /// <summary>
        /// Called when the PropertyInspector GUI needs to be created
        /// </summary>
        /// <returns>root of the property inspector</returns>
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();
            GhostManager ghostManager = (GhostManager) serializedObject.targetObject;

            TagField tag = GUIElemetCreator.createTagField(serializedObject.FindProperty("ghostTag"), null);
            
            myInspector.Add(tag);
            
            Button btn_addToTracking = new Button();
            
            btn_addToTracking.text = "Add all objects with selected Tag";
            btn_addToTracking.clicked += delegate { ghostManager.addToTrackingByTag(); };
            
            myInspector.Add(btn_addToTracking);
            
            createInspectorAuto(myInspector);

            myInspector.Add(GUIElemetCreator.createHeaderLabel("Modify States"));

            Button btn_appendNew = new Button();
            
            btn_appendNew.text = "Append current as new State";
            btn_appendNew.clicked += delegate { ghostManager.createState(-1); };
            
            myInspector.Add(btn_appendNew);

            SerializedProperty property = serializedObject.FindProperty("overrideState");
            IntegerField stateField = GUIElemetCreator.createIntField(property, unit: null);
            stateField.label = "Replace State: ";
            
            Button btn_insertState = new Button();
            
            btn_insertState.text = "Replace";
            btn_insertState.clicked += delegate { ghostManager.createState(ghostManager.overrideState); };

            stateField.Add(btn_insertState);
            myInspector.Add(stateField);
            
            Button btn_updateStates = new Button();
            
            btn_updateStates.text = "Update states with missing Ghosts";
            btn_updateStates.clicked += delegate { ghostManager.updateMissingStates(); };
            
            myInspector.Add(btn_updateStates);
            
            return myInspector;
        }
        
    }
}