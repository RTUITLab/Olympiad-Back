using AntDesign;
using System.Collections.Generic;

namespace Olympiad.ControlPanel.Components
{
    public static class DescriptionsHelpers
    {
        public static  Dictionary<string, int> AdaptiveColumns { get; } = new()
        {
            { BreakpointType.Xxl.ToString(), 3 },
            { BreakpointType.Xl.ToString(), 3},
            { BreakpointType.Lg.ToString(), 2},
            { BreakpointType.Md.ToString(), 1},
            { BreakpointType.Sm.ToString(), 1},
            { BreakpointType.Xs.ToString(), 1}
        };
    }
}
