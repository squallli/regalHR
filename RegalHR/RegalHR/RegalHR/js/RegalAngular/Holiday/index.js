var AngularApp = angular.module('index', []);


AngularApp.controller('Ctrl', function ($scope) {
   

    $scope.DeleteCompany = "";
    $scope.DeleteCompanyName = "";
    $scope.DeleteYearId = "";


    $scope.CopyCompany = "";
    $scope.CopyCompanyName = "";
    $scope.CopyeYearId = "";


    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.CopyCompanyList = [];
    $scope.YearList = [];

    var d = new Date();
    $scope.ThisYear = d.getFullYear();
    
    for (i = $scope.ThisYear - 3; i <= $scope.ThisYear + 3; i++) {
        $scope.YearList.push(i);
    }
    


    $scope.update = function (Url, PageNum) {
        
            $.ajax({
                type: "POST",
                url: Url,
                dataType: 'Json',
                data: 'PageNum=' + PageNum,
                async: false,
                success: function (data) {
                    $scope.UserFreeStyle = angular.fromJson(UserFreeStyle);

                    $scope.UserFreeStyle.PageNum = data.Page;


                    

                    $scope.DataList = data.DataList;
                    $scope.Page = data.Page;
                    $scope.PageTotal = data.PageTotal-1;
                    $scope.PageTotalDisplay = data.PageTotal;
                    $scope.PageLimit = data.PageLimit;
                    $scope.PageBegin = $scope.Page - 5;
                    $scope.SignId = $scope.UserFreeStyle.SignId;//標記點選過的
                    $scope.Serial = (data.Page * data.PageLimit) + 1;//序號起始
                    




                    if ($scope.PageBegin < 0)
                        $scope.PageBegin = 0;


                    var PaginationList = [];


                    for(var i=0;i<data.PageTotal;i++)
                    {
                        var Obj={};
                        Obj.PageNum=i;
                        Obj.PageDisplay=i+1;
                        Obj.Url=data.TargetURL;
                        PaginationList.push(Obj);
                    }

                    

                    $scope.PaginationList = PaginationList;



                    
                },
                error: function (err) {
                    alert('Error!');
                }
            });
    };


    
    $scope.update("../Holiday/GridViewer", UserFreeStyle.PageNum);


    $scope.Orderby = function (id, OrderField) {

        $('#DataGridView th span').html("");//CLEAR


        
   
        if ($scope.UserFreeStyle.OrderField == OrderField) {
            if ($scope.UserFreeStyle.OrderType == "ASC") {
                $scope.UserFreeStyle.OrderType = "DESC";
                $('#DataGridView th[data-id="' + id + '"] span').html("▲");
            } else {
                $scope.UserFreeStyle.OrderType = "ASC";
                $('#DataGridView th[data-id="' + id + '"] span').html("▼");
            }
        } else {
            $scope.UserFreeStyle.OrderField = OrderField;
            $scope.UserFreeStyle.OrderType = "ASC";
            $('#DataGridView th[data-id="' + id + '"] span').html("▼");
        }


        

        $.ajax({
            type: "POST",
            url: "../Holiday/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {

                $scope.update("../Holiday/GridViewer", 0);

            },
            error: function () {
                alert("Error !");
            }
        })
    }

    $scope.Search = function () {
        $.ajax({
            type: "POST",
            url: "../Holiday/SaveUserFreeStyle",
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                $scope.update("../Holiday/GridViewer", 0);
            },
            error: function () {
                alert("Error !");
            }
        });
    }


    $scope.DelMsgBtn = function (RecordID, RecordName) {
        //顯示刪除提示訊息

        $scope.DeleteRecordID = RecordID;
        $scope.DeleteRecordName = RecordName;
        $scope.offDutyDate = "";
        $('#DeleteMsgModal').modal('show')
    }






    $scope.AddBtn = function () {
        

        if ($scope.InsertCompany == undefined) {
            alert("請選擇公司別");
            return;
        }


        $.ajax({
            type: "POST",
            url: "../Holiday/HolidayFormAdd",
            dataType: 'Json',
            data: 'Company=' + $scope.InsertCompany + '&YearId=' + $scope.InsertYear,
            async: false,
            success: function (data) {


                if (data == "1") {
                    document.location.href = "../Holiday/";
                }
                else
                {
                    $('#InsertHolidayModal').modal('hide');
                    SysShowAlert("失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                $('#InsertHolidayModal').modal('hide');
                alert("系統發生錯誤 !");

            }
        })
    }





    $scope.DelBtn = function () {

        $.ajax({
            type: "POST",
            url: "../Holiday/HolidayFormDel",
            dataType: 'Json',
            data: 'Company=' + $scope.DeleteCompany + '&YearId=' + $scope.DeleteYearId,
            async: false,
            success: function (data) {


                if (data == "1") {
                    document.location.href = "../Holiday/";
                }
                else {
                    $('#DeleteHolidayModal').modal('hide');
                    SysShowAlert("失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                $('#DeleteHolidayModal').modal('hide');
                alert("系統發生錯誤 !");

            }
        })
    }




    $scope.Link = function (RecordID) {

        $scope.UserFreeStyle.SignId = RecordID;

        $.ajax({
            type: "POST",
            url: "../Holiday/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Holiday/HolidayForm";
            },
            error: function () {
                alert("Error !");
            }
        })
    }






    $scope.CopyBtn = function () {


        if ($scope.CopyId == undefined) {
            alert("請先選擇公司別!");
            return;
        }

        $.ajax({
            type: "POST",
            url: "../Holiday/HolidayFormCopy",
            dataType: 'Json',
            data: 'Company=' + $scope.CopyCompany + '&YearId=' + $scope.CopyYearId + '&CopyId=' + $scope.CopyId,
            async: false,
            success: function (data) {


                if (data == "1") {
                    document.location.href = "../Holiday/";
                }
                else {
                    $('#CopyHolidayModal').modal('hide');
                    SysShowAlert("失敗!" + data, "alert-danger");
                }

            },
            error: function () {
                $('#CopyHolidayModal').modal('hide');
                alert("系統發生錯誤 !");

            }
        })
    }




    $scope.CopyMsgBtn = function (Company, YearId, CompanyName) {

        $scope.CopyCompany = Company;
        $scope.CopyCompanyName = CompanyName;
        $scope.CopyYearId = YearId;


        $scope.CopyMsg = "您要將 " + CompanyName + "-" + YearId + "年的行事曆 " + " 複製於..";


        $scope.CopyCompanyList = [];
        for (i = 0 ; i < $scope.CompanyList.length; i++) {
            if ($scope.CompanyList[i].Company == $scope.CopyCompany) {
                continue;
            }


            $scope.CopyCompanyList.push($scope.CompanyList[i]);
        }

        $('#CopyHolidayModal').modal('show');
    }

    $scope.AddMsgBtn = function () {
        $('#InsertHolidayModal').modal('show');
    }

    $scope.DelMsgBtn = function (Company,YearId,CompanyName) {

        $scope.DeleteCompany = Company;
        $scope.DeleteCompanyName = CompanyName;
        $scope.DeleteYearId = YearId;

        $('#DeleteHolidayModal').modal('show');
    }

});


