
function SysShowAlert(message, alerttype) {
    $('#MessageBar').append('<div id="alertdiv" class="alert ' + alerttype + '"><a class="close" data-dismiss="alert">×</a><span>' + message + '</span></div>')

    var Time = 5000;

    if(alerttype=="alert-warning")
    {
        Time = 3500;
    }

    setTimeout(function () {
        $("#alertdiv").remove();
    }, Time);

}


function ExistsSession() {

    var Chk = false;

    $.ajax({
        type: "POST",
        url: "../Login/ExistsSession",
        dataType: 'Json',
        async: false,
        timeout: 10000,
        success: function (data) {

            if (data == "1")
            {
                Chk = true;
            }
        }
    });



    if (Chk)
    {
        setTimeout(function () {
            ExistsSession();
        }, 10000);
    }
    else 
    {
        $('#LogOutMsgModal').modal('show');
        $('#LogOutMsgModal').modal({
            keyboard: false
        })

        
    }
}

function UserLogin() {
    document.location.href = "../Login/";
}
