
import "./lib/pdfmake/pdfmake.min.js";
import "./lib/html-to-pdfmake/browser.min.js";
import "./lib/jszip/jszip.min.js";

pdfMake.fonts = {
    Roboto: {
        normal: location.origin + '/control-panel/js/lib/pdfmake/fonts/Roboto/Roboto-Regular.ttf',
        bold: location.origin + '/control-panel/js/lib/pdfmake/fonts/Roboto/Roboto-Medium.ttf',
        italics: location.origin + '/control-panel/js/lib/pdfmake/fonts/Roboto/Roboto-Italic.ttf',
        bolditalics: location.origin + '/control-panel/js/lib/pdfmake/fonts/Roboto/Roboto-MediumItalic.ttf'
    },
    Robotomono: {
        normal: location.origin + '/control-panel/js/lib/pdfmake/fonts/RobotoMono/fonts/RobotoMono-Regular.ttf',
    },
}

// Supported styles by html-to-pdfmake library https://github.com/Aymkdn/html-to-pdfmake#css-properties-supported
const supportedStyles = [
    //"background-color", // compyted styles returns black
    "border",
    "color",
    //"font-family", // Disable built in fonts
    "font-style",
    "font-weight",
    "height",
    "margin",
    "text-align",
    //"text-decoration", // unused for code block
    "text-indent",
    "white-space",
    "width"
]

// https://css-tricks.com/converting-color-spaces-in-javascript/
function RGBAToHexA(rgba) {
    let sep = rgba.indexOf(",") > -1 ? "," : " ";
    rgba = rgba.substr(rgba.indexOf("(") + 1).split(")")[0].split(sep);

    // Strip the slash if using space-separated syntax
    if (rgba.indexOf("/") > -1)
        rgba.splice(3, 1);

    for (let R in rgba) {
        let r = rgba[R];
        if (r.indexOf("%") > -1) {
            let p = r.substr(0, r.length - 1) / 100;

            if (R < 3) {
                rgba[R] = Math.round(p * 255);
            } else {
                rgba[R] = p;
            }
        }
    }

    let r = (+rgba[0]).toString(16),
        g = (+rgba[1]).toString(16),
        b = (+rgba[2]).toString(16),
        a = Math.round(+rgba[3] * 255).toString(16);

    if (r.length == 1)
        r = "0" + r;
    if (g.length == 1)
        g = "0" + g;
    if (b.length == 1)
        b = "0" + b;
    if (a.length == 1)
        a = "0" + a;

    return "#" + r + g + b + (a !== "NaN" ? a : "");
}

export async function createSingleReport(htmlRow, fileName) {
    const reportBuffer = await createReport(htmlRow);
    window.saveAsFile(fileName || "file.pdf", reportBuffer);
}

export function isSaveToFileSystemSupported() {
    return typeof window.showDirectoryPicker === 'function';
}

export async function getMultiReportsSaver(reportsPackName, fallbackCallbackObject) {
    if (!isSaveToFileSystemSupported()) {
        return new ZipPacker(reportsPackName);
    }
    try {
        const dirHandler = await window.showDirectoryPicker();
        return new MultiReportsSaver(dirHandler);
    } catch (error) {
        console.warn("getMultiReportsSaver", error);
        console.log("fallback to zip file logic");
        fallbackCallbackObject.invokeMethodAsync("ShowUseZipFileFallback");
        return new ZipPacker(reportsPackName);
    }
}

class MultiReportsSaver {
    directoryHandle;
    constructor(directoryHandle) {
        this.directoryHandle = directoryHandle;
    }
    async saveReport(htmlRow, fileName) {
        const fileHandle = await this.directoryHandle.getFileHandle(fileName, { create: true });
        const reportBuffer = await createReport(htmlRow);
        const fileStream = await fileHandle.createWritable();
        await fileStream.write(reportBuffer);
        await fileStream.close();
    }
    doneSaving() {
        delete this.directoryHandle;
    }
}

class ZipPacker {
    zip;
    reportsPackName;
    constructor(reportsPackName) {
        this.zip = new JSZip();
        this.reportsPackName = reportsPackName;
    }
    async saveReport(htmlRow, fileName) {
        const reportBuffer = await createReport(htmlRow);
        this.zip.file(fileName, reportBuffer);
    }
    async doneSaving() {
        var zipFileContent = await this.zip.generateAsync({ type: "blob" });
        const fileName = this.reportsPackName || "reports";
        window.saveAsFile(`${fileName}.zip`, zipFileContent);
    }
}

function createReport(htmlRow) {
    return new Promise((resolve, reject) => {
        try {

            const wrapper = document.createElement("div");
            wrapper.innerHTML = htmlRow;
            wrapper.style.display = "none";
            Prism.highlightAllUnder(wrapper);
            const childs = [...wrapper.querySelectorAll("code span.token")];
            document.body.appendChild(wrapper);
            for (let element of childs) {
                const elementComputedStyles = window.getComputedStyle(element, '');
                for (var styleKey of supportedStyles) {
                    const exportedValue = elementComputedStyles.getPropertyValue(styleKey);
                    element.style.setProperty(styleKey, exportedValue);
                }
            }

            for (let code of wrapper.querySelectorAll("code")) {
                code.style.whiteSpace = "pre-wrap";
                code.style.fontFamily = "RobotoMono"
            }

            for (let img of wrapper.querySelectorAll("img")) {
                img.width = "550";
            }

            htmlRow = wrapper.innerHTML.replaceAll(/rgba?\([^\)]+\)/g, RGBAToHexA);
            wrapper.remove();
            var documentDefinition = htmlToPdfmake(htmlRow, {
                imagesByReference: true
            });
            pdfMake.createPdf(documentDefinition).getBuffer(buffer => {
                resolve(buffer);
            });
        } catch (error) {
            console.error("createReport", error);
            reject(error);
        }
    })
}
