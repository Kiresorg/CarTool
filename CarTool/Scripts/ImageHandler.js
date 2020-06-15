$('imageUpload').on('change', function (e) {
    console.log("changed the file");
});


var ConvertImageToArray = function(event) {
    var input = event.target;

    var output = document.getElementById('output');
    output.src = URL.createObjectURL(event.target.files[0]);

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            console.log(" target.result: " + e.target.result);

            var dataURL = reader.readAsDataURL(input.files[0]);

            var array = new Uint8Array(dataURL);

            var foo = reader.readAsArrayBuffer(input.files[0]);

            binaryString = String.fromCharCode.apply(null, array);

            console.log(binaryString);

        }
    }

}