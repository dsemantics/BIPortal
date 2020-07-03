$(document).ready(function () {
    // This is for Get All Data
    var roleName;
    var roleId;
    $(".btnViewRights").click(function () {

        var tr = $(this).closest('tr');
        roleId = tr.find('input[name="RoleID"]').val();
        roleName = tr.find('input[name="RoleName"]').val();        
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
        document.getElementById("u425").style.display = "block";
        document.getElementById("u452").style.display = "block";        
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
            var isWorkspaceActive = workspaces.Active;
            if (i == 0) {
                var trow = $('<tr></tr>');
                //trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" id="chkWorkspace_' + i + '"/>' + '</td>');
                if (isWorkspaceActive) {
                    trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" name="chkWorkSpaces" checked value ="' + workspaces.WorkspaceName + '" id=' + workspaces.WorkspaceID + '>' + '</td>');
                }
                else {                    
                    trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" name="chkWorkSpaces" value ="' + workspaces.WorkspaceName + '" id=' + workspaces.WorkspaceID + '>' + '</td>');
                }
                tab.append(trow);


                var trow = $('<tr></tr>');
                if (reportid != null) {
                    if (isWorkspaceActive) {
                        trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" checked id=' + workspaces.ReportID + '>' + '</td>');
                    }
                    else {
                        trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" id=' + workspaces.ReportID + '>' + '</td>');
                    }
                    tab.append(trow);
                }
            }
            else {
                var previousWorkspaceid = data[i - 1].WorkspaceID;
                if (previousWorkspaceid != workspaceid) {
                    var trow = $('<tr></tr>');
                    //trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" id="chkWorkspace_' + i + '"/>' + '</td>');
                    if (isWorkspaceActive) {
                        trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" name="chkWorkSpaces" checked value ="' + workspaces.WorkspaceName + '" id=' + workspaces.WorkspaceID + '>' + '</td>');
                    }
                    else {
                        trow.append('<td style="font-weight:bold">' + workspaces.WorkspaceName + '<input type="checkbox" name="chkWorkSpaces" value ="' + workspaces.WorkspaceName + '" id=' + workspaces.WorkspaceID + '>' + '</td>');
                    }
                    tab.append(trow);

                    var trow = $('<tr></tr>');
                    if (reportid != null) {
                        if (isWorkspaceActive) {
                            trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" checked id=' + workspaces.ReportID + '>' + '</td>');
                        }
                        else {
                            trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" id=' + workspaces.ReportID + '>' + '</td>');
                        }
                        tab.append(trow);
                    }
                }
                else {
                    var previousReportid = data[i - 1].ReportID;
                    if (previousReportid != reportid) {
                        var trow = $('<tr></tr>');
                        if (reportid != null) {
                            if (isWorkspaceActive) {
                                trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" checked id=' + workspaces.ReportID + '>' + '</td>');
                            }
                            else {
                                trow.append('<td>' + workspaces.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + workspaces.ReportName + '" id=' + workspaces.ReportID + '>' + '</td>');
                            }
                            tab.append(trow);
                        }
                    }
                }

            }
            //trow.append('<td>' + val.Username + '</td>');
            //trow.append('<td>' + val.FullName + '</td>');
            //trow.append('<td>' + val.EmailID + '</td>');
            //trow.append('<td>' + val.IsActive + '</td>');
            //tab.append(trow);
            //if (reportid != null) {
            //    $.each(data, function (j, reports) {
            //        if (reportid == reports.ReportID) {
            //            var trow = $('<tr></tr>');
            //            var isReportActive = reports.Active;
            //            //if (isReportActive) {
            //            //    trow.append('<td>' + reports.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + reports.ReportName + '" checked id=' + reports.ReportID + '>' + '</td>');
            //            //}
            //            //else {
            //            //    trow.append('<td>' + reports.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + reports.ReportName + '" id=' + reports.ReportID + '>' + '</td>');
            //            //}
            //            trow.append('<td>' + reports.ReportName + '<input type="checkbox" id="chkReport_' + j + '"/>' + '</td>');
                        
            //        }
            //        tab.append(trow);

            //    });
            //}
        });
        //$("#UpdatePanel").html(tab);
        $("#u452").html(tab);

        $(".btnSaveRoleRights").click(function () {
            //alert("Save rights clicked");
            //check if any workspaces are selected
            var checkedWorkspaces = document.querySelectorAll('input[name=chkWorkSpaces]:checked');
            var workSpacesdata = [];
            if (checkedWorkspaces.length > 0) {
                for (var i = 0; i < checkedWorkspaces.length; i++) {
                    //Check if any reports are selected
                    var checkedReports = document.querySelectorAll('input[name=chkReports_' + i + ']:checked');
                    if (checkedReports.length > 0) {
                        for (var j = 0; j < checkedReports.length; j++) {
                            var obj = {};
                            obj["WorkSpaceID"] = checkedWorkspaces[i].id;
                            obj["WorkspaceName"] = checkedWorkspaces[i].value;
                            obj["ReportID"] = checkedReports[j].id;
                            obj["ReportName"] = checkedReports[j].value;
                            obj["Active"] = true;
                            obj["RoleID"] = roleId;

                            workSpacesdata.push(obj);
                        }
                    } else {
                        var obj = {};
                        obj["WorkSpaceID"] = checkedWorkspaces[i].id;
                        obj["WorkspaceName"] = checkedWorkspaces[i].value;
                        obj["ReportID"] = null;
                        obj["ReportName"] = null;
                        obj["Active"] = true;
                        obj["RoleID"] = roleId;

                        workSpacesdata.push(obj);
                    }
                }
            } else {
                alert("Please select any one workspace");
                return false;
            }

            //var data = { WorkspaceandReportList: workSpacesdata, RoleID: roleId };
            var data = { WorkspaceandReportList: workSpacesdata };
            $.post(updateRolesandRightsurl, data, function (result) {
                // TODO: do something with the response from the controller action
                window.location.reload();
            });
        });
    };
});