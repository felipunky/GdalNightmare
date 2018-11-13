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

namespace SphericalProjection
{
    public class SphericalProjection : GH_Component
    {

        public SphericalProjection() : base( "Spherical", "SphericalProjection", "This component converts latitudes and longitudes into a sphere projection", "THR34D5Workshop", "Projections" )
        {



        }

        public override Guid ComponentGuid
        {

            get { return new Guid( "1535A641-B8F5-489A-BF1F-22D0B80DBF73" ); }

        }

        protected override void RegisterInputParams( GH_InputParamManager pManager )
        {

            pManager.AddNumberParameter( "Latitude", "Lat", "Input here the latitudes", GH_ParamAccess.list );
            pManager.AddNumberParameter( "Longitude", "Lon", "Input here the longitudes", GH_ParamAccess.list );

        }

        protected override void RegisterOutputParams( GH_OutputParamManager pManager )
        {

            pManager.AddPointParameter( "Spherical Projection", "SphrPrj", "Transformed latitudes and longitudes to points on a sphere", GH_ParamAccess.list );

        }

        protected override void SolveInstance( IGH_DataAccess DA )
        {

            var lats = new List<double>();
            var longs = new List<double>();

            DA.GetDataList( 0, lats );
            DA.GetDataList( 1, longs );

            var points = new List<Point3d>();

            for( int i = 0; i < lats.Count; ++i )
            {

                lats[i] = Rhino.RhinoMath.ToRadians( lats[i] );
                longs[i] = Rhino.RhinoMath.ToRadians( longs[i] );

                points.Add( convertSphericalToCartesian( lats[i], longs[i] ) );

            }

            DA.SetDataList( 0, points );

        }

        public Point3d convertSphericalToCartesian( double lat, double lon )
        {

            var earthRadius = 6367;

            var x = earthRadius * Math.Cos( lon ) * Math.Cos( lat );
            var y = earthRadius * Math.Cos( lon ) * Math.Sin( lat );
            var z = earthRadius * Math.Sin( lon );
            return new Point3d( x, y, z );

        }

    }

}
