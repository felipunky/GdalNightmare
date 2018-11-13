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

        public GdalNightmare() : base("Read information from grib files", "ReadGribFiles", "This component converts Grib files into Grasshopper data", "THR34D5Workshop", "ExtractData")
        {



        }

        public override Guid ComponentGuid
        {

            get { return new Guid("82D5C9EB-3CC9-4671-8AD4-7D8E1658C044"); }

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddTextParameter( "PathToFile", "Path", "Path to directory where the grib2 file resides", GH_ParamAccess.item );
            pManager.AddIntegerParameter( "BandToSelect", "BandNumber", "Pick which band of the file you want to read, If you don't know just use 1", GH_ParamAccess.item );

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter( "CatchErrors", "ERR", "Tell if there is an error while loading the libraries", GH_ParamAccess.item );
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

            int pickBand = 1;

            DA.GetData( 1, ref pickBand );

            OSGeo.GDAL.Dataset ds = OSGeo.GDAL.Gdal.Open( file, OSGeo.GDAL.Access.GA_ReadOnly );

            var band = ds.GetRasterBand( pickBand );

            var bandXSize = band.XSize;
            var bandYSize = band.YSize;

            var data = new float[bandXSize * bandYSize];
            band.ReadRaster(0, 0, bandXSize, bandYSize, data, bandXSize, bandYSize, 0, 0);

            ds.GetMetadata( output );

            ds = null;

            DA.SetData( 0, output );
            DA.SetDataList( 1, data );

        }

    }

}