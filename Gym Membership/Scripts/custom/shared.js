/* prototype js additions */

if (typeof String.prototype.startsWith != 'function') {
    // see below for better implementation!
    String.prototype.startsWith = function (str) {
        return this.indexOf(str) === 0;
    };
}

/* document ready section */

$(document).ready(function () {

    $table = $('.scrolltable');
    var $window = $(window).on('resize', function () {
        var height = screen.height * .6;
        $table.height(height);
    }).trigger('resize'); //on page load



    //Applies to all pages 
    //displays the search bar
    $('body').on('click', '.showsection', function () {
        $('.searchmember').toggle();
    });


    // whether the user is checked-in or goes to the page
    $('body').on('click', '.chooseOpt', function () {
        var opt = $(this).data('ops');

        var memberId = $("#founduser").val();
        var IsPass = $("#isPass").val() == 'Y';


        if (opt == "UserDetails") {
            redirectToMember(memberId);            
        } else {
            checkin(memberId,IsPass)
        }
    });



    // choose to checkin user
    $('body').on('click', '.chooseCheckin', function () {
        var memberId = $(this).data('memberid');
        var isPass = $(this).data('ispass');

        $('#selectMemberModal').modal('hide');
        checkin(memberId,ispass)
    });

    // choose to go to member's page
    $('body').on('click', '.chooseView', function () {
        var memberId = $(this).data('memberid');
        $('#selectMemberModal').modal('hide');

        redirectToMember(memberId)
    });
    

    //// choose to go to member's page
    //$('body').on('click', '.choosePass', function () {
    //    alert('choosepass');
    //    var memberId = $(this).data('memberid');
    //    $('#selectMemberModal').modal('hide');

    //    redirectToPass(memberId)
    //});



    // close the search bar
    $('body').on('click', '#closesearch', function () {
        $('.searchmember, #found').hide();
    });

    // search membeer by name for checkin
    $('body').on('click', '#searchmemberforcheckin', function () {
        var p_search = $('#q').val();
        if (p_search.length > 2) {
            findmember(p_search);
        }
    });

    //selects a member when multiple found (by name) for checkin
    $('body').on('click', '.selectmemberforcheckin', function () {
        var d = $(this).data('memberid');
        $('.searchmember, #found').hide();
        //alert('sel mem chck in');
        $('#selectMemberModal').modal('hide');

        findmember(d);
    });


    // on enter key searches member for checkin or viewing
    $(document).keypress(function (e) {
        if (e.which == 13) {
            $("#searchmemberforcheckin").click();
        }
    });

    // TODO check usage on _layout or visit
    $("body").on("click", ".removeExistingItem", function () {

        var p_id = $(this).data('id');
        var _this = $(this);
        $.ajax({
            type: "POST",
            //url: "@Url.Action("AjaxRemoveExistingItem", "User") ",
            url: getVirtualDir() + 'User/AjaxRemoveExistingItem',
            data: { id: p_id },
            success: (function (response) {

                if (response == "SUCCESS") {

                    $(_this).parent().parent().css("display", "none");
                    alert("Item successfully removed from your existing items\nTo add it back go to your settings");
                }
            }
             )

        });

    });



// phone formatter used in NewMember.cshtml & Edit.cshtml
    $('.mobileFormat').formatter({
        'pattern': '5-{{999}}-{{9999}}',
        'persistent': true
    });


    $('.phoneFormat').formatter({
        'pattern': '{{999}}-{{9999}}',
        'persistent': true
    });


    $('.dateFormat').formatter({
        'pattern': '{{9999}}-{{99}}-{{99}}',
        //'patterns': [{'^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$'}],
        'persistent': true
    });



    //used in _visit and main page to show visits per dayte
    $.fn.datepicker.defaults.format = "dd/mm/yyyy";
    $('.datepicker').datepicker({
        orientation: "bottom auto",
        endDate: '0d',
        todayBtn: 'true',
        todayHighlight: 'true'
        //startDate: '-3d'

    }).on('changeDate', function (e) {

        refreshdate(e.format('dd-M-yyyy'));
    });


    // used on member page, shows meaning of each icon
    $('body').on('mouseover', '.message', function () {

        var msg = $(this).attr('title');
        $('#meaning').html(msg);

    });


    // returns list of visit history for user
    $('body').on('click', '#retrieveHistory', function () {
        gethistory();
    });



    
    //used on members page for datatables

    //$('#memberstable').dataTable(
    $('.datatab').dataTable();
       

    $('#standingordertable').dataTable();

    //NewMember.cshtml page
    $('body').on('load', function () {

        initialiseNewMember();
    });

    var maxyr = new Date().getFullYear() - 5;

    $('.pickadate').pickadate(
        {
            formatSubmit: 'yyyy-mm-dd',
            hiddenName: true,
            //today:'',
            selectYears: 70,
            selectMonths: true,
            min: new Date(1940, 2, 2),
            max: new Date(maxyr, 11, 30)
        });

    $('.number').number(true, 2);


    $('body').on('change', '#MembershipNumMember', function () {
        $('.hideme').hide();
        var num = $(this).find(':selected').data('num');

        if ($(this).val() == "CUST" || $(this).val() == "TEMP") {
            $('#showifcustom').show();
        } else {
            $('#showifcustom').hide();
        }

        $('#NumMembers').val(num);

        if (num == "2") {
            $('#memberNum_1').show();
        }

        if (num == "3") {
            $('#memberNum_1, #memberNum_2').show();
        }

        if (num == "4") {
            $('#memberNum_1, #memberNum_2,#memberNum_3').show();
        }
        if (num == "5") {
            $('#memberNum_1, #memberNum_2,#memberNum_3,#memberNum_4').show();
        }
    });

    $('.date-entry').dateEntry();


    $('body').on('change', '.changeGender', function () {
        var n = $(this).data('num');
        //   alert(n);

        //  alert($('.changeGender_' + n).val());

        var sel = $(this).val();

        if (sel == 'Ms' || sel == 'Mrs') {
            $('.changeGender_' + n).val('F');

        } else if (sel == 'Mr') {
            $('.changeGender_' + n).val('M');

        }


    });

    $('body').on('click', '.useSameAddress', function () {
        var n = $(this).data('num');

        if ($(this).is(':checked')) {

            $('.useSameAddress_' + n).hide();

        }
        else {

            $('.useSameAddress_' + n).show();

        }


    });

    $('.typeahead-town').typeahead({
        source: ['Port Louis', 'Ebene', 'Beau Bassin', 'Rose Hill', 'Curepipe', 'Quatre Bornes', 'Vacoas', 'Phoenix', 'Belle Etoile', 'Albion', 'Amaury', 'Amitie-Gokhoola', 'Arsenal', 'Baie du Cap', 'Baie du Tombeau', 'Bambous', 'Bambous Virieux', 'Bananes', 'Beau Vallon', 'Bel Air Riviere Seche', 'Bel Ombre', 'Belle Vue Maurel', 'Benares', 'Bois Cheri', 'Bois des Amourettes', 'Bon Accueil', 'Bramsthan', 'Brisee Verdiere', 'Britannia', 'Calebasses', 'Camp Carol', 'Camp de Masque', 'Camp de Masque Pave', 'Camp Diable', 'Camp Ithier', 'Camp Thorel', 'Cap Malheureux', 'Cascavelle', 'Case Noyale', 'Centre de Flacq', 'Chamarel', 'Chamouny', 'Chemin Grenier', 'Clemencia', 'Cluny', 'Congomah', 'Coromandel', 'Cottage', 'Creve Coeur', 'D\'epinay', 'Dagotiere', 'Dubreuil', 'ecroignard', 'Esperance', 'Esperance Trebuchet', 'Flic en Flac', 'Fond du Sac', 'Goodlands', 'Grand Baie', 'Grand Bel Air', 'Grand Bois', 'Grand Gaube', 'Grand Sable', 'Grande Retraite', 'Grande Riviere Noire', 'Grande Riviere Sud Est', 'Gros Cailloux', 'L\'Avenir', 'L\'Escalier', 'La Flora', 'La Gaulette', 'La Laura Malenga', 'Lalmatie', 'Laventure', 'Le Hochet', 'Le Morne', 'The Vale', 'Mahebourg', 'Mapou', 'Mare Chicose', 'Mare d\'Albert', 'Mare La Chaux', 'Mare Tabac', 'Medine Camp de Masque', 'Melrose', 'Midlands', 'Moka', 'Montagne Blanche', 'Montagne Longue', 'Morcellement Saint Andre', 'New Grove', 'Notre Dame', 'Nouvelle Decouverte', 'Nouvelle France', 'Olivia', 'Pailles', 'Pamplemousses', 'Petit Bel Air', 'Petit Raffray', 'Petite Riviere', 'Piton', 'Plaine des Papayes', 'Plaine des Roches', 'Plaine Magnien', 'Pointe aux Piments', 'Poste de Flacq', 'Poudre d\'Or', 'Poudre d\'Or Hamlet', 'Providence', 'Quartier Militaire', 'Quatre Cocos', 'Quatre Soeurs', 'Queen Victoria', 'Richelieu', 'Ripailles', 'Riviere des Anguilles', 'Riviere des Creoles', 'Riviere du Poste', 'Riviere du Rempart', 'Roche Terre', 'Roches Noires', 'Rose Belle', 'Saint Aubin', 'Saint Hubert', 'Saint Julien Village', 'Saint Julien d\'Hotman', 'Saint Pierre', 'Sebastopol', 'Seizieme Mille', 'Souillac', 'Surinam', 'Tamarin', 'Terre Rouge', 'Triolet', 'Trois Boutiques', 'Trou aux Biches', 'Trou d\'Eau Douce', 'Tyack', 'Union Park', 'Verdun', 'Vieux Grand Port', 'Ville Bague']
    });


}); //end of document ready

