function BindDropDowns(id, url) {
    $.ajax({
        type: 'Post',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        url: url,
        data: {},
        success: function (data) {
            $(id).empty().append('<option selected="selected" value="0">Please select</option>');

            $.each(data, function (i, index) {
                $(id).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            });
        }
    });
}


function Save(url, data) {

    $.ajax({
        type: 'Post',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        url: url,
        data: data,
        success: function (data) {

            if (data[0].Key == "Success") {
                $('.message').addClass('Metronic-alerts alert alert-success fade in');
                $('.message').show();
                $('.textboxes').val('');
                $('.errorSaveMessages').html(data[0].Message).css('color', 'green');
            }
            if (data[0].Key == "Failure") {
                $('.message').addClass('Metronic-alerts alert alert-danger fade in');
                $('.message').show();
                $('.errorSaveMessages').html(data[0].Message).css('color', 'red');
            }
            else {
                $.each(data, function (i, index) {
                    $('#error' + data[i].Key).html(data[i].Message).css('color', 'red');
                });
            }
        }

    });
}


function Delete(url, id) {

    $.ajax({
        type: 'Post',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        url: url,
        data: JSON.stringify({ id: id }),
        success: function (data) {
            $('.red').trigger('click');
        },

    });
}

function GetValue(id, url,textid) {
    $.ajax({
        type: 'Post',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        url: url,
        data: JSON.stringify({id:id}),
        success: function (data) {
            $(textid).val(data);
        }
    });
}



function ClearErrors() {
    $('.textboxes').val('');
    $('.errorMessages').html('');
    $('.errorSaveMessages').html('');
    $('.message').hide();
}


$('.dropdowns').on('change', function () {
    $('.errorMessages').html('');
});

$("input:file").change(function () {
    $('.errorMessages').html('');
});

$('.textboxes').keypress(function () {
    $('.errorMessages').html('');
    $('.message').hide();
    $('.errorSaveMessages').html('');
});

$('#btnCancel').click(function () {
    location.reload();
});


//called when key is pressed in textbox
$('.txtNumbersOnly').keypress(function (e) {
    //if the letter is not digit then display error and don't type anything
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        ////display error message
        //$("#errmsg").html("Digits Only").show().fadeOut("slow");
        return false;
    }
});

$('.txtAlphabetsOnly').keypress(function (e) {
    //if the letter is not digit then display error and don't type anything
    if (e.which == 8 && e.which == 0 && (e.which > 48 || e.which < 57)) {
        ////display error message
        //$("#errmsg").html("Digits Only").show().fadeOut("slow");
        return false;
    }
});

$('input').iCheck({
    checkboxClass: 'icheckbox_square-green',
    radioClass: 'iradio_square-green',
    increaseArea: '10%' // optional
});