// Write your JavaScript code.

var app = angular.module('iGEMForm', []);

app.controller('TheForm', function ($scope) {

    $scope.formData = {};

    $scope.whetherResearch = function ($scope) {
        return this.formData.isResearch == "Yes";
    }
});

app.controller('InitInfo', function ($scope) {

    $scope.initData = {};

    $scope.clearInputs = function ($scope) {
        $scope.initData.Name = '';
        $scope.initData.StuID = '';
    }
    
});

