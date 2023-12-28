// Function to escape special characters in the validSymbols array
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function handleButtonZmenaTextuClick() {
    // Perform your action here
    var inputValue = $('#TreeInput').val();
    var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬'];
    var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
    if (regex.test(inputValue)) {
        if (globalInput) globalInput.text(inputValue);
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
}

function handleButtonOdstranNoduClick() {

    if (globalInput) {
        globalInput.closest('li').remove();
    }
}

//TODO adding new secondary node and cannot add new node if there are already two nodes

function handleButtonPridejNoduClick() {

    // Find the span element
    var spanElement = document.querySelector('.tf-nc[style="border-color: blue;"]');
    console.log(spanElement);
    if (spanElement) {
;
        // Find the nearest ul element
        var nearestUl = spanElement.closest('ul');
        console.log(nearestUl);
        if (nearestUl) {
            // Create a new ul element and append it after the nearest ul
            var newUl = document.createElement('ul');
            var newLi = document.createElement('li');
            var newSpan = document.createElement('span');
            newSpan.className = 'tf-nc';
            newSpan.textContent = ' ▭ ';
            newLi.appendChild(newSpan);
            newUl.appendChild(newLi);

            //added new node 
            spanElement.insertAdjacentElement('afterend',newUl)

            // Check if the nearestUl has a next sibling before inserting
            $(".tf-nc").on("click", function () {
                // Set it to an empty string to reset to default
                $(".tf-nc").css("border-color", "");
                //saving current node into global property
                globalInput = $(this);
                //setting current node into blue border
                $(this).css("border-color", "blue");
            });
            
          
        } else {
            console.error('No parent ul found for the span element.');
        }
    } else {
        console.error('Span element not found.');
    }
}

var globalInput; // Declare globalInput

function showDialog(title, message) {
    // You can use a library like Bootstrap modal or a custom dialog implementation
    // For simplicity, using the built-in alert function here
    alert(title + "\n\n" + message);
}

// Case of interactive tree
$(document).ready(function () {

    // Buttons for adding symbols into texbox
    $(".insert-button").click(function () {
        var buttonText = $(this).data("text");
        $("#TreeInput").val(buttonText);
    });

    //button for 'Zmena textu'
    document.getElementById('zmenaTextuButton').addEventListener('click', handleButtonZmenaTextuClick);

    //button for 'Odstra nodu'
    document.getElementById('odstranNoduButton').addEventListener('click', handleButtonOdstranNoduClick);

    document.getElementById('pridejNoduButton').addEventListener('click', handleButtonPridejNoduClick);

    //function to add into nodes click function
    $(".tf-nc").on("click", function () {
        // Set it to an empty string to reset to default
        $(".tf-nc").css("border-color", "");
        //saving current node into global property
        globalInput = $(this);
        //setting current node into blue border
        $(this).css("border-color", "blue");
    });
})