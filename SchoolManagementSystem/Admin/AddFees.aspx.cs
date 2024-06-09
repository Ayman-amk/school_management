using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddFees : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                GetFees();
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

        private void GetFees()
        {
            try
            {
                DataTable dt = fn.Fetch("SELECT Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], f.FeesId, c.ClassName, f.FeesAmount, f.created_at FROM Fees f JOIN Class c ON f.ClassId = c.ClassId");
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
                int feesAmount = int.Parse(txtFeesAmount.Text.Trim());

                if (classId == 0 || feesAmount <= 0)
                {
                    lblMsg.Text = "Class and Fees amount cannot be empty or zero.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                DataTable dt = fn.Fetch("SELECT * FROM Fees WHERE ClassId = @ClassId AND FeesAmount = @FeesAmount", new SqlParameter("@ClassId", classId), new SqlParameter("@FeesAmount", feesAmount));

                if (dt.Rows.Count == 0)
                {
                    string query = "INSERT INTO Fees (ClassId, FeesAmount, created_at) VALUES(@ClassId, @FeesAmount, GETDATE())";
                    fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@FeesAmount", feesAmount));

                    lblMsg.Text = "Inserted Successfully !!";
                    lblMsg.CssClass = "alert alert-success";

                    GetFees();
                }
                else
                {
                    lblMsg.Text = "The fee record you're trying to add already exists!";
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
            GetFees();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetFees();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetFees();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int feesId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                int feesAmount = int.Parse((row.FindControl("txtFeesAmountEdit") as TextBox).Text);

                if (feesAmount <= 0)
                {
                    lblMsg.Text = "Fees amount cannot be zero.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Fees SET FeesAmount = @FeesAmount WHERE FeesId = @FeesId";
                fn.Query(query, new SqlParameter("@FeesAmount", feesAmount), new SqlParameter("@FeesId", feesId));

                lblMsg.Text = "Fees Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetFees();
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
                int feesId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM Fees WHERE FeesId = @FeesId";
                fn.Query(query, new SqlParameter("@FeesId", feesId));

                lblMsg.Text = "Fees Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetFees();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
