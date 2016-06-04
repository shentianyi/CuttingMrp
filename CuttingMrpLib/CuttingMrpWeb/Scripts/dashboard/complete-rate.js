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
                CompleteRate.DrawCharts(data);
                console.log("Success");
            },
            error: function () {
                console.log("Error");
            }
        })
    })
}
CompleteRate.DrawCharts = function (data) {
    var ChartStyle = {
        ChartTitle: "Complete Rate"
    }
    
    var XValue = new Array;
    var YValue = new Array;
    for (var i = 0; i < data.length; i++) {
        XValue.push(data[i].XValue.split(" ")[0]);
        YValue.push(data[i].YValue);
    }

    // 图表操作
    var chart_options = {
        chart: {
            renderTo: 'complete_rate_charts'
        },
        credits: {
            enabled: false
        },
        title: {
            text: ChartStyle.ChartTitle, x: -20
        },
        subtitle: {
            //text: '——History Data', x: 20
        },
        xAxis: {
            categories: XValue
            //max: max_count
        },
        yAxis: {
            title: { text: 'Value' },
            plotLines: [{ value: 0, width: 1, color: '#808080' }]
        },
        tooltip: {
            //formatter: function () {
            //    return '<span><b>' + this.point.id + '</b><br/><b>Value:</b><b>' + this.y + ' s</b><span>';
            //}
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
            name: 'Value',
            data: YValue,
            color: 'limegreen'
        }]
    };
    var chart = new Highcharts.Chart(chart_options);
}