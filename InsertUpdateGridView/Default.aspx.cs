using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace InsertUpdateGridView
{
    public partial class Default : System.Web.UI.Page
    {
        string connectionstring = @"Data Source=DEEPU\SQLEXPRESS01; Integrated Security=true; Initial Catalog=Demo";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGridView();
            }
        }

        private void PopulateGridView()
        {
            DataTable dtb1 = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select * from empdetails", con);
            
                sda.Fill(dtb1);
            }
            if (dtb1.Rows.Count > 0)
            {
                gvDetails.DataSource = dtb1;
                gvDetails.DataBind();
            }
            else
            {
                dtb1.Rows.Add(dtb1.NewRow());
                gvDetails.DataSource = dtb1;
                gvDetails.DataBind();
                gvDetails.Rows[0].Cells.Clear();
                gvDetails.Rows[0].Cells.Add(new TableCell());
                gvDetails.Rows[0].Cells[0].ColumnSpan = dtb1.Columns.Count;
                gvDetails.Rows[0].Cells[0].Text = "No Data Found";
                gvDetails.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void gvDetails_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDetails.EditIndex = e.NewEditIndex;
            PopulateGridView();
        }

        protected void gvDetails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDetails.EditIndex = -1;
            PopulateGridView();
        }

        protected void gvDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    string query = "Update empdetails set Name=@Name, City=@City, Country=@Country where Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", (gvDetails.Rows[e.RowIndex].FindControl("txtName") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@City", (gvDetails.Rows[e.RowIndex].FindControl("txtCity") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@Country", (gvDetails.Rows[e.RowIndex].FindControl("txtCountry") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(gvDetails.DataKeys[e.RowIndex].Value.ToString()));
                    cmd.ExecuteNonQuery();
                    gvDetails.EditIndex = -1;
                    PopulateGridView();
                    lblsuccessmsg.Text = "Updated Successfully";
                }
            }
            catch (SqlException ex)
            {
                lblErrormsg.Text = ex.Message;
            }
        }

        protected void gvDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    string query = "Delete from empdetails where Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    
                    cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(gvDetails.DataKeys[e.RowIndex].Value.ToString()));
                    cmd.ExecuteNonQuery();
                    gvDetails.EditIndex = -1;
                    PopulateGridView();
                    lblsuccessmsg.Text = "Deleted Successfully";
                }
            }
            catch (SqlException ex)
            {
                lblErrormsg.Text = ex.Message;
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            String Name;
            String City;
            String Country;
            string path = System.IO.Path.GetFileName(FileUpload1.FileName);
            path = path.Replace(" ", "");
            FileUpload1.SaveAs(Server.MapPath("~/ExcelFile/") + path);
            String ExcelPath = Server.MapPath("~/ExcelFile/") + path;
            OleDbConnection mycon = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + ExcelPath + "; Extended Properties=Excel 8.0; Persist Security Info = False");
            mycon.Open();
            OleDbCommand cmd = new OleDbCommand("select * from [Sheet2$]", mycon);
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                // Response.Write("<br/>"+dr[0].ToString());
                
                Name = dr[0].ToString();
                City = dr[1].ToString();
                Country = dr[2].ToString();
                savedata(Name, City, Country);


            }
            Label1.Text = "Data Has Been Saved Successfully";
        }

        private void savedata(String Name1, String City1, String Country1)
        {
            String query = "insert into empdetails(Name,City,Country) values('" + Name1 + "','" + City1 + "','" + Country1 + "')";
            
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
        }
    }
}