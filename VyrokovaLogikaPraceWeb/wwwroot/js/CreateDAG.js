const labelIdMap = new Map();
var network;
var data;
var globalInput;
var globalEdge;
var clickedSecondNode = false;
var clickedEdge = false;

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
    const newNode = {
        id: newNodeId,
        label: newNodeLabel,
        title: newNodeTitle,
        color: { background: color }
    };
    // Add the new node to the DataSet
    data.nodes.add(newNode);
}

function addNewEdge(NodeId, parentId, color) {
    const newEdge = {
        from: NodeId,
        to: parentId,
        arrows: 'to',
        color: { color: color } // Example color, customize as needed
    };
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

    /*  while (!haveAllNodesFilledTitle(nodes)) {*/
    var nodes = data.nodes.get();
    var edges = data.edges.get();
    console.log(nodes);
    console.log(edges);
        for (let i = 0; i < nodes.length; i++) {
            if (nodes[i].title == '▭') {
                var nodesWithParentId = getNodesByParentId(nodes, nodes[i].id)
                if (haveChildNodesFilledTitle(nodesWithParentId))
                {
                    var neededEdges = getEdges(edges, nodesWithParentId);
                    modifyNodes(nodes, nodes[i], nodesWithParentId, neededEdges);
                }
            }
        }
    /*}*/
}

function getNodesByParentId(nodes, parentId) {
    return nodes.filter(node => node.parentId === parentId);
}

function getEdges(edges, nodesWithParentId) {
    console.log("STOP");
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

function modifyNodes(nodes, node, nodesWithParentId, edges) {

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
        addNewEdge(globalInput.id, nodeId, color);
        clickedSecondNode = false;
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
    if (globalInput) {
        data.nodes.remove({ id: globalInput.id });
    }
}

function handleButtonOdstranVetevClick() {
    data.edges.remove(globalEdge);
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