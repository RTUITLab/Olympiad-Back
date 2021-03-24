
class ZipPacker {
    zip;
    printAddress;
    constructor(printAddress) {
        this.zip = new JSZip();
        this.printAddress = printAddress;
    }
    addStringFile(fileName, stringContent) {
        this.zip.file(fileName, stringContent);

    }
    async addHtmlToPdfFile(fileName, htmlContent) {
        const formData = new FormData();
        formData.append('html', htmlContent);
        const response = await fetch(this.printAddress, {
            method: 'POST',
            body: formData
        });
        const blob = await response.blob();
        this.zip.file(fileName, blob);
    }
    saveArchive() {
        this.zip.generateAsync({ type: "blob" })
            .then(function (content) {
                saveBlobAsFile("example.zip", content);
            });
    }
}

function createZipPacker(printAddress) {
    return new ZipPacker(printAddress);
}
