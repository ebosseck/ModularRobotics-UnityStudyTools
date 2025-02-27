using System;
using System.Collections.Generic;
using EditorTools.Attributes;

namespace StudyTools.GhostManager
{
    /// <summary>
    /// Dummy Class since Unity's properties do not support Lists of Arrays of objects
    /// </summary>
    [Serializable]
    public class GhostStateCollection
    {
        [NonResizeable]
        public GhostState[] states;

        public GhostStateCollection(List<GhostState> states)
        {
            this.states = states.ToArray();
        }
    }
}