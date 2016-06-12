var StockReport = {};

StockReport.Init = function () {
    $('.dashboard-left-nav li').removeClass("dashboard-nav-active");
    $('.dashboard-report').addClass("dashboard-nav-active dashboard-report");
    StockReport.SetTableWidth();
    window.onresize = function () {
        StockReport.SetTableWidth();
    }
}

StockReport.SetTableWidth= function () {
    var FixedWidth = $(window).width() / 5 > 300 ? $(window).width() / 5 : 300;

    $('.fixed-width').css({
        width: FixedWidth + 'px'
    });
}