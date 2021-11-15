using System;
using System.Collections.Generic;

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
            pManager.AddBrepParameter("Surface", "Srf", "Surface", GH_ParamAccess.list);
            pManager.AddNumberParameter("Walking speed", "Wlkspeed", "Average walking mile speed per hour", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hour", "H", "Hour of the day for estimate", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("People estimate", " PplEst", "Estimate of people passing this surface every minute", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> surfaces = new List<Brep>();
            double speed = 3;
            double hour = 0;

            if (!DA.GetDataList(0, surfaces)) return;
            if (!DA.GetData(1, ref speed)) return;
            if (!DA.GetData(2, ref hour)) return;


            // hard coded for now 
            // average data based on: https://www.mdpi.com/2071-1050/12/19/7863/htm
            // would be interesting to add inputs like retail use etc and model based on that
            int[] activityPerHour = new int[]
            {
                300, 200, 100, 50, 50, 300,
                1000, 1200, 1500, 1700, 1500,
                1200, 2000, 2200, 2200, 2200,
                1600, 1700, 1800, 2000, 1300,
                1000, 900, 800
            };

            var hourActivity = activityPerHour[(int)hour];

            List<double> peopleEstimatePerHour = new List<double>();
            foreach (Brep brep in surfaces)
            {
                // distance extracted from brep edge
                var walkingMilesPerBrep = GetWalkingDistanceForBrep(brep);

                // how long doest it take to walk this distance? 
                var timeToWalk = walkingMilesPerBrep / speed;

                // every person during this hour will take the time to walk it
                // calculate how many people on average is at once during this hour
                var peoplePerHour = Math.Round(timeToWalk * hourActivity);
                peopleEstimatePerHour.Add(peoplePerHour);
            }


            DA.SetDataList(0, peopleEstimatePerHour);
        }

        double GetWalkingDistanceForBrep(Brep brep)
        {
            var edges = Utils.GetBrepJoinedEdges(brep);

            double edgeTotalLength = 0;

            for (int i = 0; i < edges.Count; i++)
            {
                edgeTotalLength += edges[i].GetLength();
            }

            return Utils.FeetToMiles(edgeTotalLength / 2.0);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.PeopleEstimate;

        public override Guid ComponentGuid => new Guid("A1D448CB-C30A-44C8-BBAE-234046A1BCA7");

    }
}