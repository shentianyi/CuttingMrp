var Stock = {};

Stock.init = function () {
    var partnr = $('#PartNr').val();
    var partnract = $('#PartNrAct').val();
    var fifofrom = $('#FIFOFrom').val();
    var fifoto = $('#FIFOTo').val();
    var quantityfrom = $('#QuantityFrom').val() > 0 ? $('#QuantityFrom').val() : "";
    var quantityto = $('#QuantityTo').val() > 0 ? $('#QuantityTo').val() : "";
    var wh = $('#Wh').val();
    var position = $('#Position').val();

    Stock.add_string_label_to_div(partnr, 'PartNr Like ', '.filter-p');
    Stock.add_string_label_to_div(partnract, 'PartNrAct = ', '.filter-p');
    Stock.add_range_label_to_div(fifofrom +"~"+ fifoto, 'FIFO ', '.filter-p');
    Stock.add_range_label_to_div(quantityfrom +"~"+ quantityto, 'Quantity ', '.filter-p');
    Stock.add_string_label_to_div(wh, 'Wh = ', '.filter-p');
    Stock.add_string_label_to_div(position, 'Position Like', '.filter-p');
}

Stock.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

Stock.add_range_label_to_div = function (content,name, cls) {
    var from = content.split("~")[0];
    var to = content.split("~")[1];
    if ((from != "" && from != null)&&(to!=null && to!="")) {
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

Stock.click_filter = function () {
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

Stock.import_stock = function () {
    $('.import-stock').click(function () {
        $('#dialog_content').dialogModal({
            onOkBut: function () {
            },
            onCancelBut: function () { },
            onLoad: function () { },
            onClose: function () { },
        });
    });
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

window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-stocks').addClass("nav-choosed");
}