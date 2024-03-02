function toggleNode(nodeId, treeValue, op, position) {
    var node = document.getElementById('node_' + nodeId);
    var span = node.querySelector('.tf-nc');
    console.log(op);
    // Toggle between tree.Text and op
    var currentValue = span.innerText;
    span.innerText = (currentValue === treeValue) ? op : treeValue;

    // Check if toggling to treeValue
    if (span.innerText === treeValue && treeValue.length >= 2) {
        var newText;
        if (treeValue[position + 1] == '¬' && treeValue[position] == '¬') newText = span.innerText.substr(0, position) + '<span style="color: red;">' + span.innerText.substr(position, 2) + '</span>' + span.innerText.substr(position + 2);

        else newText = span.innerText.substr(0, position) + '<span style="color: red;">' + span.innerText.substr(position, 1) + '</span>' + span.innerText.substr(position + 1);
        span.innerHTML = newText;
        

    }
}