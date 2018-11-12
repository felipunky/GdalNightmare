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
using Acc = Accord.Math;

namespace GdalNightmare
{

    public class GdalNightmare : GH_Component
    {

        public GdalNightmare() : base( "ReadGribFiles", "Read information from grib files", "This component converts Grib files into Grasshopper data", "THR34D5Workshop", "ExtractData")
        {



        }

        public override Guid ComponentGuid
        {

            get { return new Guid("82D5C9EB-3CC9-4671-8AD4-7D8E1658C044"); }

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddTextParameter( "PathToFile", "Path", "Path to directory where the grib2 file resides", GH_ParamAccess.item );

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter( "CatchErrors", "ERR", "Tell if there is an error while loading the libraries", GH_ParamAccess.item );
            pManager.AddNumberParameter( "Latitude", "Lat", "Creates the latitudes from the grib2 file", GH_ParamAccess.list );
            pManager.AddNumberParameter( "Longitude", "Lon", "Creates the longitudes from the grib2 file", GH_ParamAccess.list );
            pManager.AddNumberParameter( "DataSet", "DS", "Extracts the data from the grib2 file", GH_ParamAccess.list );

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string output = "";

            try
            {

                GdalConfiguration.ConfigureOgr();

                GdalConfiguration.ConfigureGdal();

                output = "It works!";

            }

            catch (Exception e)
            {

                output = "{0} Exception caught. " + e;

            }

            string input = "";

            DA.GetData(0, ref input);

            string file = input;

            OSGeo.GDAL.Dataset ds = OSGeo.GDAL.Gdal.Open( file, OSGeo.GDAL.Access.GA_ReadOnly );

            double[] gt = new double[6];

            ds.GetGeoTransform( gt );

            var xres = gt[1];
            var yres = gt[5];

            var xsize = ds.RasterXSize;
            var ysize = ds.RasterYSize;

            var xmin = gt[0] + xres * 0.5;
            var xmax = gt[0] + (xres * xsize) - xres * 0.5;
            var ymin = gt[3] + (yres * ysize) + yres * 0.5;
            var ymax = gt[3] - yres * 0.5;

            var xx = EnumerableUtilities.RangePython(xmin, xmax + xres, xres);
            var yy = EnumerableUtilities.RangePython(ymax + yres, ymin, yres);

            var M = Acc.Matrix.MeshGrid( yy.ToArray(), xx.ToArray() );

            var y = M.Item1;
            var x = M.Item2;

            var band = ds.GetRasterBand(1);

            var bandXSize = band.XSize;
            var bandYSize = band.YSize;

            var data = new float[bandXSize * bandYSize];
            band.ReadRaster(0, 0, bandXSize, bandYSize, data, bandXSize, bandYSize, 0, 0);

            ds.GetMetadata( output );

            ds = null;

            DA.SetData( 0, output );
            DA.SetDataList( 1, x );
            DA.SetDataList( 2, y );
            DA.SetDataList( 3, data );

        }

    }

}