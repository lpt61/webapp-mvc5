/// <reference path="jquery-1.10.2.js" />
/// <reference path="jquery-ui-1.12.1.js" />

$(function () {
    $("#album-list img").mouseover(function () {
        $(this).animate({ height: '+=25', width: '+=25' })
               .animate({ height: '-=25', width: '-=25' });
    });
});

$(function () {
    $("[data-autocomplete-source]").each(function () {
        var target = $(this);
        target.autocomplete({ source: target.attr("data-autocomplete-source") });
    });
});


//jQuery function for jQuery SEARCH form, no template
$(function () {
    ////func for Jquery form (no Ajax), to request HTML from server
    //$("#artistSearch").submit(function (event) {
    //    //we want to handle the event manually, so we have to prevent default behavior of the event 
    //    //(in this case, prevent the frm from submitting directly to server)
    //    event.preventDefault();

    //    var form = $(this);
    //    //the load() takes value of "action" attribute from the form, and takes the input values the form as query string for the "action"
    //    //the input value is concatenated into a string by serialize();
    //    $("#searchresults").load(form.attr("action"), form.serialize());
    //});
});

////jQuery function for jQuery SEARCH form, using Mustache.js template
$(function () {
    //$("#artistSearch").submit(function (event) {
    //    event.preventDefault();

    //    var form = $(this);

    //    //The getJSON method issues an HTTP GET request, deserializes the JSON response into an object, 
    //    //then invokes the callback method passed as the third parameter
    //    //In the view (Home/Index), you must create markups to contain this returned object 
    //    $.getJSON(  form.attr("action"), 
    //                form.serialize(), 
    //                function (data) {
    //                    //to_html() of Mustache combines the template with the JSON data to produce markup.
    //                    var html = Mustache.to_html($("#artistTemplate").html(), { artists: data });
    //                    //takes the template output and places the output in the search results element
    //                    $("#searchresults").empty().append(html);
    //                });
    //});
});

////Ajax function for jQuery SEARCH form, using Mustache.js template
//$(function () {
//    $("#artistSearch").submit(function (event) {
//        event.preventDefault();

//        var form = $(this);
//        $.ajax({
//            url: form.attr("action"),
//            data: form.serialize(),
//            beforeSend: function () { $("#ajax-loader").show(); },
//            complete: function () { $("#ajax-loader").hide(); },
//            error: searchFailed,
//            success: function (data) {
//                var html = Mustache.to_html($("#artistTemplate").html(),
//                { artists: data });
//                $("#searchresults").empty().append(html);
//            }
//        });
//    });
//});

//function searchFailed() {
//    $("#searchresults").html("Sorry, there was a problem with the search.");
//};