$(document).ready(function () {
    // This is for Get All Data
    var roleName;
    $(".btnViewRights").click(function () {

        var tr = $(this).closest('tr');
        //var FirstCol = tr.find('input[name="RoleID"]').val();
        roleName = tr.find('input[name="RoleName"]').val();
        //alert(tr);
        $.ajax({
            url: geRightsurl,
            //data: { roleid: $('#txtSearch').val() },
            data: { roleId: tr.find('input[name="RoleID"]').val() },
            type: "GET",
            dataType: "json",
            success: function (data) {
                loadData(data);
            },
            error: function () {
                alert("Failed to get rights! Please try again.");
            }
        });

    });
    function loadData(data) {
        document.getElementById("u427_div").style.display = "block";
        document.getElementById("lblRoleName").innerHTML = roleName;

        var tab = $('<table class="myTable"></table>');
        //var thead = $('<thead></thead>');
        //thead.append('<th>User ID</th>');
        //thead.append('<th>Username</th>');
        //thead.append('<th>Full Name</th>');
        //thead.append('<th>Email ID</th>');
        //thead.append('<th>Is Active</th>');

        //tab.append(thead);
        $.each(data, function (i, workspaces) {
            // Append database data here
            var workspaceid = workspaces.WorkspaceID;
            var reportid = workspaces.ReportID;

            if (i == 0) {
                var trow = $('<tr></tr>');
                trow.append('<td>' + workspaces.WorkspaceName + '<input type="checkbox" id="chkWorkspace_' + i + '"/>' + '</td>');
            }
            else {
                var previousWorkspaceid = data[i - 1].WorkspaceID;
                if (previousWorkspaceid != workspaceid) {
                    var trow = $('<tr></tr>');
                    trow.append('<td>' + workspaces.WorkspaceName + '<input type="checkbox" id="chkWorkspace_' + i + '"/>' + '</td>');
                }
            }
            //trow.append('<td>' + val.Username + '</td>');
            //trow.append('<td>' + val.FullName + '</td>');
            //trow.append('<td>' + val.EmailID + '</td>');
            //trow.append('<td>' + val.IsActive + '</td>');
            tab.append(trow);
            $.each(data, function (j, reports) {
                if (reportid == reports.ReportID) {
                    var trow = $('<tr></tr>');
                    trow.append('<td>' + reports.ReportName + '<input type="checkbox" id="chkReport_' + j + '"/>' + '</td>');
                }
                tab.append(trow);

            });
        });
        $("#UpdatePanel").html(tab);
    };
});