$(document).ready(function () {
    // This is for Get All Data
    //var roleName;
    var requestId;
    $(".btnViewPendingApprovals").click(function () {

        var tr = $(this).closest('tr');
        requestId = tr.find('input[name="RequestID"]').val();
        //roleName = tr.find('input[name="RoleName"]').val();


        $.ajax({
            url: getRequestDetailssurl,
            //data: { roleid: $('#txtSearch').val() },
            data: { requestId: tr.find('input[name="RequestID"]').val() },
            type: "GET",
            dataType: "json",
            success: function (data) {                
                loadTree(data);
            },
            error: function () {
                alert("Failed to get request details! Please try again.");
            }
        });

    });

    function loadTree(data) {

        document.getElementById("u2274").style.display = "block";
        document.getElementById("u452").style.display = "block";        
        $('#u452').jstree('destroy');        
        $(function () {
            $('#u452').on('changed.jstree', function (e, data) {
                var i, j;
                selectedItems = [];

                var selectedElms = $('#u452').jstree("get_selected", true);
                $.each(selectedElms, function (i, selectedElm) {

                    selectedItems.push(
                        {
                            id: this.id,
                            parent: this.parent,
                            text: this.text,
                            parenttext: this.original.parenttext
                        }
                    );
                });

                //Serialize the JSON Array and save in HiddenField.
                $('#selectedItems').val(JSON.stringify(selectedItems));
            }).jstree({
                "core": {
                    "themes": {
                        "variant": "large"
                    },
                    "data": data
                },
                //"types": {
                //    "default": { "icon": "//jstree.com/tree.png" },
                //    "element": { "icon": "//jstree.com/tree.png" }
                //},
                "checkbox": {
                    "keep_selected_style": false
                },
                "plugins": ["wholerow", "checkbox", "types"],
            })

        });

    };


    $(".btnApprove").click(function () {
        
        if (selectedItems.length == 0) {
            alert("Please select any one workspace or report");
            return false;
        }
        var data = { WorkspaceandReportList: selectedItems, RequestID: requestId };
        $.post(approveRequesturl, data, function (result) {
            // TODO: do something with the response from the controller action
            window.location.reload();
        });
    });

    $(".btnReject").click(function () {

        if (selectedItems.length == 0) {
            alert("Please select any one workspace or report");
            return false;
        }
        var data = { WorkspaceandReportList: selectedItems, RequestID: requestId };
        $.post(rejectRequesturl, data, function (result) {
            // TODO: do something with the response from the controller action
            window.location.reload();
        });
    });
});