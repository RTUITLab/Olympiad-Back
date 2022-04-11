import "./lib/jszip/jszip.min.js";


export async function createZipArchive() {
    return new JsZipWrapper(new JSZip());
}

class JsZipWrapper {
    zipRef;
    constructor(zipRef) {
        this.zipRef = zipRef;
    }
    textFile(filePath, fileContent) {
        this.zipRef.file(filePath, fileContent);
    }
    async blobFileFromLink(filePath, link) {
        const response = await fetch(link);
        if (response.status != 200) {
            throw 'status_is_not_200';
        }
        const data = await response.blob();
        this.zipRef.file(filePath, data);
    }
    async saveFile(fileName) {
        const result = await this.zipRef.generateAsync({ type: "blob" });
        saveAsFileFromBlob(fileName, result);
    }
}