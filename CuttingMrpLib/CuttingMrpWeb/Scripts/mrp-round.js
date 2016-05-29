var MrpRounds = {};

MrpRounds.init = function () {
    window.onload = function () {
        $('.navbar-nav li').removeClass("nav-choosed");
        $('.nav-mrp-round').addClass("nav-choosed");
    }

    var mrproundid = $('#MrpRoundId').val();
    var timefrom = $('#TimeFrom').val();
    var timeto = $('#TimeTo').val();
    var runningstatus = $('#RunningStatus').val();
    var launcher = $('#Launcher').val();

    MrpRounds.add_string_label_to_div(mrproundid, 'MrpRoundID Like ', '.filter-p');
    MrpRounds.add_range_label_to_div(timefrom + "~" + timeto, 'Time ', '.filter-p');
    MrpRounds.add_string_label_to_div(runningstatus, 'Status Like', '.filter-p');
    MrpRounds.add_string_label_to_div(launcher, 'Launcher Like', '.filter-p');
}

MrpRounds.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

MrpRounds.add_range_label_to_div = function (content, name, cls) {
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

MrpRounds.click_filter = function () {
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

$('.datetime-picker').datetimepicker({
    lang: 'ch'
})