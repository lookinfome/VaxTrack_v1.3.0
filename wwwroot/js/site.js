// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



/**
 * exposed: to support views
 * purpose: to add comment under selected ticket
 */
$(document).ready(function() {
    $('#loadPartialView').click(function() {
        $('#partialViewContainer').load('/Support/LoadPartialView');
    });

    $(document).on('submit', '#SupportCommentForm', function(event) {
        event.preventDefault(); // Prevent the form from submitting the traditional way
        var formData = $(this).serialize(); // Serialize the form data

        // Extract the user ID from the URL
        var url = window.location.href;
        var userId = url.substring(url.lastIndexOf('/') + 1);

        var supportid = $('input[name="supportid"]').val();

        $.ajax({
            url: '/Support/' + userId + '/TicketDetails/' + supportid + '/AddNewComment',
            type: 'POST',
            data: formData,
            success: function(response) {
                $('#SupportCommentForm')[0].reset();
                $('.comments-list').html(response);
            },
            error: function(xhr, status, error) {
                alert('userId'+userId+', supportId'+supportid);
                // alert('Something went wrong: ' + xhr.responseText);
            }
        });
    });

});

$(document).ready(function() {
    $('#loadPartialView').click(function() {
        $('#partialViewContainer').load('/SupportManager/LoadPartialView');
    });

    $(document).on('submit', '#SupportCommentAdminForm', function(event){
        event.preventDefault(); // Prevent the form from submitting the traditional way
        var formData = $(this).serialize(); // Serialize the form data

        // Extract the user ID from the URL
        var url = window.location.href;
        var userId = url.substring(url.lastIndexOf('/') + 1);

        var supportid = $('input[name="supportid"]').val();

        $.ajax({
            url: '/SupportManager/' + userId + '/TicketDetails/' + supportid + '/AddNewComment',
            type: 'POST',
            data: formData,
            success: function(response) {
                $('#SupportCommentAdminForm')[0].reset();
                $('.comments-list').html(response);
            },
            error: function(xhr, status, error) {
                alert(status);
                alert(error);
                alert('Support Manager: userId'+userId+', supportId'+supportid);
                // alert('Something went wrong: ' + xhr.responseText);
            }
        });

    });
});