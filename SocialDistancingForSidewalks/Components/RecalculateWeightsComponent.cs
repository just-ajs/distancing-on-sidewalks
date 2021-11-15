using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SocialDistancingForSidewalks.Components
{
    public class RecalculateWeightsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RecalculateWeightsComponent class.
        /// </summary>
        public RecalculateWeightsComponent()
          : base("RecalculateWeightsComponent", "CalcWght",
              "This component recalculates the values based on the test points distance to given point cloud",
              "SocialDistancingForSidewalks", "Evaluate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("TestPoints", "TstPts", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("TestPointsWeights", "TstPtsWgth", "", GH_ParamAccess.list);
            pManager.AddPointParameter("LocationPoints", "AtrPt", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistanceLimit", "DstLm", "", GH_ParamAccess.item, 1000);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Recalculated Weights", "", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Only new weights", "", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> testPoints = new List<Point3d>();
            List<double> testPointsWeights = new List<double>();
            List<Point3d> locationPoints = new List<Point3d>();
            double limit = double.NaN;

            if (!DA.GetDataList(0, testPoints)) return;
            if (!DA.GetDataList(1, testPointsWeights)) return;
            if (!DA.GetDataList(2, locationPoints)) return;
            DA.GetData(3, ref limit);

            // Find distance to attraction location points
            List<double> distances = new List<double>();
            for (int i = 0; i < testPoints.Count; i++)
            {
                var distance = Utils.ClosestPointDistance(testPoints[i], locationPoints);
                distances.Add(distance);
            }

            List<double> newWeights = new List<double>();
            List<double> onlyNewWeights = new List<double>();


            // If distance is within relevant radius, reweight the number proportionally
            for (int i = 0; i < testPointsWeights.Count; i++)
            { 
                if (distances[i] < limit)
                {
                    var proportion = distances[i] / limit;
                    onlyNewWeights.Add(proportion);

                    var weight = testPointsWeights[i] * proportion;
                    newWeights.Add(weight);

                }
                else
                {
                    newWeights.Add(testPointsWeights[i]);
                    onlyNewWeights.Add(1);
                }
            }

            
            DA.SetDataList(0, newWeights);
            DA.SetDataList(1, onlyNewWeights);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AttractionPoint;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("82E2456D-C733-483F-9EF6-5CA8022F5353"); }
        }
    }
}