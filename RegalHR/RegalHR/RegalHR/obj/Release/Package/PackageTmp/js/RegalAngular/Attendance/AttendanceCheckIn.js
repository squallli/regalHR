var AngularApp = angular.module('Manage', []);


AngularApp.controller('Ctrl', function ($scope) {

    var Today = new Date();


    $scope.SDATE = Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate();
    $scope.EDATE = Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate();

    $scope.AttendanceList = angular.fromJson(AttendanceList);
    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.DepartMentList = angular.fromJson(DepartMentList);
    $scope.UserFreeStyle = angular.fromJson(UserFreeStyle);
    




    $scope.Page = 0; //目前頁數
    $scope.PageTotal = 0; //最終頁
    $scope.RecordLimit = 0; //一頁幾筆
    $scope.PageBegin = 0;
    $scope.SearchData = "";



    //快選
    $scope.Quick = function () {

        $scope.AttendanceCheckInDtlForm.CheckInDescription = $scope.QuickMemo;

    }



    $scope.GetEmpList = function () {

        $.ajax({
            type: "POST",
            url: "../Emp/GetEmpList",
            dataType: 'Json',
            data: 'FulltimeFlag=&company=' + $scope.UserFreeStyle.SearchCompany + "&DepartMentNo=" + $scope.UserFreeStyle.SearchDepartMentNo + "&status=" + $scope.UserFreeStyle.SearchEmpStatus + "&Sex=",
            async: false,
            success: function (data) {
                $scope.EmpDropDownList = data;
            }
        });
    }





    $scope.Search = function () {


        $scope.AttendanceList = [];

        $scope.SearchData = 'UserFreeStyleJson=' + angular.toJson($scope.UserFreeStyle) + '&SDATE=' + $scope.SDATE + '&EDATE=' + $scope.EDATE



        $.ajax({
            type: "POST",
            url: "../Attendance/GetManageAttendanceList",
            data: $scope.SearchData +'&Skip=0',
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

                        $scope.PageTotal = PageTotal-1; //最終頁
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
    $scope.GetEmpList();







    //變換考勤描述顏色
    $scope.AttendanceDescBgColor = function (Desc) {


        var BgColor="";
        if (Desc.search("曠職")>=0) {
            BgColor = {"background-color":"#F7D358"};
        }
       
        return BgColor;
    }



    //更新頁面資料
    $scope.update = function (PageNum) {

        $scope.AttendanceList = [];
        $.ajax({
            type: "POST",
            url: "../Attendance/GetManageAttendanceList",
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

    $scope.ShowDtl = function (SDATE, EmpNo) {



        $.ajax({
            type: "POST",
            url: "../Attendance/GetShowDtlManage",
            data: 'SDATE=' + SDATE + "&EmpNo="+EmpNo,
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


    $scope.SaveCheckInDtlForm = function () {
        if (!window.confirm("您確定要補登出勤記錄?")) {
            return;
        }


        //編輯
        $.ajax({
            type: "POST",
            url: "../Attendance/SaveCheckInDtlForm",
            dataType: 'Json',
            data: 'AttendanceCheckInDtlJson=' + angular.toJson($scope.AttendanceCheckInDtlForm),
            async: false,
            timeout: 10000,
            success: function (data) {
                if (data == "1") {

                    $("#ShowCheckInFormModal").modal('hide');
                    $scope.update($scope.Page);

                } else {
                    SysShowAlert("新增失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                SysShowAlert("系統發生錯誤!", "alert-danger");
            }
        });
    }






    $scope.ShowCheckInForm = function (SDATE, EmpNo) {

        $scope.QuickMemo = "";

        $.ajax({
            type: "POST",
            url: "../Attendance/GetCheckInDtlForm",
            data: 'SDATE=' + SDATE + "&EmpNo=" + EmpNo,
            async: false,
            success: function (data) {
                $scope.AttendanceShowDtl = null;

                

                if (data[0] != "0") {

                    $scope.AttendanceCheckInDtlForm = data;

                    $("#ShowCheckInFormModal").modal('show');


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


