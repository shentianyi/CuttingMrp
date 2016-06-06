var TopRate = {};

TopRate.Init = function () {
    $('.dashboard-left-nav li').removeClass("dashboard-nav-active");
    $('.dashboard-top-rate').addClass("dashboard-nav-active dashboard-top-rate");
    TopRate.InitCharts();
}

TopRate.ChangeRate = function (rate) {
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

TopRate.InitCharts = function () {
    $.ajax({
        url: '/Dashboard/Data',
        type: 'get',
        data: {
            Type: 300,
            Top: 5
        },
        success: function (data) {
            var XValue = new Array;
            var YValueRate = new Array;
            var Series = new Array;
            var count = 0;
            $.each(data, function (key, value) {
                for (var i = 0; i < value.length; i++) {
                    if (count == 0) {
                        XValue.push(value[i].XValue.split(" ")[0]);
                    }
                    YValueRate.push({ id: value[i].YValue, y: TopRate.ChangeRate(value[i].YValueRate) });
                }

                var TmpSeries = {
                    name: key,
                    data: YValueRate
                }
                Series.push(TmpSeries);
                count++;
                YValueRate = [];
            });
            if (XValue.length > 0) {
                TopRate.DrawCharts(XValue, Series);
            } else {
                $('#top-rate-charts').html("<div style='position:absolute;top:45%;left:30%;font-size:5em;color:#999999;'>Noting to Show ...</div>");
            }
        },
        error: function () {
            console.log("Error");
        }
    })
}

TopRate.DrawCharts = function (XValue, Series) {
    var ChartStyle = {
        ChartTitle: "Top Rate",
        ChartSubTitle: "--Top 5"
    }

    // 图表操作
    var chart_options = {
        chart: {
            renderTo: 'top-rate-charts',
            //backgroundColor: {
            //    linearGradient: { x1: 0, y1: 0, x2: 1, y2: 1 }, stops: [[0, '#6495ED'], [1, 'rgb(48, 48, 96)']]
            //}
            backgroundColor: "transparent",
            marginRight: '10'
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
                    return this.value;
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
                return '<span><b>' + this.x + '</b><br/>' +
                    '<b>Stock: </b><b>' + this.point.id + '</b>' +
                    "<br /><b>Rate: </b><b>" + this.y + '%</b><span>';
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
        series: Series
    };
    var chart = new Highcharts.Chart(chart_options);
}