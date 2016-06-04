var Dashboard = {};

window.onload = function () {
    $('.navbar-nav li').removeClass("nav-choosed");
    $('.nav-dashboard').addClass("nav-choosed");
}

$('.date-picker').datetimepicker({
    lang: 'ch',
    timepicker: false,
    format: 'Y/m/d',
    formatDate: 'Y/m/d'
})