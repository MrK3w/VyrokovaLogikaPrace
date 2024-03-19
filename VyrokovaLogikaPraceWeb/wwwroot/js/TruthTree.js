function toggleNode(nodeId, treeValue, op, truthValue) {
    var node = document.getElementById('node_' + nodeId);
    var span = node.querySelector('.tf-nc');

    // Toggle between tree.Text and op
    span.innerText = (span.innerText === treeValue + "= " + truthValue) ? op + "= " +truthValue: treeValue + "= " + truthValue;
}

var currentIndex = 0;
var treeContainer = document.getElementById('treeContainer');
var convertedTrees;

function showNextTree() {
    treeContainer = document.getElementById('treeContainer');
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;

    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);

    // Now you can work with convertedTrees like any other JavaScript array
    console.log(convertedTrees);
    currentIndex++;
    if (currentIndex >= convertedTrees.length) {
        currentIndex = convertedTrees.length - 1; // Ensure currentIndex doesn't exceed the last index
        return;
    }
    updateTreeContainer();
    updateButtonState();
}

function showPreviousTree() {
    treeContainer = document.getElementById('treeContainer');
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;

    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);

    // Now you can work with convertedTrees like any other JavaScript array
    console.log(convertedTrees);
    currentIndex--;
    if (currentIndex < 0) {
        currentIndex = 0; // Ensure currentIndex doesn't go below 0
        return;
    }
    updateTreeContainer();
    updateButtonState();
}

function updateTreeContainer() {
    var treeContainer = document.getElementById('treeContainer');
    treeContainer.innerHTML = convertedTrees[currentIndex];
}

function updateButtonState() {
    var prevButton = document.getElementById('prevBtn');
    var nextButton = document.getElementById('nextBtn');
    prevButton.disabled = currentIndex === 0;
    nextButton.disabled = currentIndex === convertedTrees.length - 1;
}