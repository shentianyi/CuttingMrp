var MrpRounds = {};

MrpRounds.init = function () {
    var mrproundid = $('#MrpRoundId').val();
    var timefrom = $('#TimeFrom').val();
    var timeto = $('#TimeTo').val();
    var runningstatus = $("#RunningStatus").children("option:selected").html();
    //var runningstatus = $('#RunningStatus').val();
    var launcher = $('#Launcher').val();

    MrpRounds.add_string_label_to_div(mrproundid, 'MrpRoundID Like ', '.filter-p');
    MrpRounds.add_range_label_to_div(timefrom + "~" + timeto, 'Time ', '.filter-p');
    MrpRounds.add_string_label_to_div(runningstatus, 'Status =', '.filter-p');
    MrpRounds.add_string_label_to_div(launcher, 'Launcher Like', '.filter-p');
}

MrpRounds.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;white-space:normal;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

MrpRounds.add_range_label_to_div = function (content, name, cls) {
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
    $('.nav-mrp-round').addClass("nav-choosed");
}