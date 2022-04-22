var controllerPath, ganttChartView, theme;

function initializeGanttChart(controllerPathValue, viewReference, themeValue) {
    controllerPath = controllerPathValue;
    ganttChartView = viewReference;
    theme = themeValue;
    var settings = ganttChartView.settings;
    initializeGanttChartTheme(settings, theme);
}