//redirects to member's page
function redirectToMember(id) {
    window.location = getVirtualDir() + "User/Member/" + id;
}


function redirectToPass(id) {
    window.location = getVirtualDir() + "User/PassVisitor/" + id;
}


//checks in member
function checkin(m_id,m_ispass) {
    var src = $('#sourcepage').val();

    $('#checkinOrViewModal').modal('hide');
    $.ajax({
        type: "POST",
        //  url: getVirtualDir() + 'User/AjaxCheckUserIn',
        url: getVirtualDir() + 'User/AjaxCheckIn',

        data: { id: m_id, s: src, p:m_ispass },
        success: (function (response) {
            if (response.startsWith('<section id="visit">')) {
                //alert('visits');
                $('#visits').html(response);

            }

            else {

                $('#myModalLabelCheckInHeader').html("<span class='label label-success'>Member Checked In</span>")
                $('#modal-body-checked-in').html(response);
                $('#customerCheckedInModal').modal('show');

            };
        })
    });
}



// used on index page, refreshes visits for particular date
function gethistory() {
//var dt = data.substr(4, 11);
    //alert(dt);

    $.ajax({
        type: "POST",
        url: getVirtualDir() + 'User/History',
        //data: { dt: data },
        success: (function (response) {

            $('#modal-body-history').html(response);


        })
    });

}



