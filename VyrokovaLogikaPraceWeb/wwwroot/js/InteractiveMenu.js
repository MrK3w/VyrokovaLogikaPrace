$(document).ready(function () {
    $('#syntacticTreeDropdown, #syntacticTreeSubMenu').hover(function () {
        $('#syntacticTreeSubMenu').addClass('show');
    }, function () {
        $('#syntacticTreeSubMenu').removeClass('show');
    });

$('#dagDropdown, #dagSubMenu').hover(function () {
    $('#dagSubMenu').addClass('show');
    }, function () {
    $('#dagSubMenu').removeClass('show');
    });
});

// Redirect on click
$('#syntacticTreeDropdown').on('click', function () {
    window.location.href = '/SyntacticTree';
});

// Redirect on click for DAG
$('#dagDropdown').on('click', function () {
    window.location.href = '/DAG';
});
