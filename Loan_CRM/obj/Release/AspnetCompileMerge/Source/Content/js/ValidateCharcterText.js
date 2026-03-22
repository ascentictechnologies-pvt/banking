/**
 * Created by Tanupriya on 9/16/2016.
 */


$(document).ready(function () {
    checkValidCharacterInput();

});
function checkValidCharacterInput() {
    $(".CharcaterTextBox").keydown(function (event) {
        //alert(event.keyCode);
        if (!((
                event.keyCode == 8 ||
                event.keyCode == 37 ||
                event.keyCode == 39 ||
                event.keyCode == 9 ||
                event.keyCode == 32 ||
                event.keyCode == 110 ||
                event.keyCode == 190) ||
                (event.ctrlKey && event.keyCode == 86) ||  // Edit: Added to allow ctrl+v
                ((event.keyCode >= 65 && event.keyCode <= 90))

            )) {
            event.preventDefault();
            return false;
        }
    });
}