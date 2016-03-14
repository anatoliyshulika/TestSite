function DrawMinMaxTable(min, pathMin, max, pathMax) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Result');
    data.addColumn('string', 'Source');
    data.addColumn('number', 'Response time (ms)');
    data.addRows([
        ["Min resp. time", pathMin, { f: min + " ms" }],
        ["Max resp. time", pathMax, { f: max + " ms" }]
    ]);
    var table = new google.visualization.Table(document.getElementById("minMaxTable"));
    $("#minMaxTable").show();
    table.draw(data, { showRowNumber: true, width: 1100, height: "100%" });
}
function DrawTable(responseTable) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Source');
    data.addColumn('number', 'Response time (ms)');
    data.addRows(responseTable);
    var table = new google.visualization.Table(document.getElementById("siteTable"));
    $("#siteTable").show();
    table.draw(data, { showRowNumber: true, width: 1100, height: "100%" });
}
function Draw(response) {
    var wi = 35 * response.length - 1;
    var data = new google.visualization.arrayToDataTable(response);
    var options = {
        width: 1100,
        height: wi,
        bar: { groupWidth: "80%" },
        legend: { position: "none" },
        annotations: { alwaysOutside: "true" },
        backgroundColor: {
            strokeWidth: 4,
        },
        chartArea: {
            left: 250,
            top: 20,
            bottom: 20,
        }
    };
    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1,
        {
            calc: "stringify",
            sourceColumn: 1,
            type: "string",
            role: "annotation",
        }, 2
    ]);
    var grafic = new google.visualization.BarChart(document.getElementById("chart"));
    $("#chart").show();
    grafic.draw(view, options);
};

function TestSite(path) {
    StartLoadIndication();
    $.ajax({
        type: "POST",
        url: "/home/Post/",
        data: { path: path },
        dataType: "json",
        success: function (data) {
            StopLoadIndication();
            if (data != null) {
                var response = [["Element", "Response time", { role: "style" }]];
                var k = 0;
                for (var i = 0, j = 1; i < data.length; i+=2, j+=2) {
                    k = parseInt(data[i]);
                    response.push([data[j], k, 'stroke-color: gold; stroke-width: 4; fill-color: green']);
                }
                Draw(response);
                var responseTable = [];
                for (var i = 0, j = 1; i < data.length; i+=2, j+=2) {
                    k = parseInt(data[i]);
                    responseTable.push([data[j], { f: k + " ms" }]);
                }
                DrawTable(responseTable);
                var min = 0;
                var pathMin;
                var max = 0;
                var pathMax;
                for (var i = 0, j = 1; i < data.length; i += 2, j += 2) {
                    if (0 == i) {
                        min = parseInt(data[i]);
                        pathMin = data[j];
                        max = parseInt(data[i]);
                        pathMax = data[j];
                    }
                    else {
                        if (min > parseInt(data[i])) {
                            min = parseInt(data[i]);
                            pathMin = data[j];
                        }
                        if (max < parseInt(data[i])) {
                            max = parseInt(data[i]);
                            pathMax = data[j];
                        }
                    }
                }
                DrawMinMaxTable(min, pathMin, max, pathMax);
            }
            else {
                StopLoadIndication();
                alert("Url not valid!!! Enter please correct url. For example: https://vk.com or https://www.facebook.com");
                return;
            }
        },
        error: function () {
            StopLoadIndication();
            alert("Something went wrong, please try again later.");
        }
    })
}
function StartLoadIndication() {
    var showimg = $("#loadimg").show();
}
function StopLoadIndication() {
    $("#loadimg").hide();
}
function ShowTop() {
    $.ajax({
        type: "GET",
        url: "/home/TableSitesView/",
        data: {},
        dataType: "html",
        success: function (data) {
            $("#chart").hide();
            $("#minMaxTable").hide();
            $("#siteTable").html(data);
        }
    });
}
function ClearDb() {
    $.ajax({
        type: "GET",
        url: "/home/ClearDb/",
        dataType: "text",
        success: function (data) {
            $("#siteTable").html("");
            alert(data);
        },
        error: function () {
            alert("The data could not be deleted, please try again later.");
        }
    });
}