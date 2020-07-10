$(document).ready(function () {
    $("#u342_img").click(function () {
        $("#u333").hide();
    });

    $(".btnViewRights").click(function () {
        var tr = $(this).closest('tr');
        workspaceid = tr.find('input[name="WorkspaceID"]').val();
        workSpaceName = tr.find('input[name="WorkspaceName"]').val(); 
        ownerid = tr.find('input[name="OwnerID"]').val();

        $.ajax({
            url: getReportsOwnerurl,
            //data: { roleid: $('#txtSearch').val() },
            data: { workspaceid: tr.find('input[name="WorkspaceID"]').val() },
            type: "GET",
            dataType: "json",
            success: function (data) {
                loadData(data);                
            },
            error: function () {
                alert("Failed to get reports! Please try again.");
            }
        });

        $(".btnSaveOwner").click(function () {            
            var e = document.getElementById("u343_input");
            var selectedOwnerValue = e.options[e.selectedIndex].value;

            var selectedOwner = {};
            selectedOwner["WorkspaceID"] = workspaceid;
            selectedOwner["OwnerID"] = selectedOwnerValue;
            var data = { WorkspaceOwnerdata: selectedOwner };
            $.post(saveWorkspaceOwnerurl, data, function (result) {
                // TODO: do something with the response from the controller action
                window.location.reload();
            });
        });

        function loadData(data) {
            document.getElementById("u333").style.display = "block";
            document.getElementById("lblWorkspaceName").innerHTML = workSpaceName;

            var tab = $('<table class="tblReports"></table>');          
            $.each(data.Reports, function (i, report) {
                var trow = $('<tr></tr>');                
                trow.append('<td>' + report.ReportName + '</td>');
                tab.append(trow);
            });
            $("#u336").html(tab);            

            var option = '';
            var selectDropdown = $('<select id="u343_input" class="u343_input"' + '>' + '</select>');
            $.each(data.Users, function (i, user) {
                if (ownerid == user.UserID) { option += '<option class="u343_input_option" selected value="' + user.UserID + '">' + user.UserName + '</option>'; }
                else { option += '<option class="u343_input_option" value="' + user.UserID + '">' + user.UserName + '</option>';}               
            });
            selectDropdown.append(option);            
            $("#u343").html(selectDropdown);
        };

    });
});