
$(document).ready(function () {

//transaction.cshtml page
$('.number').number(true, 2);

// standing order & pay later
$('body').on('click', '.IsSTO', function () {

    var checkbox = $("input:checkbox.IsSTO");

    var istocheckbox = (checkbox.is(":checked"));

  //  alert(istocheckbox)

    if (istocheckbox == true) {
        //$('.hidemonths').show();
        $('.showmonths').hide();

        $('.showPaymentMethods').hide();

        $('.setMonthsInAdvance').val('0');
        //$('.toggleMonths').val('0');
        $('#advances').val(0);
        calculate();
    } else {

        //$('.hidemonths').hide();
        $('.showmonths').show();
        $('.showmonths').val('0');
        $('.setMonthsInAdvance').val(0);
        $('#advances').val(0);
        $('.showPaymentMethods').show();

    }

    //$('.toggleMonths').toggle();
    //$('.toggleMonths').val('0');
    //$('#advances').val(0);
    calculate();


    //   alert(sel);
});


//var ppcheckboxes = $("input:checkbox.postpone");

$('body').on('click', '.postpone', function () {

    checkboxChange(this);

 

    var ppcheckboxes = $("input:checkbox.postpone");



    if (ppcheckboxes.is(":checked")) {
       // $('.hidemonths').show();
        $('.showmonths').hide();
        $('.setMonthsInAdvance').val('0');
        $('#advances').val(0);

        //alert('post-pone');
    } else {
        //$('.hidemonths').hide();
        $('.showmonths').show();
        $('.showmonths').val('0');
        $('.setMonthsInAdvance').val('0');
        $('#advances').val(0);

    }
    calculate();

});




$('body').on('blur', '.fieldchange', function () {
    if ($(this).val() == '') {
        $(this).val(0);
    }
    calculate();
});


//MONTH SECTION

$('.writeoff').change(function () {
    checkboxChange(this);
    calculate();
});

$('body').on('change', '.months', function () {
    var feepermonth = $('#monthlyFee').val();
    var mths = $(this).val()
    var sum = +feepermonth * +mths;
    $('#advances').val(sum);

    
    $('.setMonthsInAdvance').val(mths);

    calculate();

});

$('body').on('change', '.discount, #registrationFee', function () {
    //alert('discount changed L294 shared.js')
    calculate();
});






}); //end of document ready

//function that recalculates the amount to be sent
function checkboxChange(elem)
{

    var corresponding = $(elem).data("corresponding");

    $(corresponding).prop('checked', false); // Unchecks it

    //when writeoff is checked set row to 0
    if ($(elem).is(":checked")) {

        var dueId = $(elem).data("dueid");
        $(dueId).val(0);

        //when writeoff is unchecked set row to original amt
    } else {

        var origfee = $(elem).data("origfee");
        var dueId = $(elem).data("dueid");
        $(dueId).val(origfee);
    }
}

// used on transaction.cshtml

function calculate() {

   // alert('calculate called');
    var sum = 0;
    var reg = 0;

    //caculate dues
    $('.amtDue').each(function () {
        var due = $(this).val().replace(',', '');
        sum += +due;  //Or this.innerHTML, this.innerText
    });


    //set the total amount due field
    $('#amountDue').val(+sum);


    //calculate reg fee if exists, contains a hidden to initiate, in case there is none as else error occurs
    var reg = 0;
    if ($('#registrationFee').length > 0) {
        var reg = $('#registrationFee').val().replace(',', '');
    }

    $('.registrationFee').val(reg);

    //calculate subtotal

    var adv = 0;
    // check if it exists
    if ($('#advances').length > 0) {
        adv = $('#advances').val().replace(',', '');
    }

    var due = 0;
    if ($('#amountDue').length > 0) {
        due = $('#amountDue').val().replace(',', '');
    }


    //alert(due + ' ' + adv + ' ' + reg);
    $('#subtotal').val(+due + +adv + +reg);


    //verify that discount is not greater than subtotal    
    var dis = $('.discount').val().replace(',', '');


    var sub = 0;
    if ($('#subtotal').length > 0) {
        sub = $('#subtotal').val().replace(',', '');
    }

    //reassign discount bec of hidden discount field
    $('.discount').val(dis);

    //if discount greater, then replace with max amt, i.e subtotal
    // also exclude registration fro discount
    //alert(dis + ' ' + sub);
    if (+dis > (+sub - +reg)) {
        $('.discount').val(+sub - +reg);
        dis = +sub - +reg;
    }




    //calculate total
    $('#total').val(+due + +adv + +reg + -dis);



};

//https://css-tricks.com/snippets/jquery/check-if-element-exists/
// Tiny jQuery Plugin
// by Chris Goodchild
$.fn.exists = function (callback) {
    var args = [].slice.call(arguments, 1);

    if (this.length) {
        callback.call(this, args);
    }

    return this;
};

// Usage
//$('div.test').exists(function () {
//  this.append('<p>I exist!</p>');
//});
