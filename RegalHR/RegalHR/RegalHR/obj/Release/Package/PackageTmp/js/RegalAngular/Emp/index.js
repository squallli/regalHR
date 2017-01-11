var AngularApp = angular.module('index', []);


AngularApp.controller('Ctrl', function ($scope) {
   

    $scope.DeleteRecordID = "";
    $scope.DeleteRecordName = "";
    $scope.CompanyList = angular.fromJson(CompanyList);
    $scope.DepartMentList = angular.fromJson(DepartMentList);
    $scope.EmpStatusList = angular.fromJson(EmpStatusList);
    $scope.EmpSexList = angular.fromJson(EmpSexList);
    $scope.Math = window.Math;



    $scope.GetEmpList = function () {

        $.ajax({
            type: "POST",
            url: "../Emp/GetEmpList",
            dataType: 'Json',
            data: 'FulltimeFlag=1&company=' + $scope.UserFreeStyle.SearchCompany + "&DepartMentNo=" + $scope.UserFreeStyle.SearchDepartMentNo + "&status=" + $scope.UserFreeStyle.SearchEmpStatus + "&Sex=" + $scope.UserFreeStyle.SearchText2,
            async: false,
            success: function (data) {
                $scope.EmpDropDownList = data;
            }
        });
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



                    /*
                    $.ajax({
                        type: "POST",
                        url: "../Emp/GetEmpList",
                        data: 'DepartMentNo=' + $scope.UserFreeStyle.SearchText,
                        async: false,
                        success: function (data) {

                            $scope.EmpList = angular.fromJson(data);

                        }
                    })
                    */

                    
                },
                error: function (err) {
                    alert('Error!');
                }
            });
    };


    
    $scope.update("../Emp/GridViewer", UserFreeStyle.PageNum);

    $scope.GetEmpList();

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
            url: "../Emp/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {

                $scope.update("../Emp/GridViewer", 0);

            },
            error: function () {
                alert("Error !");
            }
        })
    }

    $scope.Search = function () {
        $.ajax({
            type: "POST",
            url: "../Emp/SaveUserFreeStyle",
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                $scope.update("../Emp/GridViewer", 0);
            },
            error: function () {
                alert("Error !");
            }
        });
    }
    /*
    $scope.Search = function () {

        $scope.UserFreeStyle.SearchText2 = "";
        $.ajax({
            type: "POST",
            url: "../Emp/SaveUserFreeStyle",
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {

                $.ajax({
                    type: "POST",
                    url: "../Emp/GetEmpList",
                    data: 'DepartMentNo=' + $scope.UserFreeStyle.SearchText,
                    async: false,
                    success: function (data) {
                        
                        $scope.EmpList = angular.fromJson(data);

                    }
                })


                $scope.update("../Emp/GridViewer", 0);
            },
            error: function () {
                alert("Error !");
            }
        })

    }    */



    $scope.DelMsgBtn = function (RecordID, RecordName) {
        //顯示刪除提示訊息

        $scope.DeleteRecordID = RecordID;
        $scope.DeleteRecordName = RecordName;
        $scope.offDutyDate = "";
        $('#DeleteMsgModal').modal('show')
    }



    $scope.CheckDelResult = function () {
        //檢查離職日期 是否有填寫

        if ($scope.offDutyDate == "" || $scope.offDutyDate == null) {
            return false;
        } else {
            return true;
        }

    }

    $scope.DelBtn = function (offDutyDate) {
        //離職
        $.ajax({
            type: "POST",
            url: "../Emp/EmpFormDel",
            dataType: 'Json',
            data: 'offDutyDate=' + offDutyDate + '&EmpID=' + $scope.DeleteRecordID,
            async: false,
            success: function (data) {
                

                if (data == "1")
                {
                    document.location.href = "../Emp/";
                }
                else
                {
                    $('#DeleteMsgModal').modal('hide');
                    SysShowAlert("失敗!" + data, "alert-danger");
                }


            },
            error: function () {
                $('#DeleteMsgModal').modal('hide');
                alert("系統發生錯誤 !");
                
            }
        })
    }




    $scope.Link = function (RecordID) {

        $scope.UserFreeStyle.SignId = RecordID;
 

        $.ajax({
            type: "POST",
            url: "../Emp/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Emp/EmpForm";
            },
            error: function () {
                alert("Error !");
            }
        })
    }



    $scope.Add = function () {
        $scope.UserFreeStyle.SignId = "";

        $.ajax({
            type: "POST",
            url: "../Emp/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Emp/EmpForm";

            },
            error: function () {
                alert("Error !");
            }
        })


    }

    
});


