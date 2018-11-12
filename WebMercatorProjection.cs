using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using GH_IO;

namespace WebMercatorProjection
{
    public class WebMercatorProjection : GH_Component
    {

        public WebMercatorProjection() : base( "WebMercator", "WebMercatorProjection", "This component converts latitudes and longitudes into a WebMercator projection", "THR34D5Workshop", "Projections")
        {



        }

        public override Guid ComponentGuid
        {

            get { return new Guid("AD1B6F80-A3AA-4BC7-A921-CA0FC6CBBC3B"); }

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Latitude", "Lat", "Input here the latitudes", GH_ParamAccess.list);
            pManager.AddNumberParameter("Longitude", "Lon", "Input here the longitudes", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("WebMercator Projection", "WbMctrPrj", "Transformed latitudes and longitudes to points on a WebMercator 2D projection", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var lats = new List<double>();
            var longs = new List<double>();

            DA.GetDataList(0, lats);
            DA.GetDataList(1, longs);

            var points = new List<Point3d>();

            for (int i = 0; i < lats.Count; ++i)
            {

                lats[i] = Rhino.RhinoMath.ToRadians(lats[i]);
                longs[i] = Rhino.RhinoMath.ToRadians(longs[i]);

                points.Add(MercatorProjection(lats[i], longs[i]));

            }

            DA.SetDataList(0, points);

        }

        public Point3d MercatorProjection(double lat, double lon)
        {

            var earthRadius = 6367;

            double FSin = Math.Sin(lon);

            var x = lat * earthRadius;
            var y = (earthRadius * 0.5) * Math.Log((1.0 + FSin) / (1.0 - FSin));

            return new Point3d(x, y, 0);

        }

    }

}
