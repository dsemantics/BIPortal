$(document).ready(function () {
    var roleName;
    $(".btnAddRights").click(function () {
        roleName = document.getElementById("u733_input").value;
        if (roleName == "") {
            alert("Please enter a role");
            return false;
        }
        document.getElementById("u682").style.display = "block";
        document.getElementById("u723").style.display = "block";
        //$.ajax({
        //    url: getworkspaceurl,
        //    //data: { roleid: $('#txtSearch').val() },
        //    data: "",
        //    type: "GET",
        //    dataType: "json",
        //    success: function (data) {
        //        loadData(data);
        //    },
        //    error: function () {
        //        alert("Failed to get workspaces and reports! Please try again.");
        //    }
        //});

    });
    function loadData(data) {
        document.getElementById("u682").style.display = "block";
        document.getElementById("u723").style.display = "block";

        var tab = $('<table class="myTable"></table>');

        //$.each(data, function (i, workspaces) {
        //    var trow = $('<tr></tr>');
        //    trow.append('<td style="display:none;">' + workspaces.WorkSpaceId +  '</td>');
        //    trow.append('<td>' + workspaces.WorkSpaceName + '<input type="checkbox" name="chkWorkSpaces" value ="' + workspaces.WorkSpaceName + '" id=' + workspaces.WorkSpaceId + '>' + '</td>');

        //    tab.append(trow);
        //    $.each(workspaces.Reports, function (j, reports) {
        //        var trow = $('<tr></tr>');
        //        trow.append('<td style="display:none;">' + reports.ReportId + '</td>');
        //        trow.append('<td>' + reports.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + reports.ReportName + '" id=' + reports.ReportId + '>' + '</td>');
        //        tab.append(trow);
        //    });
        //});
        $.each(data, function (i, workspaces) {
            var workspaceid = workspaces.WorkSpaceId;
            var reportid = workspaces.ReportId;

            if (i == 0) {
                var trow = $('<tr></tr>');
                trow.append('<td style="font-weight:bold">' + workspaces.WorkSpaceName + '<input type="checkbox" name="chkWorkSpaces" value ="' + workspaces.WorkSpaceName + '" id=' + workspaces.WorkSpaceId + '>' + '</td>');
            }
            else {
                var previousWorkspaceid = data[i - 1].WorkSpaceId;
                if (previousWorkspaceid != workspaceid) {
                    var trow = $('<tr></tr>');
                    trow.append('<td style="font-weight:bold">' + workspaces.WorkSpaceName + '<input type="checkbox" name="chkWorkSpaces" value ="' + workspaces.WorkSpaceName + '" id=' + workspaces.WorkSpaceId + '>' + '</td>');
                }
            }
            tab.append(trow);
            if (reportid != null) {
                $.each(data, function (j, reports) {
                    if (reportid == reports.ReportId) {
                        var trow = $('<tr></tr>');
                        trow.append('<td>' + reports.ReportName + '<input type="checkbox" name="chkReports_' + i + '" value ="' + reports.ReportName + '" id=' + reports.ReportId + '>' + '</td>');
                    }
                    tab.append(trow);

                });
            }
        });

        $("#u723").html(tab);
    };

    $(".btnSaveRoleRights").click(function () {
        //alert("Save rights clicked");
        //check if any workspaces are selected
        //var checkedWorkspaces = document.querySelectorAll('input[name=chkWorkSpaces]:checked');
        //var workSpacesdata = [];
        //if (checkedWorkspaces.length > 0) {
        //    for (var i = 0; i < checkedWorkspaces.length; i++) {
        //        //Check if any reports are selected
        //        var checkedReports = document.querySelectorAll('input[name=chkReports_' + i + ']:checked');
        //        if (checkedReports.length > 0) {
        //            for (var j = 0; j < checkedReports.length; j++) {
        //                var obj = {};
        //                obj["WorkSpaceID"] = checkedWorkspaces[i].id;
        //                obj["WorkspaceName"] = checkedWorkspaces[i].value;
        //                obj["ReportID"] = checkedReports[j].id;
        //                obj["ReportName"] = checkedReports[j].value;

        //                workSpacesdata.push(obj);
        //            }
        //        } else {
        //            var obj = {};
        //            obj["WorkSpaceID"] = checkedWorkspaces[i].id;
        //            obj["WorkspaceName"] = checkedWorkspaces[i].value;
        //            obj["ReportID"] = null;
        //            obj["ReportName"] = null;

        //            workSpacesdata.push(obj);
        //        }
        //    }
        //} else {
        //    alert("Please select any one workspace");
        //    return false;
        //}
               
        //var data = { WorkspaceandReportList: workSpacesdata, RoleName: roleName };
        if (selectedItems.length == 0) {
            alert("Please select any one workspace or report");
            return false;
        }
        var data = { WorkspaceandReportList: selectedItems, RoleName: roleName };
        $.post(saveRolesandRightsurl, data, function (result) {
            // TODO: do something with the response from the controller action
            //window.location.reload();            
            window.location.href = viewrolesurl;
        });
    }); 

});