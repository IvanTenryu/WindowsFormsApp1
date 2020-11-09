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
    public partial class sales : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan Valencia\source\repos\WindowsFormsApplication1\WindowsFormsApplication1\inventory.mdf;Integrated Security=True");
        DataTable dt = new DataTable();
        int tot = 0;
        int last_bill_id = 0;
        public sales()
        {
            InitializeComponent();
        }

        private void sales_Load(object sender, EventArgs e)
        {
            if(con.State==ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

            dt.Clear();
            dt.Columns.Add("product");
            dt.Columns.Add("price");
            dt.Columns.Add("qty");
            dt.Columns.Add("total");
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            listBox1.Visible = true;

            listBox1.Items.Clear();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from stock where Nombre_del_producto like('"+ textBox3.Text +"%')";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            foreach(DataRow dr in dt.Rows)
            {
                listBox1.Items.Add(dr["Nombre_del_producto"].ToString());
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Down)
            {
                listBox1.Focus();
                listBox1.SelectedIndex = 0;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode==Keys.Down)
                {
                    this.listBox1.SelectedIndex += 1;

                }
                if (e.KeyCode == Keys.Up)
                {
                    this.listBox1.SelectedIndex -= 1;
                }
                if(e.KeyCode==Keys.Enter)
                {
                    textBox3.Text = listBox1.SelectedItem.ToString();
                    listBox1.Visible = false;
                    textBox4.Focus();
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select top 1 * from purchase_master where Nombre_del_producto='" + textBox3.Text +"' order by id desc";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            foreach(DataRow dr in dt.Rows)
            {
                textBox4.Text = dr["Precio_del_producto"].ToString();
            }
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            try
            {
                textBox6.Text = Convert.ToString(Convert.ToInt32(textBox4.Text) * Convert.ToInt32(textBox5.Text)); 
            }
            catch(Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int stock = 0;
            SqlCommand cmd1 = con.CreateCommand();
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "select * from stock where Nombre_del_producto='" + textBox3.Text +"'";
            cmd1.ExecuteNonQuery();
            DataTable dt1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            da1.Fill(dt1);
            foreach(DataRow dr1 in dt1.Rows)
            {
                stock = Convert.ToInt32(dr1["Cantidad_de_producto"].ToString());
            }


            if(Convert.ToInt32(textBox5.Text)>stock)
            {
                MessageBox.Show("¡CANTIDAD INVÁLIDA!");
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr["Producto"] = textBox3.Text;
                dr["Precio"] = textBox4.Text;
                dr["Cantidad"] = textBox5.Text;
                dr["Total"] = textBox6.Text;
                dt.Rows.Add(dr);
                dataGridView1.DataSource = dt;

                tot += Convert.ToInt32(dr["Total"].ToString());

                label10.Text = tot.ToString();


            }

            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";



        }

#pragma warning disable IDE1006 // Estilos de nombres
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Estilos de nombres
        {
            try
            {
                tot = 0;
                dt.Rows.RemoveAt(Convert.ToInt32(dataGridView1.CurrentCell.RowIndex.ToString()));
                foreach(DataRow dr1 in dt.Rows)
                {
                    tot += Convert.ToInt32(dr1["Total"].ToString());
                    label10.Text = tot.ToString();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string orderid = "";
            SqlCommand cmd1 = con.CreateCommand();
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "insert into order_user values('"+ textBox1.Text +"','"+ textBox2.Text +"','"+ comboBox1.Text +"','"+ dateTimePicker1.Value.ToString("dd/MM/yyyy") +"')";
            cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 * from order_user order by id desc";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);
            foreach(DataRow dr2 in dt2.Rows)
            {
                orderid = dr2["id"].ToString();
            }



            foreach(DataRow dr in dt.Rows)
            {
                int qty;
                string pname;

                SqlCommand cmd3 = con.CreateCommand();
                cmd3.CommandType = CommandType.Text;
                cmd3.CommandText = "insert into order_item values('" + orderid.ToString() + "','" + 
                    dr["product"].ToString() + "','" + 
                    dr["price"].ToString() + "','" + 
                    dr["qty"].ToString() + "','"+ 
                    dr["total"].ToString() +"')";
                cmd3.ExecuteNonQuery();


                qty = Convert.ToInt32(dr["Cantidad"].ToString());
                pname = dr["Producto"].ToString();


                SqlCommand cmd6 = con.CreateCommand();
                cmd6.CommandType = CommandType.Text;
                cmd6.CommandText = "update stock set Cantidad_del_producto = Cantidad_del_producto-" + qty +
                    " where Nombre_del_producto='"+ pname.ToString() +"'";
                cmd6.ExecuteNonQuery();



            }


            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            label10.Text = "";

            dt.Clear();
            dataGridView1.DataSource = dt;

            MessageBox.Show("¡REGISTRO AGREGADO EXITOSAMENTE!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string orderid = "";
            SqlCommand cmd1 = con.CreateCommand();
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "insert into order_user values('" + textBox1.Text + "','" + 
                textBox2.Text + "','" + 
                comboBox1.Text + "','" + 
                dateTimePicker1.Value.ToString("dd/MM/yyyy") + "')";
            cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 * from order_user order by id desc";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);
            foreach (DataRow dr2 in dt2.Rows)
            {
                orderid = dr2["id"].ToString();
            }



            foreach (DataRow dr in dt.Rows)
            {
                int qty;
                string pname;

                SqlCommand cmd3 = con.CreateCommand();
                cmd3.CommandType = CommandType.Text;
                cmd3.CommandText = "insert into order_item values('" + orderid.ToString() + "','" + 
                    dr["Producto"].ToString() + "','" + 
                    dr["Precio"].ToString() + "','" + 
                    dr["Cantidad"].ToString() + "','" + 
                    dr["Total"].ToString() + "')";
                cmd3.ExecuteNonQuery();


                qty = Convert.ToInt32(dr["Cantidad"].ToString());
                pname = dr["Producto"].ToString();


                SqlCommand cmd6 = con.CreateCommand();
                cmd6.CommandType = CommandType.Text;
                cmd6.CommandText = "update stock set Cantidad_del_producto = Cantidad_del_producto-" + qty +
                    " where Nombre_del_producto='" + pname.ToString() + "'";
                cmd6.ExecuteNonQuery();



            }


            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            label10.Text = "";

            dt.Clear();
            dataGridView1.DataSource = dt;


            generate_bill1 gb1 = new generate_bill1();
            gb1.get_value(Convert.ToInt32(orderid.ToString()));
            gb1.Show();
        }
    }
}
