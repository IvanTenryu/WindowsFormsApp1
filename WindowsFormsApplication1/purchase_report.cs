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
    public partial class purchase_report : Form
    {

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan Valencia\source\repos\WindowsFormsApplication1\WindowsFormsApplication1\inventory.mdf;Integrated Security=True");
        string query = "";
        public purchase_report()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = 0;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from purchase_master";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            foreach(DataRow dr in dt.Rows)
            {
                i += Convert.ToInt32(dr["Total_de_producto"].ToString());

            }


            label3.Text = i.ToString();

            query = "select * from purchase_master";

        }

        private void purchase_report_Load(object sender, EventArgs e)
        {
            if(con.State==ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string startdate;
            string enddate;

            startdate = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            enddate= dateTimePicker2.Value.ToString("dd/MM/yyyy");

            int i = 0;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from purchase_master where Fecha_de_entrada>='" + startdate.ToString() + 
                "' AND Fecha_de_entrada<='" + enddate.ToString() +"'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                i += Convert.ToInt32(dr["Total_de_producto"].ToString());

            }


            label3.Text = i.ToString();
            query = "select * from purchase_master where Fecha_de_entrada>='" + startdate.ToString() + 
                "' AND Fecha_de_entrada<='" + enddate.ToString() + "'";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            generate_purchase_report gpr = new generate_purchase_report();
            gpr.get_value(query.ToString());
            gpr.Show();
        }
    }
}
