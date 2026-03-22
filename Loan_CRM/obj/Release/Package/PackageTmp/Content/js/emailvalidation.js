/**
 * Created by Tanupriya on 9/16/2016.
 */

$(document).ready(function () {
    EmailValidation();

});
function EmailValidation() {
    ////
    $('.EmailTextBox').focusout(function () {
        if (validate() == false) {
            $(".EmailTextBox").addClass('has-error');
            $(".EmailTextBox").val("");
        }
      });
    }
    function validate() {

        var email = $(".EmailTextBox").val();
        if (validateEmail(email)) {
            return true;
        } else {
            alert('Personal Email is not valid');
            return false;

        }
        return false;
    }

    function validateEmail(email) { 
        //alert(event.keyCode);
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
  
     
    
