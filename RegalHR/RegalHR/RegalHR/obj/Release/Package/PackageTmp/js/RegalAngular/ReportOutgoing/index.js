var AngularApp = angular.module('index', []);

AngularApp.controller('Ctrl', function ($scope) {
   
    $scope.DataList = [];

    $.ajax({
        type: "POST",
        url: "../ReportOutgoing/GetReportList",
        dataType: 'Json',
        data: '',
        async: false,
        success: function (data) {

            var JsonObj = angular.fromJson(data);

            if (JsonObj.Result == "1")
            {
                $scope.DataList = JsonObj.Query;

            } else {
                alert("Error");
            }
        }
    })


    $scope.Link = function (url) {
        document.location.href = url;
    }
});


