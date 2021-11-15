using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SocialDistancingForSidewalks
{
    public class SocialDistancingForSidewalksInfo : GH_AssemblyInfo
    {
        public override string Name => "SocialDistancingForSidewalks";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "This plugin analyzes the NYC sidewalks and visualises the oppourtinity for social distancing.";

        public override Guid Id => new Guid("71D49AE4-8D46-4445-8315-544658684E63");

        //Return a string identifying you or your company.
        public override string AuthorName => "Justyna Szychowska";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "justyna.szy@gmail.com";
    }
}