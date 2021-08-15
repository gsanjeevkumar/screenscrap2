// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$(document).ready(function(){
    $("#inputUrl").click(function(){
        $(this).val('');
    });
    $("#scrapeUrl").click(function(){
        var url = $("#inputUrl").val();
        location.replace("About/?url="+url);
    });
});
