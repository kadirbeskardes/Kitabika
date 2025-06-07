$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $('input[name="quantity"]').on('change', function () {
        var max = parseInt($(this).attr('max'));
        var min = parseInt($(this).attr('min'));
        var value = parseInt($(this).val());

        if (value > max) {
            $(this).val(max);
        } else if (value < min) {
            $(this).val(min);
        }
    });

    $('form').submit(function () {
        $(this).find('button[type="submit"]').prop('disabled', true);
    });
});