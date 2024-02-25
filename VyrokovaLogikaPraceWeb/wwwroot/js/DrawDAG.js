// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var finished = true;

function handleButtonDrawGraphButton() {
    CallAjaxToGetPaths(false);
}

function handleButtonDrawDAGButton() {
    CallAjaxToGetPaths(true);
}

function CallAjaxToGetPaths(isDag) {
    var userInput = $('#UserInput').val();
    var formula = $('#formula').val();
    var dataToSend = userInput ? userInput : formula;
    dataToSend = dataToSend
        .replace(/&/g, '∧')
        .replace(/\|/g, '∨')
        .replace(/=/g, '≡')
        .replace(/-/g, '¬')
        .replace(/>/g, '⇒')
        .replace(/--/g, '¬¬');
    $('#UserInput').val("");

    if ($('#formula option[value="' + dataToSend + '"]').length === 0) {
        // Create a new option element
        var newOption = $('<option>', {
            value: dataToSend,
            text: dataToSend
        });

        // Append the new option to the dropdown list
        $('#formula').append(newOption);

        // Optionally, you can set the selected value to the newly added option
        $('#formula').val(dataToSend);
    }

    $.ajax({
        url: '?handler=DrawDAG',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(dataToSend),
        success: function (output) {
            var parsedOutput = JSON.parse(output);
            parsedOutput.sort((a, b) => b.Id - a.Id);
            createGraph(isDag, parsedOutput);
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

function sleep(milliseconds) {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
}

//function to draw graph
async function createGraph(isDag, nodesData, isChecked) {
    document.getElementById("zmeny").innerHTML = "";
    Graphik(isDag, nodesData, isChecked);
}

async function Graphik(isDag, nodesData) {
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {

        const nodeData = nodesData[i];

        const nodeId = nodeData.Id;
        const parentId = nodeData.ParentId;
        const operator = nodeData.Operator;
        const label = nodeData.Label;


        //if node does not have parentId then it is root
        if (parentId === 0)
        {
            nodes.add({ id: nodeId, label: operator, title: label, parentId, color: { background: '#FFD700' } });
        }
        else {
            nodes.add({ id: nodeId, label: operator, title: label, parentId });

            // Check if the parent node label starts with the same text up to the length of the current node's label
            const parentNode = nodesData.find(node => node.Id === parentId);
            var tempLabel = label;
            //if parentNode label start with parenthess we need to add that parenthess to temp label to be able to properly check it
            if (parentNode.Label[0] == '(') tempLabel = '(' + tempLabel;
            const parentLabelStart = parentNode ? parentNode.Label.substring(0, tempLabel.length) : null;
            let edgeColor = 'blue';

            // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
            if (parentNode.Operator === '¬' || parentNode.Operator === '¬¬') {
                edgeColor = 'blue';
            } else if (parentLabelStart !== tempLabel) {
                edgeColor = 'orange';
            }
            edges.add({ from: nodeId, to: parentId, arrows: 'to', color: edgeColor });
        }
     
    }

    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
    const data = { nodes, edges };
    var options = {
        interaction: {
            hover: true
        },
        physics: {
            stabilization: true,
        },
        nodes: {
            physics: false,
            size: 30,
            font: {
                size: 16, // Adjust the font size as needed
            },
        },
    };
    
    const network = new vis.Network(container, data, options);

    network.on("afterDrawing", function (ctx) {
        drawOnCanvasLabels(ctx, container); // Call drawOnCanvasLabels and pass ctx
    });
    if(isDag)
    updateNodes(data, network);
}

async function updateNodes(data, network) {
    await sleep(3000);
    var dataFromGraph = data.nodes.get();
    var changeTitle = modifyNodes(dataFromGraph);
    if (finished) return; 
    var nodesToUpdate = dataFromGraph.filter(function (node) {
        return node.title === changeTitle;
    });

    // Update color for filtered nodes
    nodesToUpdate.forEach(function (node) {
        data.nodes.update({ id: node.id, color: { background: 'green' } });
    });
    console.log(data.nodes.get());
    var positions = network.getPositions();
    network.setData(data);

    network.once("stabilizationIterationsDone", function () {
        // Iterate over the positions object and update node positions
        for (const nodeId in positions) {
            if (positions.hasOwnProperty(nodeId)) {
                const position = positions[nodeId];
                network.moveNode(nodeId, position.x, position.y);
            }
        }
    });
    await sleep(3000);

    nodesToUpdate.slice(1).forEach(function (node) {
        data.edges.add({ from: nodesToUpdate[0].id, to: node.parentId, arrows: 'to' });
        dataFromGraph.forEach(function (allNodes) {
            if (allNodes.parentId == node.id) {
                allNodes.parentId = nodesToUpdate[0].id;
                data.edges.add({ from: allNodes.id, to: nodesToUpdate[0].id, arrows: 'to' });
            }
        })
        data.nodes.remove({ id: node.id });
    });
    data.nodes.update({ id: nodesToUpdate[0].id, color: { background: '#97c2fc' } });
    positions = network.getPositions();
    network.setData(data);
    network.once("stabilizationIterationsDone", function () {
        // Iterate over the positions object and update node positions
        for (const nodeId in positions) {
            if (positions.hasOwnProperty(nodeId)) {
                const position = positions[nodeId];
                network.moveNode(nodeId, position.x, position.y);
            }
        }
    });
    updateNodes(data, network);
}

function drawOnCanvasLabels(ctx, container) {
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

function modifyNodes(nodesData) {
    const labelToIdMap = {};
    var changeTitle = "";
    finished = true;
    // Iterate through nodesData and update the IDs based on label
    for (const nodeData of nodesData) {
        const label = nodeData.title;
        //if we already have label stored in map we will do operation
        if (labelToIdMap[label] !== undefined) {
            changeTitle = label;
            finished = false;

            //we will create div to show what we changed
            var newAlertDiv = document.createElement("div");
            newAlertDiv.className = "alert alert-primary";
            newAlertDiv.textContent = "Spojena noda " + changeTitle;

            // Append new alert div to the "zmeny" div
            document.getElementById("zmeny").appendChild(newAlertDiv);

            return changeTitle;
        } else {
            // If label doesn't exist, store the current ID for future reference
            labelToIdMap[label] = nodeData.id;
        }
    }
    
    return changeTitle;
}

$(document).ready(function () {
    //button for 'Vykresli DAG'
    document.getElementById('drawGraphButton').addEventListener('click', handleButtonDrawGraphButton);
    document.getElementById('drawDAGButton').addEventListener('click', handleButtonDrawDAGButton);
    $('#DAGForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
})