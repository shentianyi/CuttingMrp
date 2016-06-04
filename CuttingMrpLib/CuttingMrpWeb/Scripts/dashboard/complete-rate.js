var CompleteRate = {};

CompleteRate.Init = function () {
    var now = new Date().Format("yyyy-MM-dd");
    var WeekAgo = FutureDate(14);
    $('.date-from').val(WeekAgo);
    $('.date-to').val(now);

    $('.dashboard-left-nav li').removeClass("dashboard-nav-active");
    $('.dashboard-rate').addClass("dashboard-nav-active dashboard-rate");
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
                    display: 'partNr'
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
                CompleteRate.DrawCharts(PartNr, data);
            },
            error: function () {
                console.log("Error");
            }
        })
    })
}

CompleteRate.ChangeRate = function (rate) {
    if (rate == 0) {
        return 0;
    } else if ((rate + "").indexOf(".") == -1) {
        return rate;
    } else {
        var TmpRate = rate * 100 + "";
        var TmpRateDec = TmpRate.indexOf(".");
        var Rate = TmpRate.substring(0, TmpRateDec + 3);
        return Rate;
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
        XValue.push(PartNrData[i].XValue.split(" ")[0] + "#" +PartNrData[i].YValue);
        YValue.push(PartNrData[i].YValueRate);
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
            title: { text: 'Rate(%)' },
            plotLines: [{ value: 0, width: 1, color: '#808080' }]
        },
        tooltip: {
            formatter: function () {
                return '<span><b>' + this.x.split('#')[0] + '</b><br/>' +
                    '<b>Rate: </b><b>' + this.y + '</b>' +
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
            column: {
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    style: {
                        fontWeight: "bold",
                    }
                }
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