// used on index page, refreshes visits for particular date
function refreshdate(data) {

    var src = $('#sourcepage').val();
    //var dt = data.substr(4, 11);
    //alert(dt);

    $.ajax({
        type: "POST",
        url: getVirtualDir() + 'User/VisitsOn',
        data: { dt: data },
        success: (function (response) {

            $('#visits').html(response);
            $('#visit-date').html("<span class='label label-default'>" + data + "</span>");


        })
    });

}

//searches for user
function findmember(data) {

    var src = $('#sourcepage').val();

    $.ajax({
        type: "POST",
        //  url: getVirtualDir() + 'User/AjaxCheckUserIn',
        url: getVirtualDir() + 'User/FindUser',

        data: { q: data, s: src },
        success: (function (response) {

            if (response.startsWith('Member #')) {
                $('#myModalLabelCheckInHeader').html("<span class='label label-success'>Member Checked In</span>")
                $('#modal-body-checked-in').html(response);
                $('#customerCheckedInModal').modal('show');

            }
            else if (response.startsWith("<section id='notfound'>") || response.startsWith("<section id='userNotActive'>")) {
                $('#myModalLabelCheckInHeader').html("<span class='label label-danger'>Member Not Checked In</span>")
                $('#modal-body-checked-in').html(response);
                $('#customerCheckedInModal').modal('show');

            } else if (response.startsWith('Load_')) {

                var memid = response.replace("Load_", "");
                redirectToMember(memid);
            }
            else if (response.startsWith('LoadPass_')) {
                //alert('loadpass');
                var memid = response.replace("LoadPass_", "");
                redirectToPass(memid);


            } else if (response.startsWith("Choose_")) {
                var memid = response.replace("Choose_", "");
                $('#founduser').val(memid);
                $('#isPass').val('N');
                $('#checkinOrViewModal').modal('show');

                //               redirectToMember(memid);


            }

            else if (response.startsWith("ChoosePass_")) {
                var memid = response.replace("ChoosePass_", "");
                $('#founduser').val(memid);
                $('#isPass').val('Y');
                $('#checkinOrViewModal').modal('show');

                //               redirectToMember(memid);


            }
            else {
                if (src == "Home") {

                    if (response.startsWith('<section id="visit">')) {
                        //alert('visits');
                        $('#q').val('');
                        $('#visits').html(response);

                    }

                    else {
                        //alert('modal-body0select');
                        $('#modal-body-select').html(response);
                        $('#selectMemberModal').modal('show');

                    };


                } else {

                    if (response.startsWith('<section id="result">')) {
                        $('#modal-body-select').html(response);
                        $('#selectMemberModal').modal('show');
                    }
                    //$('#found').show();
                    //$('#found').html(response);
                }
            }

        }
         )

    });

}

