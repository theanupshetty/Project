var GenerateTable = function (url, table, columnHeaders, sortableHeaders, isEditable = false,
    isDeletable = false, isCloneable = false, isRespondable = false, isApproveable = false) {

    var columns = [];
    var j = 0;
    for (var i = 0; i < columnHeaders.length; i++) {
        j = j + 1;
        if (i == 0) {
            columns.push({
                "sTitle": columnHeaders[i],
                "aTargets": [i],
                "visible": false,
                "sortable": false,
                "searchable": false
            });

        }
        else {
            columns.push({
                "sTitle": columnHeaders[i],
                "name": sortableHeaders[i],
                "aTargets": [i],
                "sortable": true,
                "searchable": true
            });
        }
    };
    if (isEditable) {
        columns.push({
            "sTitle": "",
            "data": null,
            "sortable": false,
            "searchable": false,

            "defaultContent": '<a class= "btn-app" data-toggle = "modal"><i class= "fa fa-edit edit" title="Edit"></i></a>',
            "aTargets": [j]
        });
    }
    if (isRespondable) {
        if (isEditable) {
            j = j + 1;
        }
        columns.push({
            "sTitle": "",
            "data": null,
            "sortable": false,
            "searchable": false,
            "defaultContent": '<a class= "btn-app" data-toggle = "modal"><i class= "fa fa-reply respond" title="Respond"></i></a>',
            "aTargets": [j]
        });
    }
    if (isCloneable) {
        if (isEditable || isRespondable) {
            j = j + 1;
        }

        columns.push({
            "sTitle": "",
            "data": null,
            "sortable": false,
            "searchable": false,
            "defaultContent": '<a class= "btn-app" data-toggle = "modal"><i class= "fa fa-clone clone" title="Clone"></i></a>',
            "aTargets": [j]
        });
    }
    if (isApproveable) {
        if (isEditable || isRespondable || isCloneable) {
            j = j + 1;
        }

        columns.push({
            "sTitle": "",
            "data": null,
            "sortable": false,
            "searchable": false,
            "defaultContent": '<a class= "btn-app" data-toggle = "modal"><i class= "fa fa-check approve" title="Approve"></i></a>',
            "aTargets": [j]
        });
    }
    if (isDeletable) {
        if (isEditable || isRespondable || isCloneable || isApproveable) {
            j = j + 1;
        }
        columns.push({
            "sTitle": "",
            "data": null,
            "sortable": false,
            "searchable": false,
            "defaultContent": '<a class= "btn-app" data-toggle = "modal"><i class= "fa fa-trash delete" title="Delete"></i></a>',
            "aTargets": [j]
        });
    }


    $('#' + table).dataTable({
        "bDestroy": true,
        "aoColumnDefs": columns,
        "sDom": '<"clear">lfrtip',
        "ordering": true,
        "filter": true,
        "paging": true,
        "info": true,
        "lengthMenu": [[10, 50, 100, 200, 400, 800, 1000000000], [10, 50, 100, 200, 400, 800, "All"]],
        "lengthChange": true,
        "sLoadingRecords": "Loading...",
        "sProcessing": "Processing...",
        "searching": true,
        "scrollY": "500px",
        "scrollYInner": true,
        "sScrollX": "100%",
        "scrollCollapse": true,
        "order": [[i - 1, "desc"]],
        "bProcessing": true,
        "bServerSide": true,
        "language":
        {
            "processing": "<i class='fa fa-refresh fa-spin'></i>",
            "loadingrecords": "<i class='fa fa-refresh fa-spin'></i>",
        },

        "ajax": {
            "url": url,
            "type": "Post",
            "datatype": "json",
            "data": function (d) {

                //d.Id = id;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.modal').modal('hide');
                $('#modal-login').modal('show');
            }

        },

        "bDeferRender": true,
 	mark: true

    });
}