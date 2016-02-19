$(document).ready(function () {

    $('body').on('change', '.access', function () {

        var v = $(this).val();
        var u = $(this).data('user')
        SaveAdminChanges(u, 'access_level', v)
        //alert(v+' '+u);
    })

    $('body').on('change', '.accesscheck', function () {

        var u = $(this).data('user')
        if ($(this).is(":checked")) {

            SaveAdminChanges(u, 'is_active', 'Y')

        } else {
            SaveAdminChanges(u, 'is_active', 'N')
        }


    })

    $('body').on('click', '.resetpassword', function () {
        var u = $(this).data('user')

        var temppassword = prompt("Enter a temp password", "password");

        if (/\s/.test(temppassword)) {
            alert('should not contain spaces');
        } else {

            if (temppassword.length > 0) {

                SaveAdminChanges(u, 'password', temppassword)

            }

        }

    })


});





function SaveAdminChanges(p_user, p_item, p_value) {

    var src = $('#sourcepage').val();
    //var dt = data.substr(4, 11);
    //alert(dt);

    $.ajax({
        type: "POST",
        url: getVirtualDir() + 'Admin/AjaxAdminUpdates',
        data: { username: p_user, item: p_item, value: p_value },
        success: (function (response) {
            alert(response);


        })
    });
}