// used on NewMember page

function initialiseNewMember() {


    $('.mobileFormat').formatter({
        'pattern': '5-{{999}}-{{9999}}',
        'persistent': true
    });

   

    $('.phoneFormat').formatter({
        'pattern': '{{999}}-{{9999}}',
        'persistent': true
    });


    $('.dateFormat').formatter({
        'pattern': '2{{999}}-{{[0-1]}}{{[0-9]}}-{{0-3}}{{0-9}}',
        'persistent': true
    });



    $('.hideme').hide();
    var num = $(this).find(':selected').data('num');

    if ($(this).val() == "CUST" || $(this).val() == "TEMP") {
        $('#showifcustom').show();
    } else {
        $('#showifcustom').hide();
    }

    $('#NumMembers').val(num);

    if (num == "2") {
        $('#memberNum_1').show();
    }

    if (num == "3") {
        $('#memberNum_1, #memberNum_2').show();
    }

    if (num == "4") {
        $('#memberNum_1, #memberNum_2,#memberNum_3').show();
    }
}

function CheckFormAtLeastOne() {
    var hp = ""; var mp = ""; var op = "";
    $('#HomePhone').exists(function () { hp = $('#HomePhone').val() });
    $('#OfficePhone').exists(function () { op = $('#OfficePhone').val() });
    $('#MobilePhone').exists(function () { mp = $('#MobilePhone').val() });

    //var op = $('#OfficePhone').val();
    //var mp = $('#MobilePhone').val();

    if (hp == "" && op == "" && mp == "") {
        alert("Enter either home, office or mobile phone num");
        return false;
    }


}



//http://stackoverflow.com/questions/1937927/disable-backspace-in-textbox-via-javascript
function preventBackspace(e) {
    var evt = e || window.event;
    if (evt) {
        var keyCode = evt.charCode || evt.keyCode;
        if (keyCode === 8) {
            if (evt.preventDefault) {
                evt.preventDefault();
            } else {
                evt.returnValue = false;
            }
        }
    }
}

// shared function get current path
function getVirtualDir() {
    var path = $('#svrpath').val();
    return path;
}

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