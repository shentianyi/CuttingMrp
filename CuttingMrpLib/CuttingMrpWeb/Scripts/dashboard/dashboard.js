var Dashboard = {};

window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-dashboard').addClass("nav-choosed");
    //$('.dashboard-left-nav li').removeClass();
    //$('.dashboard-part').addClass('dashboard-nav-active');
}

$('.date-picker').datetimepicker({
    lang: 'ch',
    timepicker: false,
    format: 'Y/m/d',
    formatDate: 'Y/m/d'
})

//Dashboard.Menu = function (id) {
//    switch (id) {
//        case "1":
//            Dashboard.Ajax("Part");
//            break;
//        case "2":
//            break;
//        default:
//            console.log("Hello default ,,,,");
//            break;
//    }
//}

//Dashboard.Ajax = function (page) {
//    console.log(page);
//    $.ajax({
//        url: '/Dashboard/'+page,
//        dataType: "html",
//        type: 'post',
//        success: function (data) {
//            console.log(data);
//            $('.RightContent').html(data);
//            console.log("Success");
//        },
//        error: function () {
//            console.log("Error")
//        }
//    })
//}