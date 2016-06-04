var ProcessOrders = {};

ProcessOrders.init = function () {
    var ordernr= $('#OrderNr').val();
    var kanbans = $('#KanbanNr').val();
    var derivedfrom = $('#DerivedFrom').val();
    var proceedatefrom = $('#ProceeDateFrom').val();
    var proceedateto = $('#ProceeDateTo').val();
    var partnr = $('#PartNr').val();
    var actualquantityfrom = $('#ActualQuantityFrom').val() > 0 ? $('#ActualQuantityFrom').val() : "";
    var actualquantityto = $('#ActualQuantityTo').val() > 0 ? $('#ActualQuantityTo').val() : "";
    var completeratefrom = $('#CompleteRateFrom').val();
    var completerateto = $('#CompleteRateTo').val();
    var status = $("#Status").children("option:selected").html();
    var mrpround = $("#MrpRound").children("option:selected").html();
    var kanbanstype = $("#PartType").children("option:selected").html();

    ProcessOrders.add_string_label_to_div(ordernr, 'OrderNr like ', '.filter-p');
    ProcessOrders.add_string_label_to_div(kanbans, 'Kanbans like ', '.filter-p');
    ProcessOrders.add_string_label_to_div(derivedfrom, 'DerivedFrom like ', '.filter-p');
    ProcessOrders.add_string_label_to_div(partnr, 'PartNr like ', '.filter-p');
    ProcessOrders.add_string_label_to_div(status, 'Status =', '.filter-p');
    ProcessOrders.add_string_label_to_div(mrpround, 'MrpRound =', '.filter-p');
    ProcessOrders.add_string_label_to_div(kanbanstype, 'PartType(KB Type) =', '.filter-p');
    ProcessOrders.add_range_label_to_div(proceedatefrom + "~" + proceedateto, 'ProceeDate ', '.filter-p');
    ProcessOrders.add_range_label_to_div(actualquantityfrom + "~" + actualquantityto, 'ActualQuantity ', '.filter-p');
    ProcessOrders.add_range_label_to_div(completeratefrom + "~" + completerateto, 'CompleteRate ', '.filter-p');
}

ProcessOrders.click_filter = function () {
    $('#basic-addon-filter').click(function () {
        $('#basic-addon-filter').popModal({
            html: $('#extra-filter-content'),
            placement: 'bottomRight',
            showCloseBut: false,
            onDocumentClickClose: true,
            onOkBut: function () {
            },
            onCancelBut: function () {
            },
            onLoad: function () {
            },
            onClose: function () {
            }
        })
    });
}

ProcessOrders.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

ProcessOrders.add_range_label_to_div = function (content, name, cls) {
    var from = content.split("~")[0];
    var to = content.split("~")[1];
    if ((from != "" && from != null) && (to != null && to != "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " : " + from + "~" + to + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from != "" && from != null) && (to == null || to == "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + ">=" + from + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from == "" || from == null) && (to != null && to != "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + "<=" + to + "</p>").appendTo(cls).ready(function () {
        });
    }
}

$('.datetime-picker').datetimepicker({
    lang: 'ch'
})

ProcessOrders.finish_porcess_order = function () {
    $('.finish-process-order').click(function () {
        var ids = getCheckedIds();
        if (ids.length > 0 && confirm("Sure to Finish Selected?")) {
            $("#finishOrderIds").val(ids);
            $("#finishForm").submit();
        }
    });
}

ProcessOrders.cancel_process_order = function () {
    $('.cancel-process-order').click(function () {
        var ids = getCheckedIds();
        if (ids.length > 0 && confirm("Sure to Cancel Selected?")) {
            $("#cancelOrderIds").val(ids);
            $("#cancelForm").submit();
        }
    })
}

function getCheckedIds() {
    var ids = [];
    $(".orderNrCheck:checked").each(function () {
        ids.push($(this).val());
    });
    return ids;
}

function check_all_cb() {
    var checked = $(this).attr("current-state") == "1";
    if (checked) {
        $(this).attr("current-state", "0");
        $(this).attr("value", "All");
        $('input:checkbox').removeAttr("checked");
    } else {
        $(this).attr("current-state", "1");
        $(this).attr("value", "UnAll");
        $('input:checkbox').prop("checked", true);
    }
}

ProcessOrders.show_part_nr_msg = function () {
    var AllPartNr = document.getElementsByClassName("partNrMsg");
    for (var i = 0; i < AllPartNr.length; i++) {
        AllPartNr[i].onclick = function (e) {
            $('.popover').remove();
            e.stopPropagation();
            var PartNrMouseOver = $(this).html();
            var NowPartNr = $(this);
            if (NowPartNr.attr("data-content")) {
                console.log(NowPartNr.attr("data-content"));
                $(NowPartNr).popover('show');
            } else {
                $.ajax({
                    url: '/parts/Details/'+PartNrMouseOver,
                    type: 'get',
                    success: function (data) {
                        //{"partNr":"91C540301220",
                        //    "partTypeDisplay":"WhiteCard",
                        //    "partDesc":"91C540301220",
                        //    "moq":100,
                        //    "spq":100,
                        //    "kanbanNr":"419300"
                        //}
                        $(NowPartNr).attr("title", "<strong>" + data.partNr + "</strong>");
                        $(NowPartNr).attr("data-content", "<ul class='part-nr-ul'>" +
                           "<li><label>labelartNr:</label>" + data.partNr + "</li>" +
                           "<li><label>partTypeDisplay:</label>" + data.partTypeDisplay + "</li>" +
                           "<li><label>PartDesc:</label>" + data.partDesc + "</li>" +
                           "<li><label>moq(BundleQty):</label>" + data.moq + "</li>" +
                           "<li><label>spq(BatchQty):</label>" + data.spq + "</li>" +
                           "<li><label>KanBans:</label>" + data.kanbanNr + "</li>" +
                           "</ul>");
                        $(NowPartNr).popover('show');
                    },
                    error: function () {
                        console.log("Something error.")
                    }
                });
            }
        }
        //AllPartNr[i].onmouseout = function () {
        //    var NowPartNr = $(this);
            //$(NowPartNr).popover('hide');
        //}
    }
    
    $('body').click(function () {
        $('.popover').remove();
    });
}

ProcessOrders.import_force_record = function () {
    $('.import-force-record').click(function () {
        $('#dialog_content').dialogModal({
            onOkBut: function () {
            },
            onCancelBut: function () { },
            onLoad: function () { },
            onClose: function () { },
        });
    });
}

ProcessOrders.export_kanbans = function () {
    $('.export-kanbans').click(function () {
        console.log("export kanban")
    });
}

window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-process-orders').addClass("nav-choosed");
}