using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class AddExpense : System.Web.UI.Page
    {
        CommonFnx fn = new CommonFnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClasses();
                LoadSubjects();
                GetExpenses();
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

        private void GetExpenses()
        {
            try
            {
                DataTable dt = fn.Fetch(@"SELECT 
                    Row_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], 
                    e.ExpenseId, 
                    c.ClassName, 
                    s.SubjectName, 
                    e.ChargeAmount, 
                    e.created_at 
                FROM 
                    Expense e 
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
                int chargeAmount = int.Parse(txtChargeAmount.Text.Trim());

                if (classId == 0 || subjectId == 0 || chargeAmount <= 0)
                {
                    lblMsg.Text = "All fields are required.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "INSERT INTO Expense (ClassId, SubjectId, ChargeAmount, created_at) VALUES(@ClassId, @SubjectId, @ChargeAmount, GETDATE())";
                fn.Query(query, new SqlParameter("@ClassId", classId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@ChargeAmount", chargeAmount));

                lblMsg.Text = "Inserted Successfully !!";
                lblMsg.CssClass = "alert alert-success";

                GetExpenses();
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
            GetExpenses();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetExpenses();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetExpenses();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int expenseId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                int chargeAmount = int.Parse((row.FindControl("txtChargeAmountEdit") as TextBox).Text);

                if (chargeAmount <= 0)
                {
                    lblMsg.Text = "Charge amount must be greater than zero.";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                string query = "UPDATE Expense SET ChargeAmount = @ChargeAmount WHERE ExpenseId = @ExpenseId";
                fn.Query(query, new SqlParameter("@ChargeAmount", chargeAmount), new SqlParameter("@ExpenseId", expenseId));

                lblMsg.Text = "Expense Updated Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetExpenses();
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
                int expenseId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

                string query = "DELETE FROM Expense WHERE ExpenseId = @ExpenseId";
                fn.Query(query, new SqlParameter("@ExpenseId", expenseId));

                lblMsg.Text = "Expense Deleted Successfully !!";
                lblMsg.CssClass = "alert alert-success";
                GetExpenses();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}
