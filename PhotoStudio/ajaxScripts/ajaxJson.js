function fnIUDOperation(formData, ControlURL)
{
    var result = null;
    $.ajax({
        type: "POST",
        url: ControlURL,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            result = response;
        },
        error: function () {
            result = { "error": true, "message": "Error occur while operating:" + ControlURL };
        }

    });
    alert(result.success);
    return result;
}