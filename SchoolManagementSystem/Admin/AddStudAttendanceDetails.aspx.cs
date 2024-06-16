using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddStudAttendanceDetails : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                LoadSubjects();
                GetAttendanceDetails();
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

        private void LoadSubjects()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT SubjectId, SubjectName FROM Subjects");
                ddlSubject.DataSource = dt;
                ddlSubject.DataTextField = "SubjectName";
                ddlSubject.DataValueField = "SubjectId";
                ddlSubject.DataBind();
                ddlSubject.Items.Insert(0, new ListItem("--Select Subject--", "0"));
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void GetAttendanceDetails()
        {
            try
            {
                DataTable dt = fn.Fetch(@"SELECT 
                    Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], 
                    sa.Id, 
                    c.ClassName, 
                    s.SubjectName, 
                    sa.RollNo, 
                    CASE sa.Status WHEN 1 THEN 'Present' ELSE 'Absent' END AS Status, 
                    sa.AttendanceDate, 
                    sa.created_at 
                FROM 
                    StudentAttendance sa 
                JOIN 
                    Class c ON sa.ClassId = c.ClassId 
                JOIN 
                    Subjects s ON sa.SubjectId = s.SubjectId");

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
                int subjectId = int.Parse(ddlSubject.SelectedValue);
                string rollNo = txtRollNo.Text.Trim();
                bool status = bool.Parse(ddlStatus.SelectedValue);
                DateTime attendanceDate = DateTime.ParseExact(txtAttendanceDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (classId == 0 || subjectId == 0 || string.IsNullOrEmpty(rollNo))
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "INSERT INTO StudentAttendance (ClassId, SubjectId, RollNo, Status, AttendanceDate, created_at) VALUES(@ClassId, @SubjectId, @RollNo, @Status, @AttendanceDate, GETDATE())";
                fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@RollNo", rollNo), new SqlParameter("@Status", status), new SqlParameter("@AttendanceDate", attendanceDate));

                lblMsg.Text = "Inserted Successfully !!";
                lblMsg.CssClass = "alert alert-success";

                GetAttendanceDetails();
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
            GetAttendanceDetails();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetAttendanceDetails();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetAttendanceDetails();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                string rollNo = (row.FindControl("txtRollNoEdit") as TextBox).Text;
                bool status = bool.Parse((row.FindControl("ddlStatusEdit") as DropDownList).SelectedValue);
                DateTime attendanceDate = DateTime.ParseExact((row.FindControl("txtAttendanceDateEdit") as TextBox).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (string.IsNullOrEmpty(rollNo))
                {
                    lblMsg.Text = "Roll number is required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE StudentAttendance SET RollNo = @RollNo, Status = @Status, AttendanceDate = @AttendanceDate WHERE Id = @Id";
                fn.Query(query, new SqlParameter("@RollNo", rollNo), new SqlParameter("@Status", status), new SqlParameter("@AttendanceDate", attendanceDate), new SqlParameter("@Id", id));

                lblMsg.Text = "Attendance Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetAttendanceDetails();
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
                int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM StudentAttendance WHERE Id = @Id";
                fn.Query(query, new SqlParameter("@Id", id));

                lblMsg.Text = "Attendance Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetAttendanceDetails();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
