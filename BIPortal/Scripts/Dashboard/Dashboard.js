$(document).ready(function () {    
    var requestId;
    var workspaceName;
    $(".btnViewReports").click(function () {

        var tr = $(this).closest('tr');
        requestId = tr.find('input[name="RequestID"]').val();
        workspaceName = tr.find('input[name="WorkspaceName"]').val();
        document.getElementById("spnWorkspaceName").innerHTML = workspaceName + " / Reports";

        document.getElementById("u83_state0").style.display = "none";
        document.getElementById("u83_state1").style.visibility = "visible";

        $.ajax({
            url: getReportsAndEmbedUrl,
            //data: { roleid: $('#txtSearch').val() },
            data: { requestId: tr.find('input[name="RequestID"]').val() },
            type: "GET",
            dataType: "json",
            success: function (data) {
                loadData(data);
            },
            error: function () {
                alert("Failed to get request details! Please try again.");
            }
        });

    });

    function loadData(data) {
        var tab = $('<table class="tblReports"></table>');
        $.each(data.Reports, function (i, report) {
            var trow = $('<tr></tr>');
            trow.append('<td style="display:none;">' + '<input type="text" value="' + report.ReportId + '" name="ReportId" />' + '</td>');
            trow.append('<td>' + '<button type="button" id="btnViewReport" class="btn btn-link btnViewReport">' + report.ReportName + '</button>' + '</td>');
            trow.append('<td style="display:none;">' + '<input type="text" value="' + report.ReportEmbedUrl + '" name="ReportEmbedUrl" />' + '</td>');
            trow.append('<td style="display:none;">' + '<input type="text" value="' + data.PowerBIAccessToken + '" name="PowerBIAccessToken" />' + '</td>');
            tab.append(trow);
        });
        $("#u94").html(tab);        
    };

    $(document).on("click", ".btnViewReport", function () {    

        var tr = $(this).closest('tr');
        reportId = tr.find('input[name="ReportId"]').val();
        reportEmbedUrl = tr.find('input[name="ReportEmbedUrl"]').val();
        powerBIAccessToken = tr.find('input[name="PowerBIAccessToken"]').val();
        document.getElementById("spnReportName").innerHTML = workspaceName + " / Reports";
        
        document.getElementById("u83_state1").style.visibility = "hidden";
        document.getElementById("u83_state2").style.visibility = "visible"; 
        if (reportEmbedUrl != "null") {
            //$('#ifrmReport').attr('src', reportEmbedUrl);
            var config = {
                type: 'report',
                accessToken: powerBIAccessToken,
                embedUrl: reportEmbedUrl
            };

            // Grab the reference to the div HTML element that will host the report.
            var reportContainer = document.getElementById('reportContainer');

            // Embed the report and display it within the div container.
            var report = powerbi.embed(reportContainer, config);
        }
    });
    
});