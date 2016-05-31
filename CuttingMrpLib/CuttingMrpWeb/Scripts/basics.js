$(function () {
    $('[data-toggle="tooltip"]').tooltip();
})

function check_all_cb() {
    var checked = $(this).attr("current-state") == "1";
    if (checked) {
        $(this).attr("current-state", "0");
        $(this).attr("value", "All");
        $('input:checkbox').removeAttr("checked");
    } else {
        $(this).attr("current-state", "1");
        $(this).attr("value","UnAll");
        $('input:checkbox').prop("checked",true);
    }
}