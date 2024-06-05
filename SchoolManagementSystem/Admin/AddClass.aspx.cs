using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddClass : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetClass();
            }
        }

        private void GetClass()
        {
            try
            {
                DataTable dt = fn.Fetch("Select Row_NUMBER() over(Order by (Select 1)) as [Sr.No], ClassId, ClassName, created_at From Class");
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
                string className = txtClass.Text.Trim();
                if (string.IsNullOrEmpty(className))
                {
                    lblMsg.Text = "Class name cannot be empty.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("Select * From Class Where ClassName = @ClassName", new SqlParameter("@ClassName", className));

                if (dt.Rows.Count == 0)
                {
                    string query = "Insert Into Class (ClassName, created_at) Values(@ClassName, GETDATE())";
                    fn.Query(query, new SqlParameter("@ClassName", className));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetClass();
                }
                else
                {
                    lblMsg.Text = "The class you're trying to add already exists!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
