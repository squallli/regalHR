var AngularApp = angular.module('index', []);


AngularApp.controller('Ctrl', function ($scope) {


    $scope.DeleteRecordID = "";
    $scope.DeleteRecordName = "";

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
                $scope.PageTotal = data.PageTotal - 1;
                $scope.PageTotalDisplay = data.PageTotal;
                $scope.PageLimit = data.PageLimit;
                $scope.PageBegin = $scope.Page - 5;
                $scope.SignId = $scope.UserFreeStyle.SignId;//標記點選過的
                $scope.Serial = (data.Page * data.PageLimit) + 1;//序號起始





                if ($scope.PageBegin < 0)
                    $scope.PageBegin = 0;


                var PaginationList = [];


                for (var i = 0; i < data.PageTotal; i++) {
                    var Obj = {};
                    Obj.PageNum = i;
                    Obj.PageDisplay = i + 1;
                    Obj.Url = data.TargetURL;
                    PaginationList.push(Obj);
                }



                $scope.PaginationList = PaginationList;
            },
            error: function (err) {
                alert('Error!');
            }
        });
    };



    $scope.update("../Group/GridViewer", UserFreeStyle.PageNum);



    $scope.Orderby = function (id,OrderField) {

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
            url: "../Group/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {

                $scope.update("../Group/GridViewer", 0);

            },
            error: function () {
                alert("Error !");
            }
        })
    }



    $scope.Search = function () {
        $.ajax({
            type: "POST",
            url: "../Group/SaveUserFreeStyle",
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                $scope.update("../Group/GridViewer", 0);
            },
            error: function () {
                alert("Error !");
            }
        })
    }



    $scope.DelMsgBtn = function (RecordID, RecordName) {
        //顯示刪除提示訊息

        $scope.DeleteRecordID = RecordID;
        $scope.DeleteRecordName = RecordName;

        $('#DeleteMsgModal').modal('show');
    }



    $scope.DelBtn = function () {
        //刪除

        $.ajax({
            type: "POST",
            url: "../Group/GroupFormDel",
            dataType: 'Json',
            data: 'GroupID=' + $scope.DeleteRecordID,
            async: false,
            success: function (data) {

                if (data == "1") {
                    //成功刪除
                    document.location.href = "../Group/";
                } else {
                    $('#DeleteMsgModal').modal('hide');
                    SysShowAlert("刪除失敗!" + data, "alert-danger");
                   
                }

            },
            error: function () {
                alert("Error !");
            }
        })

    }




    $scope.Link = function (RecordID) {

        $scope.UserFreeStyle.SignId = RecordID;


        $.ajax({
            type: "POST",
            url: "../Group/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Group/GroupForm";
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
            url: "../Group/SaveUserFreeStyle",
            dataType: 'Json',
            data: 'UserFreeStyle=' + angular.toJson($scope.UserFreeStyle),
            async: false,
            success: function (data) {
                document.location.href = "../Group/GroupForm";
            },
            error: function () {
                alert("Error !");
            }
        })


    }


});


