using System;
using EditorTools.Attributes;
using EditorTools.Inspector;
using StudyTools.GhostManager;


namespace StudyTools.GhostManager.Editor
{
    /// <summary>
    /// Dummy Class to ensure custom property inspector generator is used for GhostStateCollection
    /// </summary>
    [CustomEditorInfo(typeof(GhostStateCollection))]
    public class GhostStateCollectionInspector : ExtendedBehaviourInspector
    {
        // Not Implemented
    }
}