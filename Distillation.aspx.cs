using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PRAJ_Report
{
    public partial class Distillation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm");
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

                SetDynamicTitle();
            }
        }

        private void SetDynamicTitle()
        {
            string db = Request.QueryString["db"];
            string type = Request.QueryString["type"];

            string section = "";
            string typeName = "";

            if (db == "PRAJ_EVAP")
                section = "Evaporation";
            else if (db == "PRAJ_DIST")
                section = "Distillation";
            else if (db == "PRAJ_LIQU")
                section = "Liquification";
            else
                section = "Process";

            switch (type)
            {
                case "1": typeName = "Temperature"; break;
                case "2": typeName = "Level"; break;
                case "3": typeName = "Pressure"; break;
                case "4": typeName = "Flow"; break;
                default: typeName = "Report"; break;
            }

            lblTitle.Text = section + " " + typeName + " Report";
        }

        protected void gvReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReport.PageIndex = e.NewPageIndex;
            btnShow_Click(null, null);
        }

        double[] columnSum;
        double[] columnMin;
        double[] columnMax;
        int rowCount = 0;

        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // 🔥 HEADER: DECIDE WIDTH MODE
            if (e.Row.RowType == DataControlRowType.Header)
            {
                int colCount = e.Row.Cells.Count;

                if (colCount > 15)
                {
                    gvReport.Attributes["style"] = "table-layout:fixed; width:100%;";
                    ViewState["FixedWidth"] = true;
                }
                else
                {
                    gvReport.Attributes["style"] = "table-layout:auto; width:100%;";
                    ViewState["FixedWidth"] = false;
                }

                // INIT ARRAYS (UNCHANGED)
                columnSum = new double[colCount];
                columnMin = new double[colCount];
                columnMax = new double[colCount];
                rowCount = 0;

                for (int i = 0; i < colCount; i++)
                {
                    columnMin[i] = double.MaxValue;
                    columnMax[i] = double.MinValue;
                }
            }

            bool isFixed = ViewState["FixedWidth"] != null && (bool)ViewState["FixedWidth"];

            // 🔥 APPLY WIDTH ONLY IF >15 COLUMNS
            if (isFixed)
            {
                if (e.Row.RowType == DataControlRowType.Header ||
                    e.Row.RowType == DataControlRowType.DataRow ||
                    e.Row.RowType == DataControlRowType.Footer)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        cell.BorderWidth = Unit.Pixel(1);
                        cell.BorderStyle = BorderStyle.Solid;
                        cell.BorderColor = System.Drawing.Color.Black;
                        cell.Width = Unit.Pixel(200);
                    }
                }
            }

            // 🔥 DATA ROW CALCULATION (UNCHANGED)
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                rowCount++;

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    double val;
                    if (double.TryParse(e.Row.Cells[i].Text, out val))
                    {
                        columnSum[i] += val;

                        if (val < columnMin[i]) columnMin[i] = val;
                        if (val > columnMax[i]) columnMax[i] = val;
                    }
                }
            }

            // 🔥 FOOTER (MIN/MAX/AVG) (UNCHANGED)
            if (e.Row.RowType == DataControlRowType.Footer)
            {

                GridViewRow minRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                GridViewRow maxRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                GridViewRow avgRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    TableCell minCell = new TableCell();
                    TableCell maxCell = new TableCell();
                    TableCell avgCell = new TableCell();

                    if (isFixed)
                    {
                        minCell.Width = Unit.Pixel(200);
                        maxCell.Width = Unit.Pixel(200);
                        avgCell.Width = Unit.Pixel(200);
                    }

                    if (i == 0)
                    {
                        minCell.Text = "Min";
                        maxCell.Text = "Max";
                        avgCell.Text = "Avg";
                    }
                    else if (rowCount > 0)
                    {
                        double avg = columnSum[i] / rowCount;

                        minCell.Text = columnMin[i].ToString("0.00");
                        maxCell.Text = columnMax[i].ToString("0.00");
                        avgCell.Text = avg.ToString("0.00");
                    }

                    minRow.Cells.Add(minCell);
                    maxRow.Cells.Add(maxCell);
                    avgRow.Cells.Add(avgCell);
                }

                gvReport.Controls[0].Controls.Add(minRow);
                gvReport.Controls[0].Controls.Add(maxRow);
                gvReport.Controls[0].Controls.Add(avgRow);

                e.Row.Visible = false;
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtEndDate.Text);

                string dbName = Request.QueryString["db"];
                string typeStr = Request.QueryString["type"];

                int type = 0;
                if (!string.IsNullOrEmpty(typeStr))
                    type = Convert.ToInt32(typeStr);

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


        protected void btnExport_Click(object sender, EventArgs e)
        {
            // 🔥 GET INPUT
            DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
            DateTime endDate = Convert.ToDateTime(txtEndDate.Text);

            string dbName = Request.QueryString["db"];
            int type = Convert.ToInt32(Request.QueryString["type"]);

            DataTable dt = new DataTable();

            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            // 🔥 FETCH DATA FROM SQL
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
                    da.Fill(dt);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (SpreadsheetDocument doc =
                    SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart wbPart = doc.AddWorkbookPart();
                    wbPart.Workbook = new Workbook();

                    WorksheetPart wsPart = wbPart.AddNewPart<WorksheetPart>();

                    // 🔥 STYLES
                    WorkbookStylesPart stylesPart = wbPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CreateStyles();
                    stylesPart.Stylesheet.Save();

                    SheetData sheetData = new SheetData();

                    // 🔥 COLUMN WIDTH = 200px
                    Columns cols = new Columns();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cols.Append(new Column()
                        {
                            Min = (uint)(i + 1),
                            Max = (uint)(i + 1),
                            Width = 28, // ≈ 200px
                            CustomWidth = true
                        });
                    }

                    wsPart.Worksheet = new Worksheet();
                    wsPart.Worksheet.Append(cols);
                    wsPart.Worksheet.Append(sheetData);

                    Sheets sheets = wbPart.Workbook.AppendChild(new Sheets());
                    sheets.Append(new Sheet()
                    {
                        Id = wbPart.GetIdOfPart(wsPart),
                        SheetId = 1,
                        Name = "Report"
                    });

                    MergeCells mergeCells = new MergeCells();

                    // 🔥 COMPANY NAME ROW
                    Row companyRow = new Row();
                    companyRow.Append(CreateCell("M/s. JURALA ORGANIC FARMS & AGRO INDUSTRIES LLP", 4));
                    sheetData.Append(companyRow);

                    mergeCells.Append(new MergeCell()
                    {
                        Reference = new StringValue($"A1:{GetExcelColumnName(dt.Columns.Count)}1")
                    });

                    // 🔥 REPORT TITLE ROW
                    Row titleRow = new Row();
                    titleRow.Append(CreateCell(lblTitle.Text, 4));
                    sheetData.Append(titleRow);

                    mergeCells.Append(new MergeCell()
                    {
                        Reference = new StringValue($"A2:{GetExcelColumnName(dt.Columns.Count)}2")
                    });

                    // 🔥 HEADER ROW
                    Row headerRow = new Row();
                    foreach (DataColumn col in dt.Columns)
                    {
                        headerRow.Append(CreateCell(col.ColumnName, 2));
                    }
                    sheetData.Append(headerRow);

                    int rowCount = 0;
                    double[] sum = new double[dt.Columns.Count];
                    double[] min = new double[dt.Columns.Count];
                    double[] max = new double[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        min[i] = double.MaxValue;
                        max[i] = double.MinValue;
                    }

                    // 🔥 DATA ROWS
                    foreach (DataRow dr in dt.Rows)
                    {
                        Row row = new Row();
                        row.CustomHeight = false;
                        rowCount++;

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string val = dr[i].ToString();
                            row.Append(CreateCell(val, 1));

                            double d;
                            if (i > 0 && double.TryParse(val, out d))
                            {
                                sum[i] += d;
                                if (d < min[i]) min[i] = d;
                                if (d > max[i]) max[i] = d;
                            }
                        }

                        sheetData.Append(row);
                    }

                    // 🔥 MIN ROW
                    sheetData.Append(CreateSummaryRow("Min", min, dt.Columns.Count));

                    // 🔥 MAX ROW
                    sheetData.Append(CreateSummaryRow("Max", max, dt.Columns.Count));

                    // 🔥 AVG ROW
                    double[] avg = new double[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                        avg[i] = rowCount > 0 ? sum[i] / rowCount : 0;

                    sheetData.Append(CreateSummaryRow("Avg", avg, dt.Columns.Count));

                    wsPart.Worksheet.Append(mergeCells);

                    wbPart.Workbook.Save();
                }

                // 🔥 DOWNLOAD
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=DistillationReport.xlsx");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }

        // 🔥 CREATE CELL
        private Cell CreateCell(string text, uint styleIndex)
        {
            return new Cell()
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text),
                StyleIndex = styleIndex
            };
        }

        // 🔥 SUMMARY ROW
        private Row CreateSummaryRow(string title, double[] values, int colCount)
        {
            Row row = new Row();

            for (int i = 0; i < colCount; i++)
            {
                if (i == 0)
                    row.Append(CreateCell(title, 3));
                else
                    row.Append(CreateCell(values[i].ToString("0.00"), 3));
            }

            return row;
        }

        // 🔥 COLUMN NAME (A, B, C...)
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        // 🔥 STYLES
        private Stylesheet CreateStyles()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(),
                    new Font(new Bold())
                ),
                new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = "1F3A5F" }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = "D1ECF1" }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = "FFF3CD" }) { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(),
                    new Border(
                        new LeftBorder() { Style = BorderStyleValues.Thin },
                        new RightBorder() { Style = BorderStyleValues.Thin },
                        new TopBorder() { Style = BorderStyleValues.Thin },
                        new BottomBorder() { Style = BorderStyleValues.Thin }
                    )
                ),
                new CellFormats(
                    new CellFormat(),
                    new CellFormat() { FillId = 3, BorderId = 1, ApplyFill = true },
                    new CellFormat() { FillId = 2, FontId = 1, BorderId = 1, ApplyFill = true },
                    new CellFormat() { FillId = 4, FontId = 1, BorderId = 1, ApplyFill = true },
                    new CellFormat()
                    {
                        FontId = 1,
                        Alignment = new Alignment()
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        },
                        ApplyAlignment = true
                    }
                )
            );
        }
    }
}