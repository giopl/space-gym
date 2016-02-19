
$(document).ready(function () {

    $('body').on('blur', '.number', function () {
        if ($(this).val() == '') {
            $(this).val(0);
        }
        });

/* used in _installmentTransaction.cshml */
$('body').on('change', '.facility_1', function () {

    var due = $('#unpaidAmt').val();
    var paid = $(this).val().replace(',', '');
    if (+paid <= 0) {
        $(this).val(due);
        paid = due;
    }


    if (+paid > +due) {
        $(this).val(due);
        paid = +due;
    }

    $('#facilityDownpayment').val(paid);

    var left = +due - +paid;
    $('.facility_2').val(left);

});

//YEAR SECTION


//$('body').on('change', '#startDtYear', function () {
//    var ed = $(this).find(':selected').data('enddate');
//    $('#endDtYear').val(ed)
//});


$('body').on('change', '#numInstallments', function () {

    var regfee = $('.registrationFeeForm').val();


    //recalculateYearly(+regfee);

    var fac = $(this).val();

    //var reg = $('.registrationFee').val().replace(',', '');
    var orig = $('#totdueandreg').val();



    //var regOrig = +reg + +orig;


    $('#initialDownpayment').val(orig);

    //alert(orig);


    $('#secondDownpayment, #thirdDownpayment').val('');

    if (fac == 1) {
        $('#secondInst').show();
        $('#thirdInst').hide();
        $('#labelRowInst').show();

        $("#initialDownpayment").prop("readonly", false);
    } else if (fac == 2) {
        $('#secondInst').show();
        $('#thirdInst').show();
        $("#initialDownpayment").prop("readonly", false);
        $('#labelRowInst').show();

    } else {
        $('#secondInst').hide();
        $('#thirdInst').hide();
        $("#initialDownpayment").prop("readonly", true);
        $('#labelRowInst').hide();

    }


});




$('body').on('change', '.registrationFee', function () {

    var origReg = $('#origRegistrationFee').val();

    var newReg = $(this).val();
    var paramReg = newReg;

    if (newReg < 0) {
        $(this).val(0);
        paramReg = 0;

    } else if (+newReg > +origReg) {
        $(this).val(origReg);
        paramReg = origReg;
    }

    $('.registrationFeeForm').val(paramReg);


    //alert($(this).val());
    recalculateYearly();

});


$('body').on('change', '.yearlyDiscount', function () {
    $('.reset').val();
    var disc = $(this).val().replace(',', '');

    $('.setyeardiscount').val(disc);
    recalculateYearly();
});




$('body').on('change', '#initialDownpayment', function () {

    var fac = $('#numInstallments').val();
    var orig = $('#totdueandreg').val();
    var reg = $('.registrationFeeForm').val();
    var disc = $('.setyeardiscount').val();

    var amt = $(this).val().replace(',', '');

    if (+amt <= 0)
    {
        amt = orig;
       // alert('amt is less than 0 and is being set to: ' + amt);
        $(this).val(orig);
    }



    $('#_initialDownpayment').val(amt);


    if (+amt > orig) {
        $(this).val(orig);
        $('#_initialDownpayment').val(orig);
        amt = +orig;

    }


    if (fac == 1) {
        var downpay = orig - amt;
        $('#secondDownpayment').val(downpay);
        $('#labelRowInst').show();

        $('#secondInst').show();

        $('#thirdInst').hide();
    } else if (fac == 2) {
        $('#labelRowInst').show();

        var downpay = (orig - amt) / 2;
        $('#secondDownpayment, #thirdDownpayment').val(downpay);
        $('#secondInst').show();
        $('#thirdInst').show();
    } else {
        $('#secondInst').hide();
        $('#thirdInst').hide();
        $('#labelRowInst').hide();

    }
    recalculateYearly();

});

});


/* function section */
// used on transaction.cshtml
function recalculateYearly() {

    var reg = $('.registrationFeeForm').val();
    var lof = $('#longOverdueFee').val();
    var of = $('#yearOrigFee').val();

    var fee = +lof + +of;

    var disc = $('.yearlyDiscount').val().replace(',', '');

    if(disc < 0)
    {
        disc = 0;
        $('.yearlyDiscount').val(disc);
        $('.setyeardiscount').val(disc);
    }

    if (+reg + +fee <= +disc) {
        disc = +reg + +fee;
        $('.yearlyDiscount').val(disc);
        $('.setyeardiscount').val(disc);
    }

    var tot = +reg + +fee - +disc;


    if (tot < 0) {
        $('.paybutton').hide();
    } else {
        $('.paybutton').show();

    }
    $('#totfee').val(tot);

    $('#totdueandreg').val(tot);

    if (calculateDownpayments() <= 0 || isNaN(calculateDownpayments())) {

        $('#initialDownpayment').val(tot);
        $('#_initialDownpayment').val(tot);
    }
}


function calculateDownpayments() {
    var dp2 = 0; var dp2 = 0;
    if ($('#secondDownpayment').length > 0) {
        var dp2 = $('#secondDownpayment').val().replace(',', '');
    }

    if ($('#thirdDownpayment').length > 0) {
        var dp3 = $('#thirdDownpayment').val().replace(',', '');
    }


    var downpayments = +dp2 + +dp3;

    if (downpayments > 0 && !isNaN(downpayments))
    {
        $('#UnpaidAmount').val(downpayments);
        $('#NumInstallmentsLeft').val(1);

        if (dp3 > 0)
        {
            $('#NumInstallmentsLeft').val(2);
        } 
      //  alert('unpaid amount' + downpayments);
    }

    return downpayments;
}
