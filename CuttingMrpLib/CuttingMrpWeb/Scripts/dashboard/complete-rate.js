var CompleteRate = {};

CompleteRate.Init = function () {
    var now = new Date().Format("yyyy-MM-dd");
    var WeekAgo = FutureDate(14);
    $('.date-from').val(WeekAgo);
    $('.date-to').val(now);

    $('.dashboard-left-nav li').removeClass("dashboard-nav-active");
    $('.dashboard-rate').addClass("dashboard-nav-active");
    CompleteRate.ViewMoreMovments();
}

CompleteRate.InitPartNr = function () {
    $('#part_nr').keydown(function () {
        var part_nr_value = $('#part_nr').val();
        $.ajax({
            url: '/Parts/Fuzzies',
            type: 'get',
            data: {
                id: part_nr_value
            },
            success: function (data) {
                $('#part_nr').typeahead({
                    source: data,
                    display: 'partNr',
                    items: 20
                });
            },
            error: function () {
                console.log("Error");
            }
        })
    })
}

CompleteRate.CompleteRateSearch = function () {
    $('.complete-rate-search').click(function () {
        var DateFrom = $('.date-from').val();
        var DateTo = $('.date-to').val();
        var Type = $('.part-type').val();
        var PartNr = $('.part-nr').val();

        if (DateFrom == null || DateFrom == "") {
            $('.date-from').css({
                borderColor: "#ff0000"
            });
            $('.date-from').val(FutureDate(7));
        } else {
            $('.date-from').css({
                borderColor: ""
            });
        }

        if (DateTo == null || DateTo == "") {
            $('.date-to').css({
                borderColor: "#ff0000"
            });
            $('.date-to').val(new Date().Format("yyyy/MM/dd"));
        } else {
            $('.date-to').css({
                borderColor: ""
            });
        }

        if (PartNr == null || PartNr == "") {
            $('.part-nr').css({
                borderColor: "#ff0000"
            });
            $('.part-nr').attr("placeholder", "Part Nr Cannot Empty.");
            $('.CompleteRateEmpty').css({
                left: "25%"
            });
            $('.CompleteRateEmpty').html("Part Nr Can't Empty");
            return;
        } else {
            $('.part-nr').css({
                borderColor: ""
            });
        }

        $.ajax({
            url: '/Dashboard/Data',
            type: 'get',
            data: {
                Type: Type,
                PartNr: PartNr,
                DateFrom: DateFrom,
                DateTo: DateTo
            },
            success: function (data) {
                $('.move-tbody').empty();
                $('.details-tbody').empty();
                $('.hidden-part-nr').html(PartNr);
                $('.hidden-date-from').html(DateFrom);
                $('.hidden-date-to').html(DateTo);
                $('.CompleteRateEmpty').css({
                    display: 'none'
                });
                CompleteRate.DrawCharts(PartNr, data);
                //请求第一页
                CompleteRate.StockMovements(PartNr, 1, DateFrom, DateTo);
                CompleteRate.DetailsShow(PartNr);
            },
            error: function () {
                console.log("Error");
            }
        })
    })
}

CompleteRate.StockMovements = function (PartNr, Page, DateFrom, DateTo) {
    $.ajax({
        url: '/StockMovements/JsonSearch',
        type: 'get',
        data: {
            PartNr: PartNr,
            page: Page,
            DateFrom: DateFrom,
            DateTo: DateTo
        },
        success: function (data) {
            $('.Movements').css({ display: "block" });
            if (data.length > 0) {
                $('.view-more-movements').css({
                    display: 'block'
                });

                $('.Movements').css({
                    height: "400px"
                })
                $('.move-thead').css({
                    display: ''
                });

                if ($('.move-tbody').find("h4").length > 0) {
                    $('.move-tbody').empty();
                }

                for (var i = 0 ; i < data.length; i++) {
                    var ID = data[i].id;
                    var PartNr = data[i].partNr;
                    var Qty = data[i].quantity;
                    var TypeDisplay = data[i].typeDisplay;
                    var CreatedAtDisplay = data[i].createdAtDisplay;

                    $("<tr id=" + ID + ">" +
                        "<td>" + PartNr + "</td>" +
                        "<td>" + Qty + "</td>" +
                        "<td>" + TypeDisplay + "</td>" +
                        "<td>" + CreatedAtDisplay + "</td></tr>").appendTo(".move-tbody").ready(function () {
                        });
                }
            } else {
                $('.view-more-movements').css({
                    display: 'none'
                });

                if (Page == 1) {
                    $('.Movements').css({
                        height: "120px"
                    })
                    $('.move-thead').css({
                        display: 'none'
                    });
                    $('.move-tbody').html("<h4 style='color:orange;text-align:center;margin-top:30px;'><i class='glyphicon glyphicon-exclamation-sign'></i> &emsp;No Movements</h4>");
                }
            }
        },
        error: function () {
            console.log("Error");
        }
    })
}

