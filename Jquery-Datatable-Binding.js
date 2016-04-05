var GetData = function (Url, table, aryColTableChecked, col) {
    var aryJSONColTable = [];
    var j = 0;
    for (var i = 0; i < aryColTableChecked.length; i++) {
        j = j + 1;
        if (col == 1) {
            if (i == 0) {
                aryJSONColTable.push({
                    "sTitle": aryColTableChecked[i],
                    "aTargets": [i],
                    "visible": false,
                    "sortable": false,
                    "searchable": false
                });
            }
            else {
                aryJSONColTable.push({
                    "sTitle": aryColTableChecked[i],
                    "aTargets": [i],

                });
            }
        }
        else if (col == 2) {
            if (i == 0 || i == 1) {
                aryJSONColTable.push({
                    "sTitle": aryColTableChecked[i],
                    "aTargets": [i],
                    "visible": false,
                    "sortable": false,
                    "searchable": false
                });
            }
            else {
                aryJSONColTable.push({
                    "sTitle": aryColTableChecked[i],
                    "aTargets": [i],

                });
            }
        }


    };
    aryJSONColTable.push({
        "sTitle": "",
        "data": null,
        "className": "center",
        "sortable": false,
        "defaultContent": '<a href="#" data=toggle="modal" class="glyphicon glyphicon-pencil" >Edit</button> / <a href="#small" data-toggle="modal" class="glyphicon glyphicon-remove">Delete</a>',
        "aTargets": [j]
    });
    $(table).DataTable({
        "aoColumnDefs": aryJSONColTable,
        "sDom": '<"clear">frtip',
        //serverSide: true,

        "bProcessing": false,
        "bServerSide": false,
        "bLengthChange": true,
        "bFilter": true,
        "aaSorting": [[1, "desc"]],
        "sScrollX": "100%",
        "bScrollCollapse": true,
        "bJQueryUI": true,
        "bDestroy": true,
        tableTools: {
            sSwfPath: "../Admin/plugins/datatables/extensions/TableTools/swf/copy_csv_xls_pdf.swf",
            "aButtons": [

                            "csv",
                            "pdf"
            ]
        },
        "sAjaxSource": Url,
        "bJQueryUI": true,
        "bDeferRender": true,
        "fnServerParams": function (aoData) {

        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.ajax({

                "dataType": 'json',
                "contentType": "application/json; charset=utf-8",
                "type": "Post",
                "url": sSource,
                "data": aoData,
                "success":
                        function (msg) {
                            var json = jQuery.parseJSON(msg);
                            fnCallback(json);

                        },
                "rowCallback": function (row, data) {
                    if ($.inArray(data.DT_RowId, selected) !== -1) {
                        $(row).addClass('selected');
                    }
                },
                beforeSend: function () {
                    $('.loader').show()
                },
                complete: function () {
                    $('.loader').hide()

                }
            });
        }

    });



}

