﻿const labelIdMap = new Map();
var network;
var data;
var globalInput;
var globalEdge;
var clickedSecondNode = false;
var clickedEdge = false;
var newButton = null;
var formula = null;

function handleButtonDrawDAGButton() {
    createGraph();
}
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


//function to draw graph
function createGraph()
{
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
    data = { nodes, edges };
    var options = {
            interaction: {
    hover: true
            },
            physics:
    {
    stabilization: true,
            },
            nodes:
    {
    physics: false,
                size: 30,
                font:
        {
        size: 16, // Adjust the font size as needed
                },
            },
        };

    network = new vis.Network(container, data, options);
    // Add onclick event

    network.on("afterDrawing", function(ctx) {
        drawOnCanvasLabels(ctx, container); // Call drawOnCanvasLabels and pass ctx
    });

    network.on("click", function (params) {
        if (!(params.nodes[0] === undefined) && !clickedSecondNode) {
                var nodeId = params.nodes[0];
                if (nodeId !== undefined) {
                    handleNodeClick(nodeId);
                }
        }
        else if (!(params.edges[0] === undefined))
        {
            var edgeId = params.edges[0];
            handleEdgeClick(edgeId);
        }
    });
}

function addNewNode(newNodeId, newNodeLabel, newNodeTitle, color) {
    const x = Math.random() * 100; // Adjust 1000 to the width of your container
    const y = Math.random() * 100; // Adjust 1000 to the height of your container
    var nodes = data.nodes.get();
    var id;
    if (nodes.length == 0) {
        id = 0;
    }
    else id = -1;
    const newNode = {
        id: newNodeId,
        label: newNodeLabel,
        title: newNodeTitle,
        color: { background: color },
        parentId: id,
        x: x,
        y: y,
        side: 'undefined'
    };
    // Add the new node to the DataSet
    data.nodes.add(newNode);
}

function addNewEdge(nodeId, parentId, color) {
    const newEdge = {
        from: nodeId,
        to: parentId,
        arrows: 'to',
        color: { color: color } // Example color, customize as needed
    };
    const existingNode = data.nodes.get(nodeId);
    if (existingNode.parentId != 0 || existingNode.parentId != -1) {
        existingNode.parentId2 = existingNode.parentId;
    }
    else {
        existingNode.parentId2 = existingNode.parentId;
    }
    existingNode.parentId = parentId;
    data.nodes.update(existingNode);
    console.log(data.nodes.get());
    data.edges.add(newEdge);
}

function drawOnCanvasLabels(ctx, container)
{
    // Draw text onto the canvas
    const canvasWidth = container.offsetWidth;
    const canvasHeight = container.offsetHeight;
    const centerX = canvasWidth / 2;
    const centerY = canvasHeight / 2;
    ctx.font = "16px Arial";
    ctx.fillStyle = "black";
    ctx.fillText("Barvy hran:", 100 - centerX, 100 - centerY); // Adjust position as needed
    ctx.fillStyle = "blue";
    ctx.fillRect(195 - centerX, 111 - centerY, 10, 10); // Sample color indicator
    ctx.fillText("Levá strana", 100 - centerX, 120 - centerY); // Adjust position as needed
    ctx.fillStyle = "orange";
    ctx.fillRect(195 - centerX, 133 - centerY, 10, 10); // Sample color indicator
    ctx.fillText("Pravá strana", 100 - centerX, 140 - centerY); // Adjust position as needed
    // Add more color descriptions as needed
}


function handleButtonVytvorFormuliClick() {

    var nodes = data.nodes.get();
    var edges = data.edges.get();
    for (const node of nodes) {
        // Check if the node's label matches the specified label
        if (node.label === '▭') {
            alert("Nejsou vyplněné všechny uzly");
            return;
        }
    }

    getFormulaFromNodes();
}

function getFormulaFromNodes() {
    var nodes = data.nodes.get();
    var edges = data.edges.get();
    removeTitles();
    while (!haveAllNodesFilledTitle(nodes)) {

    console.log(nodes);
    console.log(edges);
        for (let i = 0; i < nodes.length; i++) {
            if (nodes[i].title == '▭') {
                var nodesWithParentId = getNodesByParentId(nodes, nodes[i].id)
                if (haveChildNodesFilledTitle(nodesWithParentId))
                {
                    getSide(edges, nodesWithParentId);
                    modifyNodes(nodes[i], nodesWithParentId);
                }
            }
        }
    }
    var divElement = document.getElementById("zmeny");

    // Assuming data.nodes is an array of nodes
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].parentId === 0) {
            formula = nodes[i].title;
            break; // Exit loop once the first matching node is found
        }
    }

    // Check if the <div> element exists
    if (divElement) {
        // Construct the text to be displayed
        var newText = "Formule je " +formula;

        // Update the content of the <div> element
        if (newButton == null) {
            newButton = $('<input type="submit" class="btn btn-primary flex-fill mb-2" value="Ulož formuli" id="saveFormula"/>');
            $('#buttonContainer').append(newButton);
        }

        // Add any additional logic or event handlers for the new button if needed
        $('#saveFormula').on('click', handleButtonUlozFormuli);
        divElement.textContent = newText;
        divElement.appendChild(newButton);
    } else {
        console.error("Element with ID 'zmeny' not found.");
    }
}

