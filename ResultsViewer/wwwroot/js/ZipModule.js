
class ZipPacker {
    #zip;
    constructor() {
        this.#zip = new JSZip();
    }
    addFile(fileName, htmlContent) {
        return new Promise((resolve, reject) => {
            console.log(JSON.parse(htmlContent))
            pdfMake.createPdf(JSON.parse(htmlContent)).open();

            resolve();
            //pdfMake.createPdf(JSON.parse(htmlContent)).getBlob(blob => {
            //    this.#zip.file(`${fileName}.pdf`, blob);
            //    resolve();
            //});
        })
        
    }
    saveArchive() {
        this.#zip.generateAsync({ type: "blob" })
            .then(function (content) {
                saveBlobAsFile("example.zip", content);
            });
    }
}

function createZipPacker() {
    return new ZipPacker();
}
