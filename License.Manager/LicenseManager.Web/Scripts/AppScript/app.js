var app = {
    createLicense: function () {
        $('#c-key').text('');
        $('#c-created').hide();
        $('#c-created').text('');
        var name = $('#c-name').val();
        var email = $('#c-email').val();
        if (!name) {
            alert('Please enter name');
            return;
        }
        if (!email) {
            alert('Please enter email');
            return;
        }
        $.ajax({
            url: "/Home/CreateLicense?name=" + name + "&email=" + email,
            contentType: 'application/json; charset=utf-8',
            //dataType: 'application/json',
            type: 'POST',
            success: function (result) {
                if (result.Success) {
                    $('#c-key').text(result.Key);
                    $('#c-created').show();
                    $('#c-created').text('Created!');
                } else {
                    alert('Error while getting license!');
                }
            },
            error: function (xhr, status, exception) {
                console.log(xhr);
                console.log("Error: " + exception + ", Status: " + status);
            }
        });
    }
};