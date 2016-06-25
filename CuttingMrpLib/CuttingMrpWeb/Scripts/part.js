var Part = {}

Part.Init = function () {
    var partnr = $('#PartNr').val();
    var parttype = $('#PartType').children("option:selected").html();
    Part.add_string_label_to_div(partnr, 'PartNr like ', '.filter-p');
    Part.add_string_label_to_div(parttype, 'PartType = ', '.filter-p');
}

Part.click_filter = function () {
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

Part.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

Part.add_range_label_to_div = function (content, name, cls) {
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

Part.import_part_data = function () {
    $('.import-part-data').click(function () {
        $('#dialog_content').dialogModal({
            onOkBut: function () {
            },
            onCancelBut: function () { },
            onLoad: function () { },
            onClose: function () { },
        });
    });
}

Part.import_result = function () {
    var CreateFailureQty = $('#CreateFailureQty').html();
    var UpdateFailureQty = $('#UpdateFailureQty').html();
    var ActionNullQty = $('#ActionNullQty').html();
    var OtherQty = $('#OtherQty').html();

    if (CreateFailureQty == 0) {
        $('.CreateFailureTable').css({display:'none'})
    }

    if (UpdateFailureQty == 0) {
        $('.UpdateFailureTable').css({ display: 'none' })
    }

    if (ActionNullQty == 0) {
        $('.ActionNullTable').css({ display: 'none' })
    }

    if (OtherQty == 0) {
        $('.OtherTable').css({ display: 'none' })
    }
}


window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-basic-data').addClass("nav-choosed");
    $('.nav-part').addClass("nav-choosed");
}