var AngularApp = angular.module('index', []);


AngularApp.controller('Ctrl', function ($scope, $timeout) {

    $scope.Data;

    $scope.GetData = function () {
        var Model = [];


        for (i = 0; i < 100;i++)
        {
            var Obj = JSON.parse('{"No":""}');

            Obj.No = "No. " + i;
            Obj.Ans1 = Math.floor(Math.random() * 1000);
            Obj.Ans2 = Math.floor(Math.random() * 1000);
            Obj.Ans3 = Math.floor(Math.random() * 1000);
            Obj.Ans4 = Math.floor(Math.random() * 1000);
            Obj.Ans5 = Math.floor(Math.random() * 1000);
            Obj.Ans6 = Math.floor(Math.random() * 1000);
            Obj.Ans7 = Math.floor(Math.random() * 1000);
            Obj.Ans8 = Math.floor(Math.random() * 1000);
            Obj.Ans9 = Math.floor(Math.random() * 1000);
            Obj.Ans10 = Math.floor(Math.random() * 1000);


            Model.push(Obj);
        }

        $scope.Data = Model;
        $timeout($scope.GetData, 1);
    }

    $scope.GetData();

});


