using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SocialDistancingForSidewalks
{
    public class SurfaceCentrelineComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SurfaceCentrelineComponent()
          : base("SurfaceCentrelines", "SrfCentrLn",
            "Tries to find centelines for surface",
            "SocialDistancingForSidewalks", "Evaluate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Srf", "Surface", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Centrelines", "CntrLn", "Centelines for provided surfaces.", GH_ParamAccess.tree);
            pManager.AddPointParameter("SamplePoints", "SamplPt", "Points that has calculated weight", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Width", "", "", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> surfaces = new List<Brep>();
 
            if (!DA.GetDataList(0, surfaces)) return;

            List<List<Line>> centrelinesForSurfaces = new List<List<Line>>();
            List<List<double>> measurementLinesLengths = new List<List<double>>();
            List<List<Point3d>> samplePoints = new List<List<Point3d>>();

            for (int i = 0; i < surfaces.Count; i++)
            {
                // Find centrelines
                var centrelines = surfaces[i].FindCentrelines();
                centrelinesForSurfaces.Add(centrelines);

               // Other elements: test points and calculated "width" for this test point
                var testPoints = new List<Point3d>();
                var lines = new List<Line>();
                var widths = new List<double>();

                // get edges
                var surfaceEdgeCurvesJoined = Utils.GetBrepJoinedEdges(surfaces[i]);
                var middlePoints = centrelines.Select(x => x.PointAt(0.5));

                foreach (Curve edge in surfaceEdgeCurvesJoined)
                {
                    foreach (Point3d p in middlePoints)
                    {
                        double parameter;
                        edge.ClosestPoint(p, out parameter);
                        var line = new Line(p, edge.PointAt(parameter));
                        lines.Add(line);

                        testPoints.Add(line.PointAt(0.5));

                        // Multiplication * 2 comes from assumption that line goes from centreline to edge
                        // therefore, the width of brep in this test point is twice this value
                        widths.Add(line.Length * 2);
                    }
                }
                measurementLinesLengths.Add(widths);
                samplePoints.Add(testPoints);
            }

            // Finally assign the spiral to the output parameter.
            DA.SetDataTree(0, Utils.ListOfListsToTree(centrelinesForSurfaces));
            DA.SetDataTree(1, Utils.ListOfListsToTree(samplePoints));
            DA.SetDataTree(2, Utils.ListOfListsToTree(measurementLinesLengths));
        }


        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Centreline;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("9579374E-86B4-42AD-BF35-EC7CD7847E02");
    }
}