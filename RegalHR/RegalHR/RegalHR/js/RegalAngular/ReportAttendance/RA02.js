var AngularApp = angular.module('RA02', []);

AngularApp.controller('Ctrl', function ($scope) {

    $scope.ReportForm = angular.fromJson(ReportForm);
    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.DepartMentList = angular.fromJson(DepartMentList);

    $scope.EmpStatusList = angular.fromJson(EmpStatusList);

    $scope.GetEmpList = function () {

        $.ajax({
            type: "POST",
            url: "../Emp/GetEmpList",
            dataType: 'Json',
            data: 'FulltimeFlag=&company=' + $scope.ReportForm.Company + "&DepartMentNo=" + $scope.ReportForm.DepartMentNo + "&status=&Sex=",
            async: false,
            success: function (data) {


                $scope.EmpDropDownList = data;
                $scope.ReportForm.EmpNo = "";
            }
        });
    }
    $scope.GetEmpList();


    $scope.BackBtn = function () {
        location.href = "../ReportAttendance/";
    }






    //輸出excel
    $scope.ExportExcel = function () {


        $.ajax({
            type: "POST",
            url: "../ReportAttendance/GetRA02ToExcel",
            contentType: "application/json; charset=utf-8",
            data: angular.toJson($scope.ReportForm),
            async: false,
            success: function (data) {

                var JsonObj = angular.fromJson(data);

                if (JsonObj.Result == "1") {


                    location.href = JsonObj.Query.Url;
                } else {
                    SysShowAlert("Excel匯出失敗!" + JsonObj.ErrorMsg, "alert-danger");
                }
            }
        })



    }





});