//remove node/s after clickng on Odstran button
function handleButtonUlozFormuli() {
    $.ajax({
        url: '?handler=SaveFormula',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(formula),
        success: function (data) {
            console.log('Success:', data);

            // Clear any existing messages
            $('#message').empty();
            if (data.errors.length === 0) {
                // If no errors, display "Formula saved" message
                $('#message').html('<div class="alert alert-success">Formule uložena</div>');

            }
            else {
                // If there are errors, display the error messages
                var errorHtml = '<div class="alert alert-danger">' +
                    '<h4 class="alert-heading">Nesprávná formule:</h4>' +
                    '<ul>';

                $.each(data.errors, function (index, error) {
                    errorHtml += '<li>' + error + '</li>';
                });

                errorHtml += '</ul></div>';

                $('#message').html(errorHtml);
            }
            $('#saveFormula').remove();
            newButton = null;
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

function removeTitles() {
    var nodes = data.nodes.get();
    for (let i = 0; i < nodes.length; i++) {
        if (!isAlphabet(nodes[i].title)) {
            const existingNode = data.nodes.get(nodes[i].id);
            existingNode.title = '▭';
            data.nodes.update(existingNode);
        }
    }
}



function getNodesByParentId(nodes, parentId) {
    // Filter nodes based on parentId
    let filteredNodes = nodes.filter(node => node.parentId === parentId);

    // If no nodes are found, try filtering again with a different condition
    const secondaryFilteredNodes = nodes.filter(node => node.parentId2 === parentId);
    
    // Concatenate the secondary filtered nodes with the initial filtered nodes
    filteredNodes = filteredNodes.concat(secondaryFilteredNodes);

    return filteredNodes;
}

function getSide(edges, nodesWithParentId) {
    for (let i = 0; i < nodesWithParentId.length; i++) {
        for (let j = 0; j < edges.length; j++) {
            if (nodesWithParentId[i].id === edges[j].from && nodesWithParentId[i].parentId === edges[j].to) {
                const existingNode = nodesWithParentId[i];
                if (edges[j].color.color === 'orange') existingNode.side = 'right';
                else if (edges[j].color.color === 'blue') existingNode.side = 'left';
                // Assuming you have some mechanism to update the node in your own data structure
                // For example, if nodesWithParentId is an array, you can directly update the node:
                nodesWithParentId[i] = existingNode;
                // If nodesWithParentId is an object or Map, update it accordingly
            }
        }
    }

}

function haveAllNodesFilledTitle(nodes) {
    for (let i = 0; i < nodes.length; i++) {
        if (nodes[i].title == '▭') return false;
    }
    return true;
}

function haveChildNodesFilledTitle(nodes) {
    for (let i = 0; i < nodes.length; i++)
    {
        if (nodes[i].title == '▭') return false;
    }
    return true;
}

function modifyNodes(node, nodesWithParentId) {
    if (node.label != "¬" && node.label != "¬¬") {
        if (nodesWithParentId[0].side == "left") {
            node.title = '(' + nodesWithParentId[0].title + node.label + nodesWithParentId[1].title + ')';
        }
        else {
            node.title = '(' + nodesWithParentId[1].title + node.label + nodesWithParentId[0].title + ')';
        }
    }
    else {
        if (isAlphabet(nodesWithParentId[0].label)) {
            node.title = node.label + nodesWithParentId[0].title;
        }
        else {
            node.title =  node.label + '(' + nodesWithParentId[0].title + ')';
        }
    }
    const existingNode = data.nodes.get(node.id);
    existingNode.title = node.title;
    data.nodes.update(existingNode);
    return;

}

function isAlphabet(input) {
    // Regular expression to match only alphabets (case-insensitive)
    const regex = /^[a-zA-Z]+$/;
    return regex.test(input);
}


async function handleButtonPridejVetev(color) {

    if (globalInput) {
        clickedSecondNode = true;
        // Get all existing node IDs
        // Wait for user click
        const nodeId = await new Promise(resolve => {
            network.once("click", function (params) {
                resolve(params.nodes[0]);
            });
        });
        if (nodeId == undefined) {
            alert("Pro přidání větve je třeba kliknout na uzel!")
            return;
        }
        clickedSecondNode = false;
        if (nodeId == globalInput.id) {
            alert("Uzel nemůže navázat sám na sebe!");
            return;
        }

        const edges = data.edges.get();

        for (let i = 0; i < edges.length; i++) {
            if (edges[i].to == nodeId) {
                console.log(edges[i].color);
                if (edges[i].color.color == color) {
                    if (color == 'blue')
                        alert("Tento uzel už má levou větev.");
                    else alert('Tento uzel už má pravou větev. ');
                    return;
                }
            }
        }

        addNewEdge(globalInput.id, nodeId, color);

    }
}

function handleButtonPridejNodu() {
    var newId;
    var color;
    var nodeIds = data.nodes.getIds();
    if (nodeIds.length == 0) {
        newId = 1;
        color = '#FFD700';
    }
    // Find the largest ID among existing nodes
    else {
        var largestNodeId = Math.max(...nodeIds);

        // Calculate the new ID by adding 1 to the largest ID
        newId = largestNodeId + 1;
        color = '#97c2fc';
    }
    addNewNode(newId, '▭', '▭', color)
}

function handleButtonOdstranNoduClick() {
    var id = globalInput.id;
    console.log(edges);
    var edges = data.edges.get();
    console.log(edges);
    if (globalInput) {
        data.nodes.remove({ id: id });

        for (let i = 0; i < edges.length; i++) {
            if (edges[i].from == id || edges[i].to == id) {
                data.edges.remove({ id: edges[i].id });
            }
        }
    }
    console.log(edges);
}

function handleButtonOdstranVetevClick() {
    data.edges.remove(globalEdge);
    var nodes = data.nodes.get();



    for (let i = 0; i < nodes.length; i++) {
        if (nodes[i].parentId == globalEdge.from || nodes[i].parentId == globalEdge.to || nodes[i].parentId2 == globalEdge.from || nodes[i].parentId2 == globalEdge.to) {
            const existingNode = data.nodes.get(nodes[i].id);
            existingNode.parentId = -1;
            existingNode.parentId2 = -1;
            data.nodes.update(existingNode);
        }


    }
}

function handleButtonZmenaTextuClick() {
    //Get value from input
    var inputValue = $('#TreeInput').val();

    //check if input is valid
    var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬', '▭'];
    var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
    if (regex.test(inputValue)) {
        if (globalInput) {
            if (inputValue == '▭') {
                globalInput.text(inputValue);
                return;
            }
            if (/^[a-zA-Z]+$/.test(inputValue)) {
                // Update label based on spot condition
                data.nodes.update({ id: globalInput.id, label: inputValue, title: inputValue });
            } else {
                // Update label without condition
                data.nodes.update({ id: globalInput.id, label: inputValue, title: '▭' });
            }
        }
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
}

function handleNodeClick(nodeId) {
     globalInput = data.nodes.get(nodeId);
}

function handleEdgeClick(edgeId) {
    globalEdge = data.edges.get(edgeId);
}

$(document).ready(function() {
    //button for 'Vykresli DAG'
    createGraph();
    document.getElementById('drawDAGButton').addEventListener('click', handleButtonDrawDAGButton);
    $('#DAGForm').submit(function(event) {
        // Prevent the default form submission
        event.preventDefault();
    });
    // Buttons for adding symbols into texbox
    $(".insert-button").click(function () {
        var buttonText = $(this).data("text");
        $("#TreeInput").val(buttonText);
    });

    //button for 'Zmena textu'
    document.getElementById('zmenaTextuButton').addEventListener('click', handleButtonZmenaTextuClick);

    //button for 'Odstra nodu'
    document.getElementById('odstranNoduButton').addEventListener('click', handleButtonOdstranNoduClick);

    document.getElementById('odstranVetevButton').addEventListener('click', handleButtonOdstranVetevClick);

    document.getElementById('pridejLevouVetevButton').addEventListener('click', function () {
        handleButtonPridejVetev('blue');
    });

    document.getElementById('pridejPravouVetevButton').addEventListener('click', function () {
        handleButtonPridejVetev('orange');
    });

    document.getElementById('PridejNoduButton').addEventListener('click', function () {
        handleButtonPridejNodu();
    });

    document.getElementById('vytvorFormuliButton').addEventListener('click', handleButtonVytvorFormuliClick);
})