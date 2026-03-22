$(document).ready(function () {
    checkValidInput();

});

function checkValidInput() {
    $(".NumericTextBox").keypress(function (e) {
        var charCode = (e.which) ? e.which : event.keyCode
        if (String.fromCharCode(charCode).match(/[^-.\d{1,2}]/))
            return false;
    });
}