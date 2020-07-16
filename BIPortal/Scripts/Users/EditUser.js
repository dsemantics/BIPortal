$(document).ready(function () {
    // This is for Get All Data
    var roleName;
    //var roleID = [];
    //alert("test");
   // var roleID;
    //var userID;


    $(".btnViewRights").click(function () {

        var selectedValues = [];
        $("#lstSelect :selected").each(function () {
            selectedValues.push($(this).val());
        });

        var userID = $("#UserID").val();

        // need to add if condition for selectedValues
        if (selectedValues.length == 0) {
            alert("Please select any one Role");
            return false;
        }

        //var userarray = { UserID: selectedValues };
        var userarray = { roleID: selectedValues, userID: userID };

        $.ajax({
            url: geRightsurl,
            data: userarray,
            type: "GET",
            dataType: "json",
            traditional: true,
            success: function (data) {
                //loadData(data);
                loadTree(data);
            },
            error: function () {
                alert("Failed to get rights! Please try again.");
            }
        });

    });

    function loadTree(data) {

        document.getElementById("u425").style.display = "block";
        document.getElementById("u452").style.display = "block";
        //document.getElementById("lblRoleName").innerHTML = roleName;
        document.getElementById("lblRoleName").innerHTML = "Has Access To";

        // $('#u452').jstree('destroy');
        //var selectedItems = [];
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


    $(".btnSaveRoleRights").click(function () {

        if (selectedItems.length == 0) {
            alert("Please select any one workspace or report");
            return false;
        }
        var data = { WorkspaceandReportList: selectedItems, RoleID: roleId };
        $.post(updateRolesandRightsurl, data, function (result) {
            // TODO: do something with the response from the controller action
            window.location.reload();
        });
    });
});