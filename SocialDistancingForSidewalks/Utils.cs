using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Geometry;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialDistancingForSidewalks
{
    public static class Utils
    {

        #region DATA/FORMAT
        public static double FeetToMiles(double feet)
        {
            return feet * 0.000189394;
        }


        public static DataTree<T> ListOfListsToTree<T>(List<List<T>> list)
        {
            DataTree<T> tree = new DataTree<T>();
            int i = 0;
            foreach (List<T> innerList in list)
            {
                tree.AddRange(innerList, new GH_Path(new int[] { 0, i }));
                i++;
            }
            return tree;
        }
        #endregion

        #region GEOMETRY

        public static List<Curve> GetBrepJoinedEdges(Brep b)
        {
            List<Curve> edges = new List<Curve>();
            foreach (BrepLoop edge in b.Loops)
            {
                edges.Add(edge.To3dCurve());
            }

            var joinedEdges = Curve.JoinCurves(edges).ToList();

            return joinedEdges;
        }

        public static double ClosestPointDistance(Point3d point, List<Point3d> cloud)
        {
            Rhino.Geometry.PointCloud pCloud = new Rhino.Geometry.PointCloud(cloud);
            var closestPointIndex = pCloud.ClosestPoint(point);
            return point.DistanceTo(pCloud.PointAt(closestPointIndex));
        }

        public static double FindDistanceToClosestBrepPoint(Brep brep, Point3d point, bool mapToZero)
        {
            Point3d closestPoint = brep.ClosestPoint(point);

            var height = mapToZero ? 0 : point.Z;

            Point3d closestPointMapped = new Point3d(closestPoint.X, closestPoint.Y, height);

            var distance = point.DistanceTo(closestPointMapped);

            return distance;
        }
        #endregion

        // The logic of function follows the workflow idea from here: https://discourse.mcneel.com/t/extract-centreline-of-polylines/85133/15
        public static List<Line> FindCentrelines(this Brep brep)
        {
            List<Line> centreLines = new List<Line>();

            // get edges
            var surfaceEdgeCurvesJoined = Utils.GetBrepJoinedEdges(brep);

            List<Point3d> allPoints = new List<Point3d>();
            // for edges separately
            foreach (Curve edge in surfaceEdgeCurvesJoined)
            {
                allPoints.AddRange(DivideByLengthAndGetPoints(edge, 5));
            }

            // Find height to move the lines (the default Voronoi will be placed on Plane 0,0,0)
            var averageHeight = allPoints.Select(p => p.Z).Average();
            var translation = Transform.Translation(new Vector3d(0, 0, averageHeight));

            // Voronoi from points
            var polylines = GetVoronoiPolylinesFromPoints(allPoints);

            // Get all start and end points from polylines for this surface
            foreach (Polyline p in polylines)
            {
                var exploded = p.GetSegments();

                foreach (Line line in exploded)
                {
                    var distanceStart = FindDistanceToClosestBrepPoint(brep, line.PointAt(0), true);
                    var distanceEnd = FindDistanceToClosestBrepPoint(brep, line.PointAt(1), true);

                    // Check if both ends of line were already on the given brep
                    if (distanceStart + distanceEnd < 0.1)
                    {
                        line.Transform(translation);
                        centreLines.Add(line);
                    }
                }
            }
            return centreLines;
        }

        private static List<Point3d> DivideByLengthAndGetPoints(Curve c, double distance)
        {
            // Divide surface edges by distance
            var parameters = c.DivideByLength(distance, false);
            var points = parameters.Select(param => c.PointAt(param)).ToList();
            return points;
        }

        // This function copied from here: https://discourse.mcneel.com/t/voronoi-c/91379/5
        private static List<Polyline> GetVoronoiPolylinesFromPoints(List<Point3d> points)
        {
            // Create a boundingbox and get its corners
            BoundingBox bb = new BoundingBox(points);
            Vector3d diagonal = bb.Diagonal;
            double diagonalLength = diagonal.Length;
            double f = diagonalLength / 15;
            bb.Inflate(f, f, f);
            Point3d[] bbCorners = bb.GetCorners();

            // Create a list of nodes
            Node2List nodes = new Node2List();
            foreach (Point3d p in points)
            {
                Node2 n = new Node2(p.X, p.Y);
                nodes.Append(n);
            }

            // Create a list of outline nodes using the BB
            Node2List outline = new Node2List();
            foreach (Point3d p in bbCorners)
            {
                Node2 n = new Node2(p.X, p.Y);
                outline.Append(n);
            }

            // Calculate the delaunay triangulation
            var delaunay = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Connectivity(nodes, 0.1, false);

            // Calculate the voronoi diagram
            var voronoi = Grasshopper.Kernel.Geometry.Voronoi.Solver.Solve_Connectivity(nodes, delaunay, outline);

            // Get polylines from the voronoi cells and return them to GH
            List<Polyline> polylines = new List<Polyline>();
            foreach (var c in voronoi)
            {
                Polyline pl = c.ToPolyline();
                polylines.Add(pl);
            }
            return polylines;
        }
    }

    
}
