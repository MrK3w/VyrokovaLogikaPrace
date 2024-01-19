function handleButtonPridejFormuliClick(event) {
    //Get value from input
    var inputValue = $('#FormulaInput').val();

    inputValue = inputValue
        .replace(/&/g, '∧')
        .replace(/\|/g, '∨')
        .replace(/=/g, '≡')
        .replace(/-/g, '¬')
        .replace(/>/g, '⇒')
        .replace(/--/g, '¬¬');
    //check if input is valid
    $('#FormulaInput').val(inputValue);
}

// Case of interactive tree
$(document).ready(function () {
    // Buttons for adding symbols into texbox
    $(".insert-button").click(function () {
        var buttonText = $(this).data("text");
        var formulaInput = $("#FormulaInput");
        var cursorPos = formulaInput.prop("selectionStart");
        var currentValue = formulaInput.val();

        var newValue =
            currentValue.substring(0, cursorPos) + buttonText + currentValue.substring(cursorPos);

        formulaInput.val(newValue);
    });

    //button for 'Pridej formuli'
    document.getElementById('pridejFormuliButton').addEventListener('click', function (event) {
        handleButtonPridejFormuliClick(event);
    });
})