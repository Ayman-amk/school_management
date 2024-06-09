using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddStudent : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                GetStudents();
            }
        }

        private void LoadClasses()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT ClassId, ClassName FROM Class");
                ddlClass.DataSource = dt;
                ddlClass.DataTextField = "ClassName";
                ddlClass.DataValueField = "ClassId";
                ddlClass.DataBind();
                ddlClass.Items.Insert(0, new ListItem("--Select Class--", "0"));
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void GetStudents()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], s.StudentId, c.ClassName, s.Name, s.DOB, s.Gender, s.Mobile, s.RollNo, s.Address, s.created_at FROM Students s JOIN Class c ON s.ClassId = c.ClassId");
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
                int classId = int.Parse(ddlClass.SelectedValue);
                string name = txtName.Text.Trim();
                DateTime dob = DateTime.ParseExact(txtDOB.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string gender = ddlGender.SelectedValue;
                long mobile = long.Parse(txtMobile.Text.Trim());
                string rollNo = txtRollNo.Text.Trim();
                string address = txtAddress.Text.Trim();

                if (classId == 0 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gender) || mobile <= 0 || string.IsNullOrEmpty(rollNo) || string.IsNullOrEmpty(address))
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("SELECT * FROM Students WHERE RollNo = @RollNo AND ClassId = @ClassId", new SqlParameter("@RollNo", rollNo), new SqlParameter("@ClassId", classId));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO Students (ClassId, Name, DOB, Gender, Mobile, RollNo, Address, created_at) VALUES(@ClassId, @Name, @DOB, @Gender, @Mobile, @RollNo, @Address, GETDATE())";
                    fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@Name", name), new SqlParameter("@DOB", dob), new SqlParameter("@Gender", gender), new SqlParameter("@Mobile", mobile), new SqlParameter("@RollNo", rollNo), new SqlParameter("@Address", address));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetStudents();
                }
                else
                {
                    lblMsg.Text = "The student you're trying to add already exists!";
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
            GetStudents();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetStudents();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetStudents();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int studentId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                string name = (row.FindControl("txtNameEdit") as TextBox).Text;
                DateTime dob = DateTime.ParseExact((row.FindControl("txtDOBEdit") as TextBox).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string gender = (row.FindControl("ddlGenderEdit") as DropDownList).SelectedValue;
                long mobile = long.Parse((row.FindControl("txtMobileEdit") as TextBox).Text);
                string rollNo = (row.FindControl("txtRollNoEdit") as TextBox).Text;
                string address = (row.FindControl("txtAddressEdit") as TextBox).Text;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gender) || mobile <= 0 || string.IsNullOrEmpty(rollNo) || string.IsNullOrEmpty(address))
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Students SET Name = @Name, DOB = @DOB, Gender = @Gender, Mobile = @Mobile, RollNo = @RollNo, Address = @Address WHERE StudentId = @StudentId";
                fn.Query(query, new SqlParameter("@Name", name), new SqlParameter("@DOB", dob), new SqlParameter("@Gender", gender), new SqlParameter("@Mobile", mobile), new SqlParameter("@RollNo", rollNo), new SqlParameter("@Address", address), new SqlParameter("@StudentId", studentId));

                lblMsg.Text = "Student Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetStudents();
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
                int studentId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM Students WHERE StudentId = @StudentId";
                fn.Query(query, new SqlParameter("@StudentId", studentId));

                lblMsg.Text = "Student Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetStudents();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
