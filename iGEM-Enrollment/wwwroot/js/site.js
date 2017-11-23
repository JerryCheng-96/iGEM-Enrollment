// Write your JavaScript code.

var app = angular.module('iGEMForm', []);

app.controller('TheForm',
    function ($scope, $http) {

        $scope.formData = {};

        $scope.whetherResearch = function ($scope) {
            return this.formData.isResearch == "Yes";
        }

        $scope.submit = function () {
            $http.post('/Apply/SubmitForm/', $scope.formData).then();
        }

        $scope.save = function () {
            $http.post('/Apply/SaveForm/', $scope.formData).then();
        }

        $scope.getSaved = function () {
            $http.get('/Apply/GetSavedForm/')
                .then(function (result) {
                    $scope.formData = angular.fromJson(result.data);
                    console.log(result.data);
                });
        }

        $scope.getExistForm = function (isExist, hashValue) {
            if (isExist == 'Yes') {
                $http.get('/Apply/GetExistForm', {params: hashValue})
                    .then(function (result) {
                        $scope.formData = angular.fromJson(result.data);
                        console.log(result.data);
                    });
            }
        }

        $scope.init = function (isExistSaved, inputName, inputId) {
            console.log(isExistSaved + ', got it!');
            if (isExistSaved == 'Yes') {
                console.log('Got the data!');
                $scope.getSaved();
            } else {
                $scope.formData = { 'isResearch': 'Yes', 'engType': 'highSchool', 'gender': 'M', 'name': inputName, 'birthDate': '1996-11', 'phone': '12345678900', 'email': 'a@c.com', 'grade': '2017', 'stuID': inputId, 'college': 'LSC', 'major': 'Major', 'stuFrom': 'CD', 'engGrade': '100', 'stuUnionText': 'StuUnion', 'researchText': 'Research', 'prizeText': 'Prize', 'introText': 'Intro' }
            }
        }

        $scope.getGenderText = function (gender) {
            if (gender == 'M') {
                return '男';
            } else {
                return '女';
            }
        }

        $scope.getEnglishTypeText = function (engType) {
            if (engType == 'cet4') {
                return '大学英语四级';
            }
            if (engType == 'cet6') {
                return '大学英语六级';
            }
            if (engType == 'highSchool') {
                return '高考英语';
            }
            if (engType == 'toeils') {
                return '托福/雅思';
            }
        }

    });

app.controller('InitInfo', function ($scope, $http) {

    $scope.initData = {};

});

app.controller('BriefInfo', function ($scope) {

    $scope.briefInfo = {};

});

app.controller('Welcome', function ($scope) {

    $scope.briefInfo = {};

});


