window.onload = function () {
    //Escalamiento de la vista
    var $el = $("#very-specific-design");
    var elHeight = $el.outerHeight();
    var elWidth = $el.outerWidth();

    var $wrapper = $("#scaleable-wrapper");

    $wrapper.resizable({
        resize: doResize
    });

    //Funcion para escalar
    function doResize(event, ui) {
        var scale, origin;
        if (ui.size.width < 1350) {
            equ = (100 - ((ui.size.width / elWidth) * 100)) / 2
            scale = Math.min(
                ui.size.width / elWidth,
                ui.size.height / elHeight
            );

            $el.css({
                transform: "translate(-" + equ + "%, -" + equ + "%) " + "scale(" + scale + ")"
            });
        }
    }

    var starterData = {
        size: {
            width: $wrapper.width(),
            height: $wrapper.height()
        }
    }
    doResize(null, starterData);    
};

                    