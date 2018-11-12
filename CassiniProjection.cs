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

namespace CassiniProjection
{
    public class CassiniProjection : GH_Component
    {

        public CassiniProjection() : base("Cassini", "CassiniProjection", "This component converts latitudes and longitudes into a Cassini projection", "THR34D5Workshop", "Projections")
        {



        }

        public override Guid ComponentGuid
        {

            get { return new Guid("480A77D6-6706-4D90-9127-D653432B0E6B"); }

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Latitude", "Lat", "Input here the latitudes", GH_ParamAccess.list);
            pManager.AddNumberParameter("Longitude", "Lon", "Input here the longitudes", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("Cassini Projection", "CassPrj", "Transformed latitudes and longitudes to a Cassini 2D projection", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var lats = new List<double>();
            var longs = new List<double>();

            DA.GetDataList(1, lats);
            DA.GetDataList(0, longs);

            var points = new List<Point3d>();

            for (int i = 0; i < lats.Count; ++i)
            {

                lats[i] = Rhino.RhinoMath.ToRadians(lats[i]);
                longs[i] = Rhino.RhinoMath.ToRadians(longs[i]);

                points.Add(CassiniProj(lats[i], longs[i]));

            }

            DA.SetDataList(0, points);

        }

        public Point3d CassiniProj(double lat, double lon)
        {

            var earthRadius = 6367;

            var x = earthRadius * Math.Asin(Math.Cos(lat) * Math.Sin(lon));
            var y = earthRadius * Math.Atan2(Math.Sin(lat), Math.Cos(lat) * Math.Cos(lon));
            var z = earthRadius * Math.Sin(lat);
            return new Point3d(x, y, 0);

        }

    }

}