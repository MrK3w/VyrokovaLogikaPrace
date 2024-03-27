const labelIdMap = new Map();
var network;
var data;
var globalInput;

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

    nodes.add({ id: 1, label: '▭', title: '▭', parentId: 0, color: { background: '#FFD700' } });
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
        var nodeId = params.nodes[0];
        if (nodeId !== undefined) {
            handleNodeClick(nodeId);
        }
    });
}

function addNewNodeAndEdge(newNodeId, newNodeLabel, newNodeTitle, parentId, color) {
    // Assuming 'nodes' and 'edges' are accessible in this context, e.g., declared globally or passed to this function.
    // Create the new node data
    const newNode = {
        id: newNodeId,
        label: newNodeLabel,
        title: newNodeTitle,
        parentId: parentId,
    };
    // Add the new node to the DataSet
    data.nodes.add(newNode);

    // If parentId is provided, create and add an edge from the new node to the parent node
    if (parentId !== undefined) {
        const newEdge = {
            from: newNodeId,
            to: parentId,
            arrows: 'to',
            color: { color: color } // Example color, customize as needed
        };
        data.edges.add(newEdge);
    }
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

}

function handleButtonPridejNoduClick(color) {

    if (globalInput) {
        // Get all existing node IDs
        var nodeIds = data.nodes.getIds();

        // Find the largest ID among existing nodes
        var largestNodeId = Math.max(...nodeIds);

        // Calculate the new ID by adding 1 to the largest ID
        var newId = largestNodeId + 1;
       
        addNewNodeAndEdge(newId, '▭', '▭',globalInput.id,color)
    }
    
}

function handleButtonOdstranNoduClick() {
    if (globalInput) {
        // Get the ID of the node to be removed
        var nodeIdToRemove = globalInput.id;

        // Remove all edges connected to the node being deleted
        var connectedEdges = data.edges.get({
            filter: function (edge) {
                return edge.from === nodeIdToRemove || edge.to === nodeIdToRemove;
            }
        });
        connectedEdges.forEach(function (edge) {
            data.edges.remove({ id: edge.id });
        });

        // Remove the node itself
        data.nodes.remove({ id: nodeIdToRemove });

        // Get IDs of all remaining nodes
        var remainingNodeIds = data.nodes.getIds();

        // Iterate through all nodes
        remainingNodeIds.forEach(function (nodeId) {
            // Check if the node has any edges connected
            var connectedEdges = data.edges.get({
                filter: function (edge) {
                    return edge.from === nodeId || edge.to === nodeId;
                }
            });

            // If the node has no connected edges, remove it
            if (connectedEdges.length === 0) {
                if(globalInput.parentId != 1)
                data.nodes.remove({ id: nodeId });
            }
        });
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
            console.log(globalInput.nodeId);
            data.nodes.update({id: globalInput.id,label: inputValue});
        }
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
}

function handleNodeClick(nodeId) {
    globalInput = data.nodes.get(nodeId);
    console.log(nodeId);
}

$(document).ready(function() {
    //button for 'Vykresli DAG'
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

    document.getElementById('pridejLevouNoduButton').addEventListener('click', function () {
        handleButtonPridejNoduClick('blue');
    });

    document.getElementById('pridejPravouNoduButton').addEventListener('click', function () {
        handleButtonPridejNoduClick('orange');
    });

    document.getElementById('vytvorFormuliButton').addEventListener('click', handleButtonVytvorFormuliClick);
})