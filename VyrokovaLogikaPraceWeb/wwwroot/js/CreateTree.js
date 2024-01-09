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

function handleButtonVytvorFormuliClick() {
    $.ajax({
        url: '/CreateTree?handler=CreateFormula',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        data: "asdsad",
        dataType: "text",
        success: function (data) {
            console.log('Success:', data);
            // Handle the response from the server
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

function handleButtonPridejNoduClick() {

    // Find the span element
    var spanElement = document.querySelector('.tf-nc[style="border-color: blue;"]');
    if (spanElement) {
        //get to parent
        var parent = spanElement.parentNode;
        var foundUl;
        for (var i = 0; i < parent.childNodes.length; i++) {
            var childNode = parent.childNodes[i];
            // Check if the child node is an element node and has the tag name 'ul'
            if (childNode.nodeType === 1 && childNode.tagName.toLowerCase() === 'ul') {
                console.log('Found nested ul element:', childNode);
                foundUl = childNode;
                break;  // Stop iterating after finding the first ul element
            }
        }

        //count direct childs
        directChildLiElements = 0;
        if (foundUl != undefined) {
            directChildLiElements = Array.from(foundUl.children).filter(function (child) {
                return child.tagName.toLowerCase() === 'li';
            });
        }
      
        //creating new node
        var newUl = document.createElement('ul');
        var newLi = document.createElement('li');
        var newSpan = document.createElement('span');
        newSpan.className = 'tf-nc';
        newSpan.textContent = ' ▭ ';
        //new li node for case when we already have one node
        newLi.appendChild(newSpan);
        //new ul for case if we don't have any child ndoe
        newUl.appendChild(newLi);

        //If we don't have any child node we can create left one in case that content of node is not literal
        if (directChildLiElements == 0) {
            if (/^[a-zA-Z]+$/.test(spanElement.textContent)) {
                alert("Literal nemuze mit potomka!");
                return;
            }
            spanElement.insertAdjacentElement('afterend', newUl)
        }
        //if we have left child we can create right child in case that content of node is not negation
        if (directChildLiElements.length == 1) {
            if (spanElement.textContent == '¬' || spanElement.textContent == "¬¬") {
                alert("Negace muze mit jen jednoho potomka!");
                return;
            }
           foundUl.appendChild(newLi);
        }
        //if we have two childs we can't create another one
        else if (directChildLiElements.length == 2) {
            alert("Maximalni pocet potomku!");
        }
} else {
        var divTree = document.createElement('div');
        divTree.className = 'tf-tree tf-gap-sm';

        var listItem = document.createElement('li');

        var spanElement = document.createElement('span');
        spanElement.className = 'tf-nc';
        spanElement.style.borderColor = 'blue';
        spanElement.textContent = ' ▭ ';

        // Append elements to create the hierarchy
        listItem.appendChild(spanElement);
        divTree.appendChild(listItem);

        // Append the main tree div to the container with id "createdTree"
        document.getElementById('createdTree').appendChild(divTree);
    }

    // set functions to node
    $(".tf-nc").on("click", function () {
        // Set it to an empty string to reset to default
        $(".tf-nc").css("border-color", "");
        //saving current node into global property
        globalInput = $(this);
        //setting current node into blue border
        $(this).css("border-color", "blue");
    });
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

    document.getElementById('vytvorFormuliButton').addEventListener('click', handleButtonVytvorFormuliClick);

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