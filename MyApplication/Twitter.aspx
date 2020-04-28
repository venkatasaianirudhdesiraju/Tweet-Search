<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Twitter.aspx.cs" Inherits="MyApplication.Twitter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="jumbotron text-center">
            <h1>Twitter search</h1>
            <p>Welcome to my first twitter search application! Using this page you can search for the most popular tweets for the text entered.</p>
        </div>
        <div class="container text-center">
            <div class="row"> 
                Enter some text here : 
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox><br /><br />
                <asp:Button CssClass="btn btn-primary" ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                <hr />
                <asp:GridView ID="grdResults" runat="server" CssClass="table table-hover table-striped" AutoGenerateColumns="false"
                    cellspacing="0" style="border-collapse:collapse;" GridLines="None" >
                    <Columns>
                        <asp:BoundField DataField="user_name"   HeaderText="User Name" ItemStyle-HorizontalAlign="Left"/>
                        <asp:BoundField DataField="text"  HeaderText="Text" ItemStyle-HorizontalAlign="Left"/>
                        <asp:BoundField DataField="created_at"  DataFormatString="{0:d}" HeaderText="Creation Date" ItemStyle-HorizontalAlign="Left" />
                 <%--       <asp:BoundField DataField="" DataFormatString="" HeaderText="" />--%>

                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
