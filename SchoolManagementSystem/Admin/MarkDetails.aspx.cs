using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class MarkDetails : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                LoadSubjects();
                GetMarks();
            }
        }

        private void LoadClasses()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT ClassId, ClassName FROM Class");
                ddlClassFilter.DataSource = dt;
                ddlClassFilter.DataTextField = "ClassName";
                ddlClassFilter.DataValueField = "ClassId";
                ddlClassFilter.DataBind();
                ddlClassFilter.Items.Insert(0, new ListItem("--Select Class--", "0"));
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
                ddlSubjectFilter.DataSource = dt;
                ddlSubjectFilter.DataTextField = "SubjectName";
                ddlSubjectFilter.DataValueField = "SubjectId";
                ddlSubjectFilter.DataBind();
                ddlSubjectFilter.Items.Insert(0, new ListItem("--Select Subject--", "0"));
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void GetMarks(string classId = "0", string subjectId = "0", string rollNo = "")
        {
            try
            {
                string query = @"SELECT 
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
                    Subjects s ON e.SubjectId = s.SubjectId 
                WHERE 
                    (@ClassId = '0' OR e.ClassId = @ClassId) 
                    AND (@SubjectId = '0' OR e.SubjectId = @SubjectId) 
                    AND (@RollNo = '' OR e.RollNo LIKE '%' + @RollNo + '%')";

                DataTable dt = fn.Fetch(query,
                    new SqlParameter("@ClassId", classId),
                    new SqlParameter("@SubjectId", subjectId),
                    new SqlParameter("@RollNo", rollNo));

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string classId = ddlClassFilter.SelectedValue;
            string subjectId = ddlSubjectFilter.SelectedValue;
            string rollNo = txtRollNoFilter.Text.Trim();

            GetMarks(classId, subjectId, rollNo);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlClassFilter.SelectedIndex = 0;
            ddlSubjectFilter.SelectedIndex = 0;
            txtRollNoFilter.Text = string.Empty;

            GetMarks();
        }

        protected void ddlClassFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetMarks(ddlClassFilter.SelectedValue, ddlSubjectFilter.SelectedValue, txtRollNoFilter.Text.Trim());
        }

        protected void ddlSubjectFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetMarks(ddlClassFilter.SelectedValue, ddlSubjectFilter.SelectedValue, txtRollNoFilter.Text.Trim());
        }
    }
}
