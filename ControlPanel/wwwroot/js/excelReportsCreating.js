import "./lib/exceljs/exceljs.min.js";

export async function createReport(challengeName, tableData, exercisesData) {
    const workbook = new ExcelJS.Workbook();
    workbook.creator = workbook.lastModifiedBy = 'RTUITLab Olympiad';
    const sheet = workbook.addWorksheet("Результаты");

    sheet.columns = [
        { header: 'Id', key: 'studentId' },
        { header: 'Имя', key: 'name' },
        ...exercisesData.map(ed => ({ header: ed.name, key: `exercise_${ed.id}` })),
        { header: 'Баллы', key: 'totalScore' }
    ];
    sheet.views = [
        { state: 'frozen', xSplit: 2, ySplit: 1 }
    ];
    sheet.autoFilter = {
        from: {
            row: 1,
            column: 1
        },
        to: {
            row: 1,
            column: sheet.columns.length
        }
    }
    const rows = tableData.map(r => ({
        studentId: r.user.studentId,
        name: r.user.firstName,
        ...exercisesData.reduce((a, e) => ({ ...a, [`exercise_${e.id}`]: r.scores[e.id] }), {}),
        totalScore: r.totalScore
    }))
    sheet.addRows(rows);

    const buffer = await workbook.xlsx.writeBuffer();
    const fileName = challengeName || "Отчет";
    window.saveAsFile(`${fileName}.xlsx`, buffer);
}
