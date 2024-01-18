function handleButtonPridejFormuliClick() {
    //Get value from input
    var inputValue = $('#FormulaInput').val();

    //check if input is valid
    var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬'];
    var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
    if (regex.test(inputValue)) {
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
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
    document.getElementById('pridejFormuliButton').addEventListener('click', handleButtonPridejFormuliClick);
})