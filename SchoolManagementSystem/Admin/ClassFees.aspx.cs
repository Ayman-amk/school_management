using SchoolManagementSystem.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class ClassFees : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetClass();
                GetFees();
            }
        }

        private void GetClass()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT * FROM Class");
                ddlClass.DataSource = dt;
                ddlClass.DataTextField = "ClassName";
                ddlClass.DataValueField = "ClassID";
                ddlClass.DataBind();
                ddlClass.Items.Insert(0, "Select Class");
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
                if (ddlClass.SelectedIndex == 0)
                {
                    lblMsg.Text = "Please select a class.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string classId = ddlClass.SelectedItem.Value;
                string FeesAmount = txtFeesAmounts.Text.Trim();

                DataTable dt = fn.Fetch("SELECT * FROM Fees WHERE ClassId = @ClassId", new SqlParameter("@ClassId", classId));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO Fees (ClassId, FeesAmount) VALUES (@ClassId, @FeesAmount)";
                    fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@FeesAmount", FeesAmount));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    ddlClass.SelectedIndex = 0;
                    txtFeesAmounts.Text = string.Empty;
                    GetFees();
                }
                else
                {
                    string className = ddlClass.SelectedItem.Text;
                    lblMsg.Text = $"The fees you're trying to add already exist for <b>'{className}'</b>!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void GetFees()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT * FROM Fees");
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
