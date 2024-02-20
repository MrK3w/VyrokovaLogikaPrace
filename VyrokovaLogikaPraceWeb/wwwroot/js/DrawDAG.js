﻿// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var finishedList = false;
var lbl = "";
var changedLabels = [];
var firstRun;
function handleButtonDrawGraphButton() {
    const isChecked = document.getElementById("labelCheckbox").checked;
    CallAjaxToGetPaths(false, isChecked);
}

function handleButtonDrawDAGButton() {
    const isChecked = document.getElementById("labelCheckbox").checked;
    CallAjaxToGetPaths(true, isChecked);
}

function CallAjaxToGetPaths(isDag, isChecked) {
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
            parsedOutput = modifyNodes(parsedOutput);
            parsedOutput.sort((a, b) => b.Id - a.Id);
            // Log the parsed object with Unicode characters properly displayed
            

            createGraph(isDag, output, isChecked);
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

function Node(id, label, parentId = null) {
    this.id = id;
    this.label = label;
    this.parentId = parentId;
}

function sleep(milliseconds) {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
}

//function to draw graph
async function createGraph(isDag, dagPaths, isChecked) {
    // Parse the JSON string to get the array of node objects
    var nodesData = JSON.parse(dagPaths);
    changedLabels = [];
    firstRun = true;
    finishedList = false;
    document.getElementById("zmeny").innerHTML = "";

    Graphik(isDag, nodesData, isChecked)

    
}

async function Graphik(isDag, nodesData, isChecked) {

    if (isDag) {
        nodesData = modifyNodes(nodesData);
    }
    firstRun = false;
    const existingNodes = {};
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();
    nodesData.sort((a, b) => b.Id - a.Id);
    console.log('Node Object:', nodesData);
    if (finishedList) return;
    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        const nodeData = nodesData[i];
        const nodeId = nodeData.Id;
        const parentId = nodeData.ParentId;
        const operator = nodeData.Operator;
        const label = nodeData.Label;
        if (parentId === 0) {
            // Create a new root node
            if (isChecked) {
                nodes.add({ id: nodeId, label: label, color: { background: '#FFD700' } });
            }
            else nodes.add({ id: nodeId, label: operator, color: { background: '#FFD700' } });
        } else {
            // Create a new node if it doesn't exist
            if (!existingNodes[nodeId]) {
                existingNodes[nodeId] = true;
                if (label != lbl) {
                    if (isChecked) {
                        nodes.add({ id: nodeId, label: label, parentId });
                    }
                    else {
                        nodes.add({ id: nodeId, label: operator, parentId });
                    }
                }
                else {
                    if (isChecked) {
                        nodes.add({ id: nodeId, label: label, parentId, color: { background: '#AAD7FF' } });
                    }
                    else {
                        nodes.add({ id: nodeId, label: operator, parentId, color: { background: '#AAD7FF' } });
                    }
                }
            }

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
    const options = {};
    const network = new vis.Network(container, data, options);
    await sleep(3000);
    Graphik(isDag, nodesData, isChecked);
}

function modifyNodes(nodesData) {
    const labelToIdMap = {};
    var changedLabel = "";
    // Iterate through nodesData and update the IDs based on label
    for (const nodeData of nodesData) {
        const label = nodeData.Label;
        const oldId = nodeData.Id;
        if (labelToIdMap[label] !== undefined) {
            if (changedLabel == "")
            {
                lbl = nodeData.Label;
                if (!changedLabels.includes(nodeData.Label)) {
                    changedLabel = nodeData.Label;
                    changedLabels.push(nodeData.Label);
                    var newAlertDiv = document.createElement("div");
                    newAlertDiv.className = "alert alert-primary";
                    newAlertDiv.textContent = "Spojena noda " + nodeData.Label;

                    // Append new alert div to the "zmeny" div
                    document.getElementById("zmeny").appendChild(newAlertDiv);
                }
            }            
            if(!firstRun)
            if (changedLabel == nodeData.Label) {
                nodeData.Id = labelToIdMap[label];
            }
            // If label already exists, override the ID of the next node
        } else {
            // If label doesn't exist, store the current ID for future reference
            labelToIdMap[label] = nodeData.Id;
        }

        // Update ParentId of nodes that have their ParentId matching the oldId
        for (const node of nodesData) {
            if (node.ParentId === oldId) {
                node.ParentId = nodeData.Id;
            }
        }
    }
    if(changedLabel == "") finishedList = true;
    return nodesData;
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