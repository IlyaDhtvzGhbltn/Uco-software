$(document).ready(
    $('#get_table_end').on("click", () =>
    {
        $.get('/more').done(content =>
        {
            $('#info_table tr:last').after(content);
        });
    })
    )