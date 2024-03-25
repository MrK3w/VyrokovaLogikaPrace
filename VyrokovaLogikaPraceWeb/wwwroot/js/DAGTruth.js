﻿// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var finished = true;

function handleButtonDrawGraphButton() {
    CallAjaxToGetPaths(false);
}

function handleButtonDrawDAGButton() {
    CallAjaxToGetPaths(true);
}

function CallAjaxToGetPaths(isDag) {
    var dataToSend = $('#formula').val();
    dataToSend = transformInputValue(dataToSend);

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
            document.getElementById("zmeny").innerHTML = "";
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
async function createGraph(isDag, nodesData) {
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        //if node does not have parentId then it is root
        if (nodesData[i].ParentId === 0) {
            nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId, color: { background: '#FFD700' } });
        }
        else {
            nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId });
            edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
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
    if (isDag) updateNodes(data, network);
}

function getEdgeColor(nodesData, parentId, label) {
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.Id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.Label[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.Label.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.Operator === '¬' || parentNode.Operator === '¬¬')) {
        edgeColor = 'blue';
    } else if (parentLabelStart !== label) {
        edgeColor = 'orange';
    }
    return edgeColor;
}

function getEdgeColorModified(nodesData, parentId, label) {
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.title[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.title.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.label === '¬' || parentNode.label === '¬¬')) {
        edgeColor = 'blue';
    } else if (parentLabelStart !== label) {
        edgeColor = 'orange';
    }
    return edgeColor;
}

async function updateNodes(data, network) {
    var dataFromGraph = data.nodes.get();
    var changeTitle = modifyNodes(dataFromGraph);
    var nodesToUpdateFull = dataFromGraph.filter(function (node) {
        return changeTitle.includes(node.title);
    });

    console.log(data.nodes.get());
    network.setData(data);

    // Group nodes by their title property
    var groupedNodes = {};
    nodesToUpdateFull.forEach(function (node) {
        if (!groupedNodes[node.title]) {
            groupedNodes[node.title] = [];
        }
        groupedNodes[node.title].push(node);
    });

    // Iterate over each group of nodes
    for (var title in groupedNodes) {
        if (groupedNodes.hasOwnProperty(title)) {
            var nodesToUpdate = groupedNodes[title];
            var i = 0; // Initialize i here
            nodesToUpdate.slice(1).forEach(function (node) {
                i++; // Increment i for each iteration
                data.edges.add({ from: nodesToUpdate[0].id, to: node.parentId, arrows: 'to', color: getEdgeColorModified(dataFromGraph, node.parentId, node.label) });
                dataFromGraph.forEach(function (allNodes) {
                    if (allNodes.parentId == node.id) {
                        allNodes.parentId = nodesToUpdate[0].id;
                        data.edges.add({ from: allNodes.id, to: nodesToUpdate[0].id, arrows: 'to', color: getEdgeColorModified(dataFromGraph, node.parentId, node.label) });
                    }
                });
                data.nodes.remove({ id: node.id });
            });
        }
    }
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
    var changeTitles = []; // Change changeTitle to changeTitles to store multiple titles
    finished = true;

    // Iterate through nodesData and update the IDs based on label
    for (const nodeData of nodesData) {
        const label = nodeData.title;

        //if we already have label stored in map we will do operation
        if (labelToIdMap[label] !== undefined) {
            changeTitles.push(label); // Push label to changeTitles array

            // If you need to set finished to false, uncomment the following line
            // finished = false;
        } else {
            // If label doesn't exist, store the current ID for future reference
            labelToIdMap[label] = nodeData.id;
        }
    }

    return changeTitles; // Return changeTitles array
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