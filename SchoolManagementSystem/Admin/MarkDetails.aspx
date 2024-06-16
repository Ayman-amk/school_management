<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminMst.Master" AutoEventWireup="true" CodeBehind="MarkDetails.aspx.cs" Inherits="SchoolManagementSystem.Admin.MarkDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="background-image:url('../Images/bg3.jpg'); width:103%; height:720px; background-repeat: no-repeat; background-size:cover; background-attachment:fixed;">
        <div class="container p-md-4 p-sm-4">
            <div>
                <asp:Label ID="lblMsg" runat="server"></asp:Label>
            </div>
            <h2 class="text-center">Mark Details</h2>
            <div class="row mb-3 mr-lg-5 mt-md-5">
                <div class="col-md-4">
                    <label for="ddlClassFilter">Class</label>
                    <asp:DropDownList ID="ddlClassFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlClassFilter_SelectedIndexChanged" />
                </div>
                <div class="col-md-4">
                    <label for="ddlSubjectFilter">Subject</label>
                    <asp:DropDownList ID="ddlSubjectFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSubjectFilter_SelectedIndexChanged" />
                </div>
                <div class="col-md-4">
                    <label for="txtRollNoFilter">Roll Number</label>
                    <asp:TextBox ID="txtRollNoFilter" runat="server" CssClass="form-control" placeholder="Enter Roll Number" Width="100%" />
                </div>
            </div>
            <div class="row mb-3 mr-lg-5 ml-lg-5">
                <div class="col-md-3 col-md-offset-2 mb-3">
                    <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary btn-block" BackColor="#4561F5" Text="Filter" OnClick="btnFilter_Click" />
                </div>
                <div class="col-md-3 col-md-offset-2 mb-3">
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-secondary btn-block" Text="Clear" OnClick="btnClear_Click" />
                </div>
            </div>
            <div class="row mb-3 mr-lg-5 ml-lg-5">
                <div class="col-md-12">
                    <asp:GridView ID="GridView1" runat="server" CssClass="table table-hover table-bordered" DataKeyNames="ExamId" AutoGenerateColumns="False"
                        EmptyDataText="No Record to display!" AllowPaging="true" PageSize="10" Width="100%">
                        <Columns>
                            <asp:BoundField DataField="Sr.No" HeaderText="Sr.No" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Class">
                                <ItemTemplate>
                                    <asp:Label ID="lblClassName" runat="server" Text='<%# Eval("ClassName") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Subject">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubjectName" runat="server" Text='<%# Eval("SubjectName") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Roll Number">
                                <ItemTemplate>
                                    <asp:Label ID="lblRollNo" runat="server" Text='<%# Eval("RollNo") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total Marks">
                                <ItemTemplate>
                                    <asp:Label ID="lblTotalMark" runat="server" Text='<%# Eval("TotalMark") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Out of Marks">
                                <ItemTemplate>
                                    <asp:Label ID="lblOutOfMark" runat="server" Text='<%# Eval("OutOfMark") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Created At">
                                <ItemTemplate>
                                    <asp:Label ID="lblCreatedAt" runat="server" Text='<%# Eval("created_at", "{0:yyyy-MM-dd}") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#5558C9" ForeColor="White" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
