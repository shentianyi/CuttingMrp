var Requirement = {};

Requirement.init = function () {
    var partNr = $('#PartNr').val();
    var ordereddatefrom = $('#OrderedDateFrom').val();
    var ordereddateto = $('#OrderedDateTo').val();
    var requiredtimefrom = $('#RequiredTimeFrom').val();
    var requiredtimeto = $('#RequiredTimeTo').val();
    var quantityfrom = $('#QuantityFrom').val() > 0 ? $('#QuantityFrom').val() : "";
    var quantityto = $('#QuantityTo').val() > 0 ? $('#QuantityTo').val() : "";
    var status = $("#Status").children("option:selected").html();
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
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

Requirement.add_range_label_to_div = function (content, name, cls) {
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

$('.datetime-picker-from').datetimepicker({
    lang: 'ch',
    timepicker: false,
    format: 'Y/m/d H:i',
    formatDate: 'Y/m/d',
    formatTime: 'H:i',
    defaultTime: '00:00'
})

$('.datetime-picker-to').datetimepicker({
    lang: 'ch',
    timepicker: false,
    format: 'Y/m/d H:i',
    formatDate: 'Y/m/d',
    formatTime: 'H:i',
    defaultTime: '23:59'
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

        $("input[name='MergeMethodType']").change(function () {
            $('.choosed-merge-method-type').html($(this).val());
        });

        $('.remove-process-order').click(function () {
            $('#ProcessOrderMask').fadeOut(400);
            $('#ProcessOrder').fadeOut(400);
            window.location.reload();
        });

        $('.confirm-process-order').click(function () {
            var OrderType = $("input[name='OrderTypes']:checked").val();
            var MergeMethodType = $("input[name='MergeMethodType']:checked").val();
            if ($("#FirstDay").val().length == 0) {
                alert("please select datetime!");
                return;
            }
            $.ajax({
                url: "/Requirements/RunMrp",
                type: "post",
                data:{
                    "OrderType": OrderType,
                    "MergeType": MergeMethodType,
                    "FirstDay": $("#FirstDay").val(),
                    "Count": $("#Count").val()
                },
                success: function (data) {
                    if (data.Result) {
                        ShowMsg("Success", "glyphicon glyphicon-ok-circle", "green", data.Msg);
                    } else if (!data.Result) {
                        ShowMsg("Failure", "glyphicon glyphicon-exclamation-sign", "orange", data.Msg);
                    } else {
                        ShowMsg("Error", "glyphicon glyphicon-remove-circle", "#ff0000", data.Msg);
                    }
                },
                error: function (data) {
                    ShowMsg("Error", "glyphicon glyphicon-remove-circle", "#ff0000", "Unable to request to the service, please check before operation.");
                }
            });
        });

    });

    //Change Icon
    function ShowMsg(titleMsg, iconClass , fontColor,contentMsg) {
        $('#ProcessOrder').children('h3').html(titleMsg + " -- " + new Date().Format("yyyy-MM-dd hh:mm:ss")).css({ color: fontColor });
        $('hr').remove();
        var PanelDiv = $('#ProcessOrder').children('div');
        $('#ProcessOrder').find(PanelDiv).remove();

        $("<hr/><div class='col-sm-12' style='text-align:center;'>" +
            "<i class='" + iconClass + "' style='font-size:9em;color:" + fontColor + "'></i>" +
            "<br/><br/><div class='col-sm-12'>" +
            "<label style='text-align:center; color:"+fontColor+";font-size:1.5em;'>"+contentMsg+"</label>" +
            "</div></div>").appendTo($('#ProcessOrder'));
    }
}

window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-requirements').addClass("nav-choosed");
}
