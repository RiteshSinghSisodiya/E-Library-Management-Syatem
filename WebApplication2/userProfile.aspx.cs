using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class userProfile : System.Web.UI.Page
    {
        string strcon = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["username"] == null || string.IsNullOrEmpty(Session["username"].ToString()))
                {
                    Response.Write("<script>alert('Session Expired. Please log in again.');</script>");
                    Response.Redirect("userLogin.aspx");
                }
                else
                {
                    getUserBookData();

                    if (!Page.IsPostBack)
                    {
                        getUserPersonalDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred. Please try again later.');</script>");
                Response.Redirect("userLogin.aspx");
            }
        }

        // Update button click
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Session["username"] == null || string.IsNullOrEmpty(Session["username"].ToString()))
            {
                Response.Write("<script>alert('Session Expired. Please log in again.');</script>");
                Response.Redirect("userLogin.aspx");
            }
            else
            {
                updateUserPersonalDetails(); // Call the update method
            }
        }

        // Update user personal details
        void updateUserPersonalDetails()
        {
            string password = string.IsNullOrEmpty(TextBox10.Text.Trim()) ? TextBox9.Text.Trim() : TextBox10.Text.Trim();

            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE member_master_tbl SET full_name=@full_name, dob=@dob, contact_no=@contact_no, email=@email, state=@state, city=@city, pincode=@pincode, full_address=@full_address, password=@password, account_status=@account_status WHERE member_id=@member_id", con))
                    {
                        cmd.Parameters.AddWithValue("@full_name", TextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@dob", TextBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact_no", TextBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", TextBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@state", DropDownList1.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@city", TextBox6.Text.Trim());
                        cmd.Parameters.AddWithValue("@pincode", TextBox7.Text.Trim());
                        cmd.Parameters.AddWithValue("@full_address", TextBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@account_status", "pending");
                        cmd.Parameters.AddWithValue("@member_id", Session["username"].ToString().Trim());

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Response.Write("<script>alert('Details Updated Successfully');</script>");
                            getUserPersonalDetails();
                            getUserBookData();
                        }
                        else
                        {
                            Response.Write("<script>alert('Invalid Entry');</script>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred: " + ex.Message + "');</script>");
            }
        }

        void getUserPersonalDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * from member_master_tbl WHERE member_id=@member_id", con))
                    {
                        cmd.Parameters.AddWithValue("@member_id", Session["username"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            TextBox1.Text = dt.Rows[0]["full_name"].ToString();
                            TextBox2.Text = dt.Rows[0]["dob"].ToString();
                            TextBox3.Text = dt.Rows[0]["contact_no"].ToString();
                            TextBox4.Text = dt.Rows[0]["email"].ToString();
                            DropDownList1.SelectedValue = dt.Rows[0]["state"].ToString().Trim();
                            TextBox6.Text = dt.Rows[0]["city"].ToString();
                            TextBox7.Text = dt.Rows[0]["pincode"].ToString();
                            TextBox5.Text = dt.Rows[0]["full_address"].ToString();
                            TextBox8.Text = dt.Rows[0]["member_id"].ToString();
                            TextBox9.Text = dt.Rows[0]["Password"].ToString();

                            Label1.Text = dt.Rows[0]["account_status"].ToString().Trim();

                            if (dt.Rows[0]["account_status"].ToString().Trim() == "Active")
                            {
                                Label1.Attributes.Add("class", "badge badge-pill badge-success");
                            }
                            else if (dt.Rows[0]["account_status"].ToString().Trim() == "pending")
                            {
                                Label1.Attributes.Add("class", "badge badge-pill badge-warning");
                            }
                            else if (dt.Rows[0]["account_status"].ToString().Trim() == "Deactivate")
                            {
                                Label1.Attributes.Add("class", "badge badge-pill badge-danger");
                            }
                            else
                            {
                                Label1.Attributes.Add("class", "badge badge-pill badge-info");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred: " + ex.Message + "');</script>");
            }
        }

        void getUserBookData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * from book_issue_tbl where member_id=@member_id", con))
                    {
                        cmd.Parameters.AddWithValue("@member_id", Session["username"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred: " + ex.Message + "');</script>");
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DateTime dt = Convert.ToDateTime(e.Row.Cells[5].Text);
                    DateTime today = DateTime.Today;

                    if (today > dt)
                    {
                        e.Row.BackColor = System.Drawing.Color.PaleVioletRed;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred: " + ex.Message + "');</script>");
            }
        }
    }
}
