$(function () {
    $(document).on("change", "#dateSelect", function () {
        var selectBox = $("#dateSelect");

        if (selectBox.val() == "2") {
            $("#searchTerm2").css('display', 'inline');
        }
        else {
            $("#searchTerm2").css('display', 'none');
            $("#searchTerm2").val('');
        }
    })
});