using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddSubject : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                GetSubjects();
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

        private void GetSubjects()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], s.SubjectId, c.ClassName, s.SubjectName, s.created_at FROM Subjects s JOIN Class c ON s.ClassId = c.ClassId");
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
                string subjectName = txtSubject.Text.Trim();

                if (classId == 0 || string.IsNullOrEmpty(subjectName))
                {
                    lblMsg.Text = "Class and Subject name cannot be empty.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("SELECT * FROM Subjects WHERE SubjectName = @SubjectName AND ClassId = @ClassId", new SqlParameter("@SubjectName", subjectName), new SqlParameter("@ClassId", classId));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO Subjects (ClassId, SubjectName, created_at) VALUES(@ClassId, @SubjectName, GETDATE())";
                    fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectName", subjectName));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetSubjects();
                }
                else
                {
                    lblMsg.Text = "The subject you're trying to add already exists for the selected class!";
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
            GetSubjects();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetSubjects();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetSubjects();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int subjectId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                string subjectName = (row.FindControl("txtSubjectEdit") as TextBox).Text;

                if (string.IsNullOrEmpty(subjectName))
                {
                    lblMsg.Text = "Subject name cannot be empty.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Subjects SET SubjectName = @SubjectName WHERE SubjectId = @SubjectId";
                fn.Query(query, new SqlParameter("@SubjectName", subjectName), new SqlParameter("@SubjectId", subjectId));

                lblMsg.Text = "Subject Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetSubjects();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
