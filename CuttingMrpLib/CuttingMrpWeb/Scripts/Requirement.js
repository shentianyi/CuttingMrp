var Requirement = {};

Requirement.init = function () {
    var partNr = $('#PartNr').val();
    var ordereddatefrom = $('#OrderedDateFrom').val();
    var ordereddateto = $('#OrderedDateTo').val();
    var requiredtimefrom = $('#RequiredTimeFrom').val();
    var requiredtimeto = $('#RequiredTimeTo').val();
    var quantityfrom = $('#QuantityFrom').val() > 0 ? $('#QuantityFrom').val() : "";
    var quantityto = $('#QuantityTo').val() > 0 ? $('#QuantityTo').val() : "";
    var status = $('#Status').val();
    var derivedfrom = $('#DerivedFrom').val();

    Requirement.add_string_label_to_div(partNr, 'PartNr Like ', '.filter-p');
    Requirement.add_range_label_to_div(ordereddatefrom + "~" + ordereddateto, 'OrderedDate ', '.filter-p');
    Requirement.add_range_label_to_div(requiredtimefrom + "~" + requiredtimeto, 'RequiredDate ', '.filter-p');
    Requirement.add_range_label_to_div(quantityfrom + "~" + quantityto, 'Quantity ', '.filter-p');
    Requirement.add_string_label_to_div(derivedfrom, 'DerivedFrom Like', '.filter-p');
    Requirement.add_string_label_to_div(status, 'Status =', '.filter-p');
}

Requirement.click_filter = function () {
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

Requirement.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

Requirement.add_range_label_to_div = function (content, name, cls) {
    var from = content.split("~")[0];
    var to = content.split("~")[1];
    if ((from != "" && from != null) && (to != null && to != "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + " : " + from + "~" + to + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from != "" && from != null) && (to == null || to == "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + ">=" + from + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from == "" || from == null) && (to != null && to != "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + "<=" + to + "</p>").appendTo(cls).ready(function () {
        });
    }
}

$('.datetime-picker').datetimepicker({
    lang: 'ch'
})

//{
//    "OrderType":"FIX",
//      "MergeMethod":"DAY"
//}

//post /Requirements/RunMrp

Requirement.run_mrp = function () {
    $('.runMrp').click(function () {
        $('#ProcessOrderMask').fadeIn(0);
        $('#ProcessOrder').fadeIn(400);

        $('.remove-process-order').click(function () {
            $('#ProcessOrderMask').fadeOut(400);
            $('#ProcessOrder').fadeOut(400);
            //是否需要手动刷新？  
            //如手动刷新，注释此行
            //如自动刷新，此行去注释
            //window.location.reload();
        });

        $('.confirm-process-order').click(function () {
            $.ajax({
                url: "/Requirements/RunMrp",
                type: "post",
                data:{
                    "OrderType": "FIX",
                    "MergeMethod":"DAY"
                },
                success: function (data) {
                    //{"Result":true,"Msg":"MRP 任务运行成功!"}
                    //{"Result":false,"Msg":"队列中已经有待运行的任务，请稍后再试"}
                    if (data.Result) {
                        ShowMsg("生成成功--", "glyphicon glyphicon-ok-circle", "green", data.Msg);
                    } else if (!data.Result) {
                        ShowMsg("生成失败--", "glyphicon glyphicon-exclamation-sign", "orange", data.Msg);
                    } else {
                        ShowMsg("错误--", "glyphicon glyphicon-remove-circle", "#ff0000", data.Msg);
                    }
                },
                error: function (data) {
                    ShowMsg("错误--", "glyphicon glyphicon-remove-circle", "#ff0000", "无法请求到服务，请检查确认之后再运行。");
                }
            });
        });

    });

    //Change Icon
    function ShowMsg(titleMsg, iconClass , fontColor,contentMsg) {
        $('#ProcessOrder').children('h3').html(titleMsg + new Date().toLocaleString()).css({ color: fontColor });
        $('hr').remove();
        var PanelDiv = $('#ProcessOrder').children('div');
        $('#ProcessOrder').find(PanelDiv).remove();

        $("<hr/><div class='col-sm-12' style='text-align:center;'>" +
            "<i class='" + iconClass + "' style='font-size:9em;color:" + fontColor + "'></i>" +
            "<br/><br/><div class='col-sm-12'>" +
            "<label style='text-align:center;color:"+fontColor+";font-size:1em;'>"+contentMsg+"</label>" +
            "</div></div>").appendTo($('#ProcessOrder'));
    }
}