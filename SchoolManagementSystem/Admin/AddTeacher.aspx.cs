using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddTeacher : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTeachers();
            }
        }

        private void GetTeachers()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], TeacherId, Name, DOB, Gender, Mobile, Email, Address, Password, created_at FROM Teachers");
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                DateTime dob = DateTime.ParseExact(txtDOB.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string gender = ddlGender.SelectedValue;
                long mobile = long.Parse(txtMobile.Text.Trim());
                string email = txtEmail.Text.Trim();
                string address = txtAddress.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gender) || mobile <= 0 || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("SELECT * FROM Teachers WHERE Email = @Email", new SqlParameter("@Email", email));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO Teachers (Name, DOB, Gender, Mobile, Email, Address, Password, created_at) VALUES(@Name, @DOB, @Gender, @Mobile, @Email, @Address, @Password, GETDATE())";
                    fn.Query(query, new SqlParameter("@Name", name), new SqlParameter("@DOB", dob), new SqlParameter("@Gender", gender), new SqlParameter("@Mobile", mobile), new SqlParameter("@Email", email), new SqlParameter("@Address", address), new SqlParameter("@Password", password));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetTeachers();
                }
                else
                {
                    lblMsg.Text = "The teacher you're trying to add already exists!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GetTeachers();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetTeachers();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetTeachers();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int teacherId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                string name = (row.FindControl("txtNameEdit") as TextBox).Text;
                DateTime dob = DateTime.ParseExact((row.FindControl("txtDOBEdit") as TextBox).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string gender = (row.FindControl("ddlGenderEdit") as DropDownList).SelectedValue;
                long mobile = long.Parse((row.FindControl("txtMobileEdit") as TextBox).Text);
                string email = (row.FindControl("txtEmailEdit") as TextBox).Text;
                string address = (row.FindControl("txtAddressEdit") as TextBox).Text;
                string password = (row.FindControl("txtPasswordEdit") as TextBox).Text;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gender) || mobile <= 0 || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Teachers SET Name = @Name, DOB = @DOB, Gender = @Gender, Mobile = @Mobile, Email = @Email, Address = @Address, Password = @Password WHERE TeacherId = @TeacherId";
                fn.Query(query, new SqlParameter("@Name", name), new SqlParameter("@DOB", dob), new SqlParameter("@Gender", gender), new SqlParameter("@Mobile", mobile), new SqlParameter("@Email", email), new SqlParameter("@Address", address), new SqlParameter("@Password", password), new SqlParameter("@TeacherId", teacherId));

                lblMsg.Text = "Teacher Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetTeachers();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int teacherId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM Teachers WHERE TeacherId = @TeacherId";
                fn.Query(query, new SqlParameter("@TeacherId", teacherId));

                lblMsg.Text = "Teacher Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetTeachers();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
