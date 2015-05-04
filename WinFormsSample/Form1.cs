using System;
using System.Data;
using System.Windows.Forms;
using ODPSpatial;
using Oracle.DataAccess.Client;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void AppendLog(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }

        private void BtnQueryClick(object sender, EventArgs e)
        {
            try
            {
                // Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)
                //(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = xe)));
                //User Id = scott; Password = scott;

                // Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = XE))); User Id = scott; Password = scott;
                var connStr = txtConn.Text;
                using (var conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    AppendLog("Connected to Oracle: " + conn.ServerVersion);

                    var qryStr = txtSQL.Text;
                    using (var command = new OracleCommand(qryStr, conn))
                    {
                        command.CommandType = CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Columns.Clear();
                                for (int i = 0; i < reader.FieldCount; i++)
                                    dataGridView1.Columns.Add(reader.GetName(i), reader.GetName(i));
                                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;

                                while (reader.Read())
                                {
                                    var values = new object[dataGridView1.ColumnCount];
                                    reader.GetValues(values);
                                    dataGridView1.Rows.Add(values);

                                    int shapeId = -1;
                                    //int pointId = -1;
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        if (values[i] is SdoGeometry)
                                        {
                                            shapeId = i;
                                            break;
                                        }
                                        //if (values[i] is SdoPoint)
                                        //{
                                        //    pointId = i;
                                        //}
                                    }

                                    if (shapeId != -1)
                                    {
                                        AppendLog(
                                            String.Format("Found SdoGeometry in column \"{1}\" ({0})",
                                            shapeId, reader.GetName(shapeId))
                                            );
                                    }
                                }
                            }
                            else
                                MessageBox.Show("No Results", "Info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                AppendLog(ex.Message);
            }
        }

        private void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            propertyGrid1.SelectedObject = dataGridView1.CurrentCell.Value;
            propertyGrid1.Refresh();
        }
    }
}
