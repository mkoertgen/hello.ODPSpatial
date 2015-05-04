using System.Data;
using System;
using System.Diagnostics;
using NUnit.Framework;
using Oracle.DataAccess.Client;

namespace ODPSpatial
{
    [TestFixture]
    public class SdoGeometryTests
    {
        // TODO: adjust connection string
        private const string ConnStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE))); User Id=scott; Password=scott;";

        [Test]
        public void TestGeometry()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                Trace.TraceInformation("Connected to Oracle: " + conn.ServerVersion);

                // TODO: adjust query
                const string query = "select * from MyShapes.MyShape";
                using (var command = new OracleCommand(query, conn))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.HasRows);
                        Trace.TraceInformation("Executed query: \"{0}\".", query);

                        // find SdoGeometry
                        var types = new Type[reader.FieldCount];
                        var shapeId = -1;
                        for (int i = 0; i < types.Length; i++)
                        {
                            types[i] = reader.GetFieldType(i);
                            if (shapeId == -1 && (types[i] == typeof(SdoGeometry)))
                            {
                                shapeId = i;
                            }
                        }

                        Assert.IsTrue(shapeId != -1, "No geometry column found.");
                        Trace.TraceInformation("Found SdoGeometry in column \"{1}\" ({0})", shapeId, reader.GetName(shapeId));

                        var values = new object[reader.FieldCount];
                        var iRow = 0;
                        while (reader.Read())
                        {
                            reader.GetValues(values);

                            var g = values[shapeId] as SdoGeometry;
                            if (g != null)
                            {
                                Trace.TraceInformation("Row {0}: geometry={{Dim={1}, LRSDim={2}, SdoGType={3}, #V={4}}}", 
                                    iRow, g.Dim, g.LrsDim, g.GTypeAsEnum, g.Ordinates.Length);
                            }
                            else
                                Trace.TraceInformation("Row {0}: null geometry.", iRow);

                            iRow++;
                        }
                    }
                }
            }
        }
    }
}
