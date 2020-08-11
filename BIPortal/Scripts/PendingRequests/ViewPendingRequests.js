$(document).ready(function () {
    $("#u649").css('visibility', 'visible');
    $("#u644_state1").css('visibility', 'visible');

    $("#u342_img").click(function () {
        $("#u33333").hide();
    });

    var requestId;
    $(".btnViewPendingRequests").click(function () {
        
        document.getElementById("u33333").style.display = "block";

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

    //$(".btnSendremainder").click(function () {

    //    //document.getElementById("u33333").style.display = "block";

    //    var tr = $(this).closest('tr');
    //    //requestId = tr.find('input[name="RequestID"]').val();
    //    //roleName = tr.find('input[name="RoleName"]').val();

    //    $.ajax({
    //        url: sendRemainderEmailurl,
    //        //data: { roleid: $('#txtSearch').val() },
    //        //data: { requestId: tr.find('input[name="RequestID"]').val() },
    //       // data: { requestId: requestId },
    //        data: { },
    //        type: "GET",
    //        dataType: "json",
    //        success: function (data) {
    //           // loadTree(data);
    //        },
    //        error: function () {
    //            alert("Failed to get request details! Please try again.");
    //        }
    //    });

    //});

    function loadTree(data) {

        //document.getElementById("u33333").style.display = "block";
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

    
});