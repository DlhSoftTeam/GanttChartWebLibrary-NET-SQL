var controllerPath, ganttChartView, theme;

function initializeGanttChart(controllerPathValue, viewReference, themeValue) {
    controllerPath = controllerPathValue;
    ganttChartView = viewReference;
    theme = themeValue;
    var settings = ganttChartView.settings;
    initializeGanttChartTheme(settings, theme);
}
function addNewGanttChartItem() {
    var date = new Date(), year = date.getFullYear(), month = date.getMonth(); day = date.getDate()
    var item = {
        content: 'New task', indentation: 0, isMilestone: false,
        start: new Date(year, month, day, 8, 0, 0), finish: new Date(year, month, day, 16, 0, 0),
        completedFinish: new Date(year, month, day, 8, 0, 0)
    };
    fetch(controllerPath + '/CreateNewGanttChartItem', {
        method: 'POST',
        headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        body: JSON.stringify(item)
    }).then(response => response.json()).then(taskId => {
        item.key = taskId;
        ganttChartView.addItem(item);
        ganttChartView.selectItem(item);
        ganttChartView.scrollToItem(item);
        ganttChartView.scrollToDateTime(item.start);
    });
}
function deleteGanttChartItem() {
    var selectedItem = ganttChartView.selectedItem;
    if (!selectedItem)
        return;
    if (!confirm("Are you sure you want to delete the selected task?"))
        return;
    fetch(controllerPath + '/DeleteGanttChartItem?id=' + selectedItem.key).then(response => {
        if (response.ok)
            ganttChartView.removeItem(selectedItem);
    });
}