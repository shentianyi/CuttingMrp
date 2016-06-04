var TopRate = {};

TopRate.Init = function () {
    $('.dashboard-left-nav li').removeClass("dashboard-nav-active");
    $('.dashboard-top-rate').addClass("dashboard-nav-active dashboard-top-rate");
    TopRate.InitCharts();
}

TopRate.InitCharts = function () {
    $.ajax({
        url: '/Dashboard/Data',
        type: 'get',
        data: {
            Type: 300,
            Top:5
        },
        success: function (data) {
            var XValue = new Array;
            var YValueRate = new Array;
            var Series = new Array;
            var count=0;
            $.each(data, function (key, value) {
                for (var i = 0; i < value.length; i++) {
                    if (count == 0) {
                        XValue.push(value[i].XValue.split(" ")[0]);
                    }
                    YValueRate.push(value[i].YValueRate);
                }

                var TmpSeries = {
                    name: key,
                    data: YValueRate
                }

                Series.push(TmpSeries);
                count++;
                YValueRate = [];
            })

            TopRate.DrawCharts(XValue, Series);
        },
        error: function () {
            console.log("Error");
        }
    })
}

TopRate.DrawCharts = function (XValue, Series) {
    var ChartStyle = {
        ChartTitle: "Top Rate",
        ChartSubTitle: "--Complete Rate"
    }

    // 图表操作
    var chart_options = {
        chart: {
            renderTo: 'top-rate-charts',
            //backgroundColor: {
            //    linearGradient: { x1: 0, y1: 0, x2: 1, y2: 1 }, stops: [[0, '#6495ED'], [1, 'rgb(48, 48, 96)']]
            //}
            backgroundColor:"white"
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
            title: { text: 'Value' },
            plotLines: [{ value: 0, width: 1, color: '#808080' }]
        },
        tooltip: {
            //formatter: function () {
            //    return '<span><b>' + this.x.split('#')[0] + '</b><br/>' +
            //        '<b>Value: </b><b>' + this.y + '</b>' +
            //        "<br /><b>Rate: </b><b>" + this.x.split('#')[1] + '</b><span>';
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
        series:Series
    };
    var chart = new Highcharts.Chart(chart_options);
}