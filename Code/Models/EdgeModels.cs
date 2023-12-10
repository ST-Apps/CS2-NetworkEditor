using cohtml.Net;
using Game.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEditor.Models
{
    [CoherentType]
    internal struct UIEdge
    {
        [CoherentProperty]
        public UINode StartNode { get; set; }

        [CoherentProperty]
        public UINode EndNode { get; set; }

        [CoherentProperty]
        public UICompositionFlags CompositionFlags { get; set; }
    }

    [CoherentType]
    internal struct UINode
    {
        [CoherentProperty]
        public UICompositionFlags CompositionFlags { get; set; }
    }

    [CoherentType]
    internal struct UICompositionFlags
    {
        [CoherentProperty("general")]
        public Dictionary<string, bool> General { get; set; }

        [CoherentProperty("left")]
        public Dictionary<string, bool> Left { get; set; }

        [CoherentProperty("right")]
        public Dictionary<string, bool> Right { get; set; }
    }

    //[CoherentType]
    //internal struct UICompositionGeneralFlags
    //{

    //}

    //[CoherentType]
    //internal struct UICompositionSideFlags
    //{
    //    public CompositionFlags.Side 
    //}
}
