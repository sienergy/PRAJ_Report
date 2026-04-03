<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Distillation.aspx.cs" Inherits="PRAJ_Report.Distillation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2>Distillation Report</h2>

    <table>
        <tr>
            <td>Start Date:</td>
            <td>
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>End Date:</td>
            <td>
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:Button ID="btnShow" runat="server" Text="Show Report" OnClick="btnShow_Click" />
            </td>
        </tr>
    </table>

    <br />

    <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="true" 
        CssClass="table table-bordered" />

</asp:Content>
