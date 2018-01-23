//Toogle left side navigation menu
var toggleSideNav = {
    open: 0,
    toggle: function () {
        if (this.open == 0) {
            openNav();
            this.open = 1;
        }
        else {
            closeNav();
            this.open = 0;
        }
        setTimeout(function () {
            setSliderHeight();
        },
        500);
}
};

$('#toggleSidenav').on('click', function () {
    $('#sb-wrapper').css('opacity', 1);
    toggleSideNav.toggle();
});

var openNav = function () {
    $("#sb-wrapper").css('width', '250px');
    //$("#mainBody").css('margin-left', '250px');
    $("#imgToggle").css('transform', 'rotate(180deg)');
    $('#toggleSidenav').addClass("on");
    $('.sidebar').removeClass("sidebar-close");
    $('.sidebar').addClass("sidebar-open");
};

var closeNav = function () {
    $("#sb-wrapper").css('width', '0px');
    //$("#mainBody").css('margin-left', '0px');
    $("#imgToggle").css('transform', 'none');
    $('#toggleSidenav').removeClass("on");
    $('.sidebar').removeClass("sidebar-open");
    $('.sidebar').addClass("sidebar-close");
};

var setSliderHeight = function() {
    $('.slider').css('height', $('#cd').height());
};

$(window).resize(function () {
    setSliderHeight();
});

$(function () {
    setSliderHeight();
//    if (toggleSideNav.open == 1) {
//        alert($('#cd').height());
//        setSliderHeight();
//    }
});


//Login popup
/*
<script type="text/javascript">
    $('#loginLink').on('click', function () {
        $.ajax({
            url: "/Account/Login",
            cache: false,
            method: "get",
            success: function (html) {
                $('body').append('<div id="over"></div>');
                $('#over').fadeIn(300);
                $('#loginForm').append(html);
                $('#loginForm').fadeIn("slow");
            },
        });
    });

$('#registerLink').on('click', function () {
    $.ajax({
        url: "/Account/Create",
        cache: false,
        method: "get",
        success: function (html) {
            $('body').append('<div id="over"></div>');
            $('#over').fadeIn(300);
            $('#registerForm').append(html);
            $('#registerForm').fadeIn("slow");
        },
    });
});

//Click on #over will close the login box
$(document).on('click', "#over", function () {
    $('#over, #loginForm, #registerForm').fadeOut(300, function () {
        $('#over').remove();
    });
});
*/

