function ConvertImageToArray (obj) {
    console.log("In ImageHandler.js");

    var reader = new FileReader();
    reader.onload = function () {

        var arrayBuffer = this.result,
            array = new Uint8Array(arrayBuffer),
            binaryString = String.fromCharCode.apply(null, array);

        console.log(binaryString);

    }
    reader.readAsArrayBuffer(this.files[0]);

}