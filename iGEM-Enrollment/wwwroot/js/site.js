// Write your JavaScript code.

var app = angular.module('iGEMForm', ['ngFileUpload']);

app.controller('TheForm', ['$scope', 'Upload', function ($scope, $http, Upload) {
    // upload later on form submit or something similar
    $scope.submit = function () {
        if (form.file.$valid && $scope.file) {
            $scope.upload($scope.file);
        }
    };

    $scope.upload = function (file) {
        Upload.upload({
            url: 'upload/url',
            data: { file: file, 'username': $scope.username }
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
        }, function (resp) {
            console.log('Error status: ' + resp.status);
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });
    };

    $scope.formData = {};
    $scope.lastSavedTime = '';

    $scope.whetherResearch = function ($scope) {
        return this.formData.isResearch == "Yes";
    }

    $scope.submit = function () {
        $http.post('/Apply/SubmitForm/', $scope.formData)
            .then(function (result) {
                window.location.href = '/Apply/SubmitFormSucceeded/';
            });
    }

    $scope.save = function () {
        $http.post('/Apply/SaveForm/', $scope.formData)
            .then(function (result) {
                $scope.lastSavedTime = result.data;
            });
    }

    $scope.getSaved = function () {
        $http.get('/Apply/GetSavedForm/')
            .then(function (result) {
                if (result.data != '') {
                    $scope.formData = angular.fromJson(result.data);
                    console.log(result.data);
                } else {
                    $http.get('/Apply/Clear/').then();
                }
            });
    }

    $scope.getExistForm = function (isExist, hashValue) {
        if (isExist == 'Yes') {
            $http.get('/Apply/GetExistForm', { params: hashValue })
                .then(function (result) {
                    if (result.data != '') {
                        $scope.formData = angular.fromJson(result.data);
                        console.log(result.data);
                    } else {
                        window.location.href = '/Apply/Error?errCode=1';
                    }
                });
        }
    }

    $scope.init = function (isExistSaved, inputName, inputId) {
        console.log(isExistSaved + ', got it!');
        $scope.lastSavedTime = '';
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

}]);

app.controller('MyCtrl', ['$scope', 'Upload', function ($scope, Upload) {
    // upload later on form submit or something similar
    $scope.submit = function () {
        if ($scope.form.file.$valid && $scope.file) {
            $scope.upload($scope.file);
        }
    };

    // upload on file select or drop
    $scope.upload = function (file) {
        Upload.upload({
            url: 'upload/url',
            data: { file: file, 'username': $scope.username }
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
        }, function (resp) {
            console.log('Error status: ' + resp.status);
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });
    };
}]);

app.controller('InitInfo', function ($scope, $http) {

    $scope.initData = {};

});

app.controller('BriefInfo', function ($scope) {

    $scope.briefInfo = {};

});

app.controller('Welcome', function ($scope) {

    $scope.briefInfo = {};

});


