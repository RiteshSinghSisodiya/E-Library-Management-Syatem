using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class userSignup : System.Web.UI.Page
    {

        string strcon = ConfigurationManager.ConnectionStrings["con"].ConnectionString;



        protected void Page_Load(object sender, EventArgs e)
        {

        }


        // signup button click event
        protected void Button1_Click(object sender, EventArgs e)
        {
            
            DateTime dob;
            if (!DateTime.TryParse(TextBox2.Text.Trim(), out dob))
            {
                Response.Write("<script>alert('Invalid date of birth.');</script>");
                return;
            }

            int age = DateTime.Now.Year - dob.Year;
            int m = DateTime.Now.Month - dob.Month;
            if (m < 0 || (m == 0 && DateTime.Now.Day < dob.Day))
            {
                age--;
            }

            if (age < 10)
            {
                Response.Write("<script>alert('You must be at least 10 years old.');</script>");
                return;
            }


            if (TextBox3.Text.Trim().Length != 10 || !TextBox3.Text.Trim().All(char.IsDigit))
            {
                Response.Write("<script>alert('Contact number must be 10 digits.');</script>");
                return;
            }

            if (!TextBox4.Text.Trim().EndsWith("@gmail.com"))
            {
                Response.Write("<script>alert('Email must end with @gmail.com.');</script>");
                return;
            }

            
            if (!(TextBox8.Text.Trim().StartsWith("M") || TextBox8.Text.Trim().StartsWith("m")))
            {
                Response.Write("<script>alert('Member ID must start with M or m.');</script>");
                return;
            }

            
            string password = TextBox9.Text.Trim();
            if (password.Length < 6 || !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                Response.Write("<script>alert('Password must be at least 6 characters long and contain at least one special character.');</script>");
                return;
            }

            
            if (checkMemberExists())
            {
                Response.Write("<script>alert('Member ID already exists, try another ID.');</script>");
            }
            else
            {
                signUpNewMember();
            }
        }



        // User defined method


        bool checkMemberExists()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("SELECT * from member_master_tbl where member_id='"+TextBox8.Text.Trim()+"';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if(dt.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                
                con.Close();

                Response.Write("<script>alert('Sign Up Successful. Go to User Login to Login');</script>");


            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
                return false;
            }

            
        }



        void signUpNewMember()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("INSERT INTO member_master_tbl(full_name, dob ,contact_no ,email ,state ,city ,pincode ,full_address ,member_id ,password ,account_status) values(@full_name, @dob ,@contact_no ,@email ,@state ,@city ,@pincode ,@full_address ,@member_id ,@password ,@account_status)", con);

                cmd.Parameters.AddWithValue("@full_name", TextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@dob", TextBox2.Text.Trim());
                cmd.Parameters.AddWithValue("@contact_no", TextBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@email", TextBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@state", DropDownList1.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@city", TextBox6.Text.Trim());
                cmd.Parameters.AddWithValue("@pincode", TextBox7.Text.Trim());
                cmd.Parameters.AddWithValue("@full_address", TextBox5.Text.Trim());
                cmd.Parameters.AddWithValue("@member_id", TextBox8.Text.Trim());
                cmd.Parameters.AddWithValue("@password", TextBox9.Text.Trim());
                cmd.Parameters.AddWithValue("@account_status", "pending");

                cmd.ExecuteNonQuery();
                con.Close();

                Response.Write("<script>alert('Sign Up Successful. Go to User Login to Login');</script>");


            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

    }
}