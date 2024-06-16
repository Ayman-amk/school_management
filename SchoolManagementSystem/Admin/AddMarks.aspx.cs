using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddMarks : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                LoadSubjects();
                GetExamMarks();
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

        private void GetExamMarks()
        {
            try
            {
                DataTable dt = fn.Fetch(@"SELECT 
                    Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], 
                    e.ExamId, 
                    c.ClassName, 
                    s.SubjectName, 
                    e.RollNo, 
                    e.TotalMark, 
                    e.OutOfMark, 
                    e.created_at 
                FROM 
                    Exam e 
                JOIN 
                    Class c ON e.ClassId = c.ClassId 
                JOIN 
                    Subjects s ON e.SubjectId = s.SubjectId");

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
                int totalMark = int.Parse(txtTotalMark.Text.Trim());
                int outOfMark = int.Parse(txtOutOfMark.Text.Trim());

                if (classId == 0 || subjectId == 0 || string.IsNullOrEmpty(rollNo) || totalMark < 0 || outOfMark <= 0)
                {
                    lblMsg.Text = "All fields are required and marks must be positive.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "INSERT INTO Exam (ClassId, SubjectId, RollNo, TotalMark, OutOfMark, created_at) VALUES(@ClassId, @SubjectId, @RollNo, @TotalMark, @OutOfMark, GETDATE())";
                fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@RollNo", rollNo), new SqlParameter("@TotalMark", totalMark), new SqlParameter("@OutOfMark", outOfMark));

                lblMsg.Text = "Inserted Successfully !!";
                lblMsg.CssClass = "alert alert-success";

                GetExamMarks();
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
            GetExamMarks();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetExamMarks();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetExamMarks();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int examId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                string rollNo = (row.FindControl("txtRollNoEdit") as TextBox).Text;
                int totalMark = int.Parse((row.FindControl("txtTotalMarkEdit") as TextBox).Text);
                int outOfMark = int.Parse((row.FindControl("txtOutOfMarkEdit") as TextBox).Text);

                if (string.IsNullOrEmpty(rollNo) || totalMark < 0 || outOfMark <= 0)
                {
                    lblMsg.Text = "All fields are required and marks must be positive.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Exam SET RollNo = @RollNo, TotalMark = @TotalMark, OutOfMark = @OutOfMark WHERE ExamId = @ExamId";
                fn.Query(query, new SqlParameter("@RollNo", rollNo), new SqlParameter("@TotalMark", totalMark), new SqlParameter("@OutOfMark", outOfMark), new SqlParameter("@ExamId", examId));

                lblMsg.Text = "Marks Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetExamMarks();
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
                int examId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM Exam WHERE ExamId = @ExamId";
                fn.Query(query, new SqlParameter("@ExamId", examId));

                lblMsg.Text = "Marks Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetExamMarks();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
