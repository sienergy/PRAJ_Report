using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PRAJ_Report
{
    public partial class Distillation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Optional: Set default dates
                txtStartDate.Text = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm");
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                // Get values from UI
                DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtEndDate.Text);

                // Get from Query String (URL)
                string dbName = Request.QueryString["db"];
                string typeStr = Request.QueryString["type"];

                int type = 0;
                if (!string.IsNullOrEmpty(typeStr))
                    type = Convert.ToInt32(typeStr);

                // Connection string from Web.config
                string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("GetTagData", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@DBName", dbName);
                        cmd.Parameters.AddWithValue("@Type", type);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvReport.DataSource = dt;
                        gvReport.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
            }
        }
    }
}