CompleteRate.DetailsShow = function (PartNr) {
    $.ajax({
        url: '/Parts/Parents',
        type: 'get',
        data: {
            id: PartNr
        },
        success: function (data) {
            $('.Details').css({ display: "block" });
            if (data.length > 0) {
                $('.Details').css({
                    height: "400px"
                })
                $('.details-thead').css({
                    display: ''
                });

                $('.details-tbody').empty();

                //partDesc "L-00096G"
                //partNr "L-00096G"
                //partTypeDisplay "Product"
                for (var i = 0 ; i < data.length; i++) {
                    var PartNr = data[i].partNr;
                    var PartDesc = data[i].partDesc;
                    var PartTypeDisplay = data[i].partTypeDisplay;
                    $("<tr><td>" + PartNr + "</td>" +
                        "<td>" + PartDesc + "</td>" +
                        "<td>" + PartTypeDisplay + "</td></tr>").appendTo(".details-tbody").ready(function () {
                        });
                }
            } else {
                $('.Details').css({
                    height: "120px"
                })
                $('.details-thead').css({
                    display: 'none'
                });

                $('.Details').css({
                    height: "120px"
                })

                $('.details-tbody').html("<h4 style='color:orange;text-align:center;margin-top:30px;'><i class='glyphicon glyphicon-exclamation-sign'></i> &emsp;No Part Details</h4>");
            }
        },
        error: function () {
            console.log("Error");
        }
    });
}

CompleteRate.ChangeRate = function (rate) {
    if (rate == 0) {
        return 0;
    } else if ((rate + "").indexOf(".") == -1) {
        return parseFloat(rate);
    } else {
        var TmpRate = rate * 100 + "";
        var TmpRateDec = TmpRate.indexOf(".");
        var Rate = TmpRate.substring(0, TmpRateDec + 3);
        return parseFloat(Rate);
    }
    return 0;
}

CompleteRate.DrawCharts = function (PartNr, data) {
    //{"91C511601925":
    //    [{"XValue":"2016/6/2 0:00:00",
    //        "YValue":100,
    //        "YValueRate":0.5
    //    }]
    //}

    var XValue = new Array;
    var YValue = new Array;
    var PartNrData = data[PartNr];
    for (var i = 0; i < PartNrData.length; i++) {
        XValue.push(PartNrData[i].XValue.split(" ")[0] + "#" + PartNrData[i].YValue);
        YValue.push(CompleteRate.ChangeRate(PartNrData[i].YValueRate));
    }

    var ChartStyle = {
        ChartTitle: PartNr,
        ChartSubTitle:"--Complete Rate"
    }
    
    // 图表操作
    var chart_options = {
        chart: {
            renderTo: 'complete_rate_charts',
            backgroundColor: "transparent"
        },
        credits: {
            enabled: false
        },
        title: {
            text: ChartStyle.ChartTitle, x: -20
        },
        subtitle: {
            text: ChartStyle.ChartSubTitle, x: 20
        },
        xAxis: {
            categories: XValue,
            //max: max_count
            labels: {
                formatter: function () {
                    return this.value.split('#')[0];
                }
            }
        },
        yAxis: {
            labels: {
                format: '{value} %'
            },
            title: { text: 'Rate(%)' },
            plotLines: [{ value: 0, width: 1, color: '#808080' }]
        },
        tooltip: {
            formatter: function () {
                return '<span><b>' + this.x.split('#')[0] + '</b><br/>' +
                    '<b>Rate: </b><b>' + this.y + '%</b>' +
                    "<br /><b>Stock: </b><b>" + this.x.split('#')[1] + '</b><span>';
            }
        },
        scrollbar: {
            enabled: true
        },
        legend: {
            layout: 'horizontal', align: 'center', verticalAlign: 'bottom', borderHeight: 0
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        return '<b>' + this.y + '%</b>';
                    }
                },
                enableMouseTracking: true
            },
            series: {
                stickyTracking: false,
                turboThreshold: 0 //不限制数据点个数
            }
        },
        series: [{
            type:'line',
            name: 'Rate',
            data: YValue
        }]
    };

    var chart = new Highcharts.Chart(chart_options);
    console.log(YValue);
}

CompleteRate.ViewMoreMovments = function () {
    var Page = 1;
    $('.view-more-movements').click(function () {
        Page++;
        var PartNr = $('.hidden-part-nr').html();
        var DateFrom = $('.hidden-date-from').html();
        var DateTo = $('.hidden-date-to').html();
        CompleteRate.StockMovements(PartNr, Page, DateFrom, DateTo);
    })
}