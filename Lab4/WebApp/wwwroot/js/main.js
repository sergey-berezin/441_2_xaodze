let experiments = []; // 用于存储实验列表
let keepRunning = false; // 控制循环的全局变量

async function createExperiment() {
    const square1num = parseInt(document.getElementById('txtSquare1').value) || 2;
    const square2num = parseInt(document.getElementById('txtSquare2').value) || 3;
    const square3num = parseInt(document.getElementById('txtSquare3').value) || 4;
    const experimentName = prompt("Enter Experiment dsName:", "TestExperiment");

    if (!experimentName) {
        alert("ExperimentName is required");
        return;
    }
    let squareSizes = [];

    for (let i = 0; i < parseInt(square1num, 10); i++) {
        squareSizes.push(1); // 添加边长为 1 的正方形
    }

    for (let i = 0; i < parseInt(square2num, 10); i++) {
        squareSizes.push(2); // 添加边长为 2 的正方形
    }

    for (let i = 0; i < parseInt(square3num, 10); i++) {
        squareSizes.push(3); // 添加边长为 3 的正方形
    }
    const response = await fetch('/experiments', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ExperimentName: experimentName,
            PopulationSize: 1000,
            SquareSizes: squareSizes
        })
    });

    if (response.ok) {
        const data = await response.json();
        experiments.push(data.experimentId); // 添加到实验列表
        updateExperimentList();
        updateStatusText(`Experiment created with ID: ${data.experimentId}`);
    } else {
        const errorText = await response.text();
        updateStatusText("Failed to create experiment: " + errorText);
    }
}

async function runSteps() {
    const experimentName = prompt("Enter Experiment Name to Run Steps:");

    if (!experimentName) {
        alert("Experiment name is required.");
        return;
    }

    keepRunning = true; // 控制循环的变量

    while (keepRunning) {
        const response = await fetch(`/experiments/${experimentName}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
        });

        if (response.ok) {
            const data = await response.json();
            console.log(data.bestIndividual);

            // 更新状态
            updateStatusText(
                `Generation: ${data.generation}\nBest Loss: ${data.bestIndividual.loss}\nBest Squares: ${JSON.stringify(data.bestIndividual.squares)}`
            );
            // 绘制正方形
            drawSquares(data.bestIndividual.squares.map(s => ({
                x: s.x,
                y: s.y,
                sideLength: s.sideLength
            })));

            // 控制循环的延时，避免请求过于频繁
            await new Promise(resolve => setTimeout(resolve, 300));
        } else {
            const errorText = await response.text();
            updateStatusText("Failed to run steps: " + errorText);
            keepRunning = false; // 停止循环
        }
    }
}

function stopSteps() {
    keepRunning = false; // 设置控制变量，停止循环
    updateStatusText("Stopped running steps.");
}


function updateStatusText(text) {
    const outputElement = document.getElementById('outputText');
    outputElement.innerText += text + '\n';
    outputElement.scrollTop = outputElement.scrollHeight; // 滚动到最新内容
}

function drawSquares(squares) {
    const canvas = document.getElementById('canvasRectangles');
    const ctx = canvas.getContext('2d');
    const scale = 20;

    // 清空画布
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // 绘制背景网格
    drawGrid(ctx, canvas.width, canvas.height, scale);

    // 绘制每个正方形
    squares.forEach(sq => {
        const x = sq.x * scale;
        const y = canvas.height - (sq.y + sq.sideLength) * scale;
        const size = sq.sideLength * scale;

        ctx.fillStyle = sq.sideLength === 1 ? 'AliceBlue' :
            sq.sideLength === 2 ? 'LightSalmon' : 'LightPink';
        ctx.strokeStyle = 'black';

        ctx.fillRect(x, y, size, size);
        ctx.strokeRect(x, y, size, size);
    });
}

function drawGrid(ctx, width, height, step) {
    ctx.strokeStyle = '#ddd';
    ctx.lineWidth = 0.5;

    for (let y = 0; y <= height; y += step) {
        ctx.beginPath();
        ctx.moveTo(0, y);
        ctx.lineTo(width, y);
        ctx.stroke();
    }

    for (let x = 0; x <= width; x += step) {
        ctx.beginPath();
        ctx.moveTo(x, 0);
        ctx.lineTo(x, height);
        ctx.stroke();
    }
}

async function deleteExperiment() {
    const experimentName = prompt("Enter Experiment Name to delete:");
    if (!experimentName) {
        alert("Experiment name is required.");
        return;
    }

    const response = await fetch(`/experiments/${experimentName}`, { method: 'DELETE' });

    if (response.ok) {
        experiments = experiments.filter(e => e !== experimentName); // 从列表中移除
        updateExperimentList();
        updateStatusText("Experiment deleted.");
    } else {
        const errorText = await response.text();
        updateStatusText("Failed to delete experiment: " + errorText);
    }
}

async function fetchExperiments() {
    const response = await fetch('/experiments', { method: 'GET' });
    if (response.ok) {
        experiments = await response.json();
        updateExperimentList();
    } else {
        const errorText = await response.text();
        updateStatusText("Failed to fetch experiments: " + errorText);
    }
}

function updateExperimentList() {
    const listElement = document.getElementById('experimentList');
    listElement.innerHTML = ""; // 清空列表
    experiments.forEach(exp => {
        const item = document.createElement('li');
        item.textContent = exp;
        listElement.appendChild(item);
    });
}

function updateStatusText(text) {
    document.getElementById('outputText').innerText = text;
}
