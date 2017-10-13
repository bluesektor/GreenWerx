angular.module('ionic.utils', [])

.factory('$localstorage', ['$window', function($window) {
  return {
    set: function(key, value) {
      $window.localStorage[key] = value;
    },
    get: function(key, defaultValue) {
      return $window.localStorage[key] || defaultValue;
    },
    setObject: function(key, value) {
      $window.localStorage[key] = JSON.stringify(value);
    },
    getObject: function(key) {
      return JSON.parse($window.localStorage[key] || '{}');
    }
  }
}]).directive("compareTo", function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
});

//var compareTo = function () {
//    return {
//        require: "ngModel",
//        scope: {
//            otherModelValue: "=compareTo"
//        },
//        link: function (scope, element, attributes, ngModel) {

//            ngModel.$validators.compareTo = function (modelValue) {
//                return modelValue == scope.otherModelValue;
//            };

//            scope.$watch("otherModelValue", function () {
//                ngModel.$validate();
//            });
//        }
//    };
//};
