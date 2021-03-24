
// Use localhost for prism while use self builded image
const header = `
<html>
<head>
    <meta charset="UTF-8">
    <link rel="stylesheet" href="http://localhost/prism.css" />
    <script src="http://localhost/prism.js" ></script>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Roboto" />
    <style>
        * {
            font-family: 'Roboto';
        }
    </style>
</head>
<body>
`
const footer = `
</body>
</html>
`

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
        htmlContent = `${header}${htmlContent}${footer}`;
        this.addStringFile(`${fileName}.html`, htmlContent);
        const formData = new FormData();
        formData.append('html', htmlContent);
        const response = await fetch(this.printAddress, {
            method: 'POST',
            body: formData
        });
        const blob = await response.blob();
        this.zip.file(fileName, blob);
    }
    saveArchive(fileName) {
        this.zip.generateAsync({ type: "blob" })
            .then(function (content) {
                saveBlobAsFile(`${fileName}.zip`, content);
            });
    }
}

function createZipPacker(printAddress) {
    return new ZipPacker(printAddress);
}
