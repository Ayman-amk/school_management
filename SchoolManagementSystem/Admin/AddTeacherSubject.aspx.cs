using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddTeacherSubject : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                LoadSubjects();
                LoadTeachers();
                GetTeacherSubjects();
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

        private void LoadTeachers()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT TeacherId, Name FROM Teachers");
                ddlTeacher.DataSource = dt;
                ddlTeacher.DataTextField = "Name";
                ddlTeacher.DataValueField = "TeacherId";
                ddlTeacher.DataBind();
                ddlTeacher.Items.Insert(0, new ListItem("--Select Teacher--", "0"));
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void GetTeacherSubjects()
        {
            try
            {
                DataTable dt = fn.Fetch(@"SELECT 
                    Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], 
                    ts.Id, 
                    c.ClassName, 
                    s.SubjectName, 
                    t.Name AS TeacherName, 
                    ts.created_at 
                FROM 
                    TeacherSubject ts 
                JOIN 
                    Class c ON ts.ClassId = c.ClassId 
                JOIN 
                    Subjects s ON ts.SubjectId = s.SubjectId 
                JOIN 
                    Teachers t ON ts.TeacherId = t.TeacherId");

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
                int teacherId = int.Parse(ddlTeacher.SelectedValue);

                if (classId == 0 || subjectId == 0 || teacherId == 0)
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("SELECT * FROM TeacherSubject WHERE ClassId = @ClassId AND SubjectId = @SubjectId AND TeacherId = @TeacherId",
                    new SqlParameter("@ClassId", classId),
                    new SqlParameter("@SubjectId", subjectId),
                    new SqlParameter("@TeacherId", teacherId));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO TeacherSubject (ClassId, SubjectId, TeacherId, created_at) VALUES(@ClassId, @SubjectId, @TeacherId, GETDATE())";
                    fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@TeacherId", teacherId));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetTeacherSubjects();
                }
                else
                {
                    lblMsg.Text = "The assignment you're trying to add already exists!";
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
            GetTeacherSubjects();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetTeacherSubjects();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetTeacherSubjects();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                int classId = int.Parse((row.FindControl("ddlClassEdit") as DropDownList).SelectedValue);
                int subjectId = int.Parse((row.FindControl("ddlSubjectEdit") as DropDownList).SelectedValue);
                int teacherId = int.Parse((row.FindControl("ddlTeacherEdit") as DropDownList).SelectedValue);

                if (classId == 0 || subjectId == 0 || teacherId == 0)
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE TeacherSubject SET ClassId = @ClassId, SubjectId = @SubjectId, TeacherId = @TeacherId WHERE Id = @Id";
                fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@TeacherId", teacherId), new SqlParameter("@Id", id));

                lblMsg.Text = "Assignment Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetTeacherSubjects();
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

                string query = "DELETE FROM TeacherSubject WHERE Id = @Id";
                fn.Query(query, new SqlParameter("@Id", id));

                lblMsg.Text = "Assignment Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetTeacherSubjects();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
