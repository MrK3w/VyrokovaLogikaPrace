// Function to escape special characters in the validSymbols array
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function handleButtonZmenaTextuClick() {
    //Get value from input
    var inputValue = $('#TreeInput').val();

    //check if input is valid
    var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬'];
    var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
    if (regex.test(inputValue)) {
        if (globalInput) globalInput.text(inputValue);
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
}

//remove node/s after clickng on Odstran button
function handleButtonOdstranNoduClick() {

    if (globalInput) {
        globalInput.closest('li').remove();
    }
}

//Create formula from just create tree
function handleButtonVytvorFormuliClick() {
    //get html tree from page
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm")[0].innerHTML;

    //check if each node is filled with valid content
    if (isContentPresent()) {
        alert("Některá z nod je nevyplněná!");
        return;
    }
    //ajax calling function
    $.ajax({
        url: '?handler=CreateFormula',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(elements),
        success: function (data) {
            console.log('Success:', data);
            // Put the convertedTree to the element with id 'createdTree'
            document.getElementById('createdTree').innerHTML = data.convertedTree;
            //Put formula to the element with id 'formula'
            document.getElementById('formula').innerHTML = data.formula;

            // set functions to node
            $(".tf-nc").on("click", function () {
                // Set it to an empty string to reset to default
                $(".tf-nc").css("border-color", "");
                //saving current node into global property
                globalInput = $(this);
                //setting current node into blue border
                $(this).css("border-color", "blue");
            });
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

//check if we have some unfilled node
function isContentPresent() {
    var spans = document.querySelectorAll('.tf-nc');

    for (var i = 0; i < spans.length; i++) {
        if (spans[i].textContent.trim() === '▭') {
            return true;
        }
    }

    return false;
}

//Add new node 
function handleButtonPridejNoduClick() {
    // Find the span element of just selected ndoe
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
        //new ul for case if we don't have any child node
        newUl.appendChild(newLi);

        //If we don't have any child node we can create left one in case that content of node is not literal
        if (directChildLiElements == 0) {
            //if node is literal we don't create new node
            if (/^[a-zA-Z]+$/.test(spanElement.textContent)) {
                alert("Literal nemuze mit potomka!");
                return;
            }
            spanElement.insertAdjacentElement('afterend', newUl)
        }
        //if we have left child we can create right child in case that content of node is not negation
        if (directChildLiElements.length == 1) {
            //if it is negation we don't have right node, hence we don't create new node
            if (spanElement.textContent == '¬' || spanElement.textContent == "¬¬") {
                alert("Negace muze mit jen jednoho potomka!");
                return;
            }
           foundUl.appendChild(newLi);
        }
        //if we have two nodes we can't create another one
        else if (directChildLiElements.length == 2) {
            alert("Maximalni pocet potomku!");
        }
    }
    //in case we don't have tree we create new tree
    else {
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

    $('#formulaForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });

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