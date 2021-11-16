using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SocialDistancingForSidewalks.Components
{
    public class PeopleCountEstimateComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PeopleCountEstimateComponent class.
        /// </summary>
        public PeopleCountEstimateComponent()
          : base("EstimatePeoplePerMinute", "PplPerMin",
              "This components estimates the number of people that will pass this area during one minute",
              "SocialDistancingForSidewalks", "Estimate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Srf", "Surface", GH_ParamAccess.list);
            pManager.AddNumberParameter("Walking speed", "Wlkspeed", "Average walking mile speed per hour", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Hour", "H", "Hour of the day for estimate", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("People estimate", " PplEst", "Estimate of people passing this surface every minute", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> surfaces = new List<Brep>();
            double speed = 3;
            int hour = 0;

            if (!DA.GetDataList(0, surfaces)) return;
            if (!DA.GetData(1, ref speed)) return;
            if (!DA.GetData(2, ref hour)) return;


            // Average number of people observed in the study: https://www.mdpi.com/2071-1050/12/19/7863/htm 
            // The numbers were different according to the month and the type of neighbourhood
            // Values below are generic, looking at the average
            int[] activity = new int[]
            {
                300, 200, 100, 50, 50, 300,
                1000, 1200, 1500, 1700, 1500,
                1200, 2000, 2200, 2200, 2200,
                1600, 1700, 1800, 2000, 1300,
                1000, 900, 800
            };

            List<int> peopleCount = surfaces.Select(x => EstimatePeopleCount(x, speed, activity[hour])).ToList();

            DA.SetDataList(0, peopleCount);
        }

        int EstimatePeopleCount(Brep brep, double walkingSpeed, int numberOfPeople)
        {
            // distance extracted from brep edge
            var walkingMilesPerBrep = Utils.GetWalkingDistanceForBrep(brep);

            // how long doest it take to walk this distance? 
            var timeToWalk = walkingMilesPerBrep / walkingSpeed;

            // every person during this hour will take the time to walk it
            // calculate how many people on average is at once during this hour
            return (int)Math.Round(timeToWalk * numberOfPeople);
        }

       
        protected override System.Drawing.Bitmap Icon => Properties.Resources.PeopleEstimate;

        public override Guid ComponentGuid => new Guid("A1D448CB-C30A-44C8-BBAE-234046A1BCA7");

    }
}