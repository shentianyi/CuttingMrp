﻿var Requirement = {};

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
        $('#ProcessOrder').removeAttr('class').addClass('animated bounceIn').fadeIn();

        $('.remove-process-order').click(function () {
            $('#ProcessOrderMask').fadeOut(400);
            $('#ProcessOrder').fadeOut(400);
            $('#ProcessOrder').addClass('bounceOutUp').fadeOut();
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
                    console.log(data);
                    $('#ProcessOrderMask').fadeOut(400);
                    $('#ProcessOrder').fadeOut(400);
                    $('#ProcessOrder').addClass('bounceOutUp').fadeOut();
                },
                error: function () {
                    console.log("Error");
                }
            });
        });

    });
    
}