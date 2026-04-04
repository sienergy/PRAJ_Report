<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Distillation.aspx.cs" Inherits="PRAJ_Report.Distillation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style>

/* 🔷 HEADER */
.page-title {
    background: linear-gradient(90deg, #ff7e5f, #6a11cb);
    color: #fff;
    padding: 15px;
    border-radius: 10px;
    margin-bottom: 20px;
    box-shadow: 0 4px 15px rgba(0,0,0,0.2);
}

/* 🔷 CARD */
.card-box {
    background: #fff;
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 5px 20px rgba(0,0,0,0.1);
    margin-bottom: 20px;
}

/* 🔷 INPUT */
.form-control {
    border-radius: 8px;
    height: 45px;
}

/* 🔷 BUTTON */
.btn-custom {
    background: linear-gradient(90deg, #36d1dc, #5b86e5);
    color: #fff;
    border: none;
    border-radius: 8px;
    height: 45px;
    font-weight: bold;
}

/* 🔥 MAIN TABLE */
#gvReport {
    width: 100%;
    border-collapse: collapse;
    border: 2px solid #007bff;

}



/* 🔷 ALT ROW */
#gvReport tr:nth-child(even) td {
    background-color: #bee5eb;
}

/* 🔷 HOVER */
#gvReport tr:hover td {
    background-color: #ffe082;
}

/* 🔷 FIRST COLUMN */
#gvReport td:first-child {
    background-color: #343a40;
    color: #fff;
}

/* 🔷 WARNING FOOTER */
#gvReport tr:last-child td,
#gvReport tr:nth-last-child(2) td,
#gvReport tr:nth-last-child(3) td {
    background-color: #fff3cd;
    color: #856404;
    font-weight: bold;
}
.btn-export {
    background: linear-gradient(90deg, #28a745, #20c997);
    color: #fff;
    border: none;
    border-radius: 8px;
    height: 45px;
    font-weight: bold;
}
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="page-title">
    <h3>
        <i class="fa fa-chart-line"></i>
        <asp:Label ID="lblTitle" runat="server"></asp:Label>
    </h3>
</div>

<div class="card-box">
    <div class="row g-3 align-items-end">

        <div class="col-md-2 d-grid">
            <label><b>Start Date</b></label>
            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
        </div>

        <div class="col-md-2 d-grid">
            <label><b>End Date</b></label>
            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
        </div>
        <div class="col-md-2 d-grid">
    <label><b>Interval</b></label>
    <asp:DropDownList ID="ddlInterval" runat="server" CssClass="form-control">
        <asp:ListItem Text="1 Minute" Value="1" />
        <asp:ListItem Text="5 Minutes" Value="5" Selected="True" />
        <asp:ListItem Text="15 Minutes" Value="15" />
        <asp:ListItem Text="30 Minutes" Value="30" />
        <asp:ListItem Text="1 Hour" Value="60" />
        <asp:ListItem Text="2 Hours" Value="120" />
    </asp:DropDownList>
</div>
        <div class="col-md-2 d-grid">
            <asp:Button ID="btnShow" runat="server"
                Text="🚀 Show Report"
                CssClass="btn btn-custom"
                OnClick="btnShow_Click" />
        </div>
        <div class="col-md-2 d-grid">
    <asp:Button ID="btnExport" runat="server"
    Text="📥 Export to Excel"
    CssClass="btn btn-export"
    OnClick="btnExport_Click" />
</div>

    </div>
</div>

<div class="card-box">
    <div class="table-responsive">

        <asp:GridView ID="gvReport" runat="server"
            AutoGenerateColumns="true"
            CssClass="text-center"
            AllowPaging="true"
            PageSize="30"
            GridLines="Both"
            OnPageIndexChanging="gvReport_PageIndexChanging"
            OnRowDataBound="gvReport_RowDataBound"
            ShowFooter="true">

            <HeaderStyle BackColor="#1f3a5f" ForeColor="White" Font-Bold="true" />

            <RowStyle BackColor="#d1ecf1" ForeColor="#212529" />
            <AlternatingRowStyle BackColor="#bee5eb" ForeColor="#212529" />

            <FooterStyle BackColor="#fff3cd" ForeColor="#856404" Font-Bold="true" />

        </asp:GridView>

    </div>
</div>

</asp:Content>