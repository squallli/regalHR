var AngularApp = angular.module('index', []);


AngularApp.controller('Ctrl', function ($scope) {

    var Today = new Date();

    //$scope.SDATE = Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + "01";
    $scope.SDATE = Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate();
    $scope.EDATE = Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate();
    $scope.AttendanceShowDtl = null;


    $scope.Page = 0; //目前頁數
    $scope.PageTotal = 0; //最終頁
    $scope.RecordLimit = 0; //一頁幾筆
    $scope.PageBegin = 0;
    $scope.SearchData = "";
    $scope.AttendanceList = angular.fromJson(AttendanceList);




    //搜尋
    $scope.Search = function () {
        $scope.AttendanceList = [];



        $scope.SearchData = 'SDATE=' + $scope.SDATE + '&EDATE=' + $scope.EDATE;



        $.ajax({
            type: "POST",
            url: "../Attendance/GetAttendanceList",
            data: $scope.SearchData + '&Skip=0',
            async: false,
            success: function (data) {

                if (data[0] != "0") {
                    $scope.AttendanceList = data;






                    $scope.Page = 0; //目前頁數
                    $scope.PageTotal = 0; //最終頁
                    $scope.RecordLimit = 0; //一頁幾筆
                    $scope.PageBegin = 0;//起始
                    var PaginationList = [];

                    //賦予頁數
                    if ($scope.AttendanceList.length > 0) {


                        var PageTotal = Math.ceil($scope.AttendanceList[0].RowNumTotal / $scope.AttendanceList[0].RecordLimit);

                        
                        for (var i = 0; i < PageTotal; i++) {
                            var Obj = {};
                            Obj.PageNum = i;
                            Obj.Skip = i * $scope.AttendanceList[0].RecordLimit;
                            Obj.PageDisplay = i + 1;

                            PaginationList.push(Obj);
                        }

                        $scope.PageTotal = PageTotal - 1; //最終頁
                        $scope.RecordLimit = $scope.AttendanceList[0].RecordLimit;

                        $scope.PaginationList = PaginationList;

                    }



                } else {
                    var str = data.split("|");

                    SysShowAlert("查詢失敗!" + str[1], "alert-danger");
                }
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        })


    }

    $scope.Search();







    //變換考勤描述顏色
    $scope.AttendanceDescBgColor = function (Desc) {


        var BgColor = "";
        if (Desc.search("曠職") >= 0) {
            BgColor = { "background-color": "#F7D358" };
        }

        return BgColor;
    }



    //更新頁面資料
    $scope.update = function (PageNum) {

        $scope.AttendanceList = [];
        $.ajax({
            type: "POST",
            url: "../Attendance/GetAttendanceList",
            data: $scope.SearchData + '&Skip=' + PageNum * $scope.RecordLimit,
            async: false,
            success: function (data) {

                if (data[0] != "0") {
                    $scope.AttendanceList = data;

                    $scope.Page = PageNum; //目前頁數
                    $scope.PageBegin = $scope.Page - 5;//起始

                    if ($scope.PageBegin < 0)
                        $scope.PageBegin = 0;


                } else {
                    var str = data.split("|");

                    SysShowAlert("查詢失敗!" + str[1], "alert-danger");
                }
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        })


    }



    //顯示明細
    $scope.ShowDtl = function (SDATE) {
        //alert(SDATE + "-" + EmpNo);
        

        $.ajax({
            type: "POST",
            url: "../Attendance/GetShowDtl",
            data: 'SDATE=' + SDATE ,
            async: false,
            success: function (data) {
                $scope.AttendanceShowDtl = null;


                
                if (data[0] != "0") {
                    
                    $scope.AttendanceShowDtl = data;


                    $("#ShowDtlModal").modal('show');
                } else {
                    var str = data.split("|");
                    SysShowAlert("失敗!" + str[1], "alert-danger");
                }
            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        })
        
    }
    
});


