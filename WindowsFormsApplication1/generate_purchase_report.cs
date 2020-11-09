using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    public partial class generate_purchase_report : Form
    {

        string j;
        int tot = 0;
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\inventorymanagementsystem\WindowsFormsApplication1\WindowsFormsApplication1\inventory.mdf;Integrated Security=True");


        public void get_value(string i)
        {
            j = i;
        }

        public generate_purchase_report()
        {
            InitializeComponent();
        }

        private void generate_purchase_report_Load(object sender, EventArgs e)
        {
            if (con.State==ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

            DataSet3 ds = new DataSet3();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = j;
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds.DataTable1);
            da.Fill(dt);
            tot = 0;
            foreach (DataRow dr in dt.Rows)
            {
                tot += Convert.ToInt32(dr["product_total"].ToString());
            }

            CrystalReport3 myreport = new CrystalReport3();
            myreport.SetDataSource(ds);
            myreport.SetParameterValue("total", tot.ToString());
            crystalReportViewer1.ReportSource = myreport;
        }
    